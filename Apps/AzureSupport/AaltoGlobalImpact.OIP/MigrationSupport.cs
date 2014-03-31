using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;
using TheBall.CORE;

namespace AaltoGlobalImpact.OIP
{
    public static class MigrationSupport
    {
        public static void MigrateAaltoGlobalImpactContent(Process process, string sourceContentRoot, string targetContentRoot, Dictionary<string, string> categoryMap)
        {
            if (targetContentRoot != string.Empty && targetContentRoot.EndsWith("/") == false)
                targetContentRoot += "/";
            var owner = InformationContext.CurrentOwner;
            var argDict = process.InitialArguments.ToDictionary(item => item.ItemFullType, item => item.ItemValue);
            string mediaContentFullName = typeof(MediaContent).FullName;
            string processID = process.ID;
            var sourceBlobs = owner.GetOwnerBlobListing(sourceContentRoot, true).Cast<CloudBlockBlob>().ToArray();
            Dictionary<string, ProcessItem> sourceProcessItems = getMigrationProcessItemDict(process);
            List<ProcessItem> processItemsToAdd;
            List<ProcessItem> processItemsToUpdate;
            List<ProcessItem> processItemsToDelete;
            determineHowToProcessBlobs(sourceBlobs, sourceProcessItems, out processItemsToAdd, out processItemsToUpdate, out processItemsToDelete);

            processItemsToDelete.ForEach(itemToDelete =>
                {
                    deleteProcessItem(itemToDelete);
                    process.ProcessItems.Remove(itemToDelete);
                });
            string targetCategoryCollectionLocation = string.Format("{0}AaltoGlobalImpact.OIP/CategoryCollection/MasterCollection",
                                                                    targetContentRoot);
            var targetCategoryCollection = CategoryCollection.RetrieveCategoryCollection(targetCategoryCollectionLocation, owner);
            process.ProcessItems.AddRange(processItemsToAdd);
            var processItemsToProcess = processItemsToAdd.Union(processItemsToUpdate);
            foreach (var processItem in processItemsToProcess)
            {
                string sourceLocation = processItem.GetInputValue("SourceLocation");
                string informationObjectType = processItem.GetInputValue("InformationObjectType");
                string contentMD5 = null;
                string eTag = null;
                List<CloudBlob> storedBlobs = new List<CloudBlob>();
                string targetLocation = null;
                if (informationObjectType != null && informationObjectType != mediaContentFullName)
                {
                    var iObject = StorageSupport.RetrieveInformation(sourceLocation, informationObjectType, null, owner);
                    CloudBlob storedBlob = null;
                    if (iObject is TextContent)
                    {
                        storedBlob = processTextContent((TextContent)iObject, targetContentRoot, categoryMap,
                            targetCategoryCollection, processID);
                    }
                    if (storedBlob != null)
                    {
                        processItem.SetOutputValue("InformationObjectType", informationObjectType);
                        targetLocation = StorageSupport.RemoveOwnerPrefixIfExists(storedBlob.Name);
                        contentMD5 = storedBlob.Properties.ContentMD5;
                        eTag = storedBlob.Properties.ETag;
                        processItem.SetOutputValue("ContentMD5", contentMD5);
                        processItem.SetOutputValue("ETag", eTag);
                        processItem.SetOutputValue("TargetLocation", targetLocation);
                    }
                }
                else if(informationObjectType == mediaContentFullName)
                {
                    string fileNamePart = Path.GetFileName(sourceLocation);
                    string targetBlobPath = StorageSupport.GetOwnerContentLocation(owner,
                                                                                          string.Format("{0}AaltoGlobalImpact.OIP/MediaContent/{1}", targetContentRoot, fileNamePart));
                    var targetBlob = StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, targetBlobPath);
                    var sourceBlob = StorageSupport.GetOwnerBlobReference(InformationContext.CurrentOwner, sourceLocation);
                    targetBlob.CopyFromBlob(sourceBlob);
                    targetLocation = StorageSupport.RemoveOwnerPrefixIfExists(targetBlob.Name);
                    contentMD5 = targetBlob.Properties.ContentMD5;
                    eTag = targetBlob.Properties.ETag;
                    processItem.SetOutputValue("ContentMD5", contentMD5);
                    processItem.SetOutputValue("ETag", eTag);
                    processItem.SetOutputValue("TargetLocation", targetLocation);

                }
            }
        }

        private static CloudBlob processTextContent(TextContent textContent, string targetContentRootLocation, Dictionary<string, string> categoryMap, CategoryCollection targetCategoryCollection, string processID)
        {
            var owner = InformationContext.CurrentOwner;
            var sourceCategories = textContent.Categories;
            var targetItemCategories = GetTargetItemCategories(sourceCategories, targetCategoryCollection, categoryMap);
            textContent.Categories = targetItemCategories;
            textContent.RelativeLocation = StorageSupport.GetOwnerContentLocation(owner,
                                                                                  string.Format("{0}AaltoGlobalImpact.OIP/TextContent/{1}", targetContentRootLocation, textContent.ID));
            textContent.GeneratedByProcessID = processID;
            if (textContent.ImageData != null)
            {
                textContent.ImageData.FixCurrentOwnerLocation();
            }
            if(textContent.Locations != null)
                textContent.Locations.CollectionContent.ForEach(location => location.FixCurrentOwnerLocation());
            var storedBlob = textContent.StoreInformationMasterFirst(owner, true, overwriteIfExists: true);
            return storedBlob;
        }


        private static void deleteProcessItem(ProcessItem itemToDelete)
        {
            try
            {
                string informationObjectType = itemToDelete.GetOutputValue("InformationObjectType");
                string targetLocation = itemToDelete.GetOutputValue("TargetLocation");
                if (string.IsNullOrEmpty(targetLocation))
                    throw new InvalidDataException("Target location must not be null");
                if (informationObjectType != null)
                {
                    var iObject = StorageSupport.RetrieveInformation(targetLocation, informationObjectType, null, InformationContext.CurrentOwner);
                    iObject.DeleteInformationObject();
                }
                else
                {
                    StorageSupport.DeleteBlobsFromOwnerTarget(InformationContext.CurrentOwner, targetLocation);
                }
            }
            catch
            {
                
            }
        }

        private static void determineHowToProcessBlobs(CloudBlockBlob[] sourceBlobs, Dictionary<string, ProcessItem> sourceProcessItems, out List<ProcessItem> processItemsToAdd, out List<ProcessItem> processItemsToUpdate, out List<ProcessItem> processItemsToDelete)
        {
            processItemsToAdd = new List<ProcessItem>();
            processItemsToUpdate = new List<ProcessItem>();
            processItemsToDelete = new List<ProcessItem>();
            HashSet<string> processedBlobs = new HashSet<string>();
            foreach (var sourceBlob in sourceBlobs)
            {
                string candidateType = sourceBlob.GetBlobInformationObjectType();
                bool continueProcessing = candidateType == "AaltoGlobalImpact.OIP.TextContent" || candidateType == "AaltoGlobalImpact.OIP.MediaContent";
                if (!continueProcessing)
                    continue;
                string ownerStrippedName = StorageSupport.RemoveOwnerPrefixIfExists(sourceBlob.Name);
                processedBlobs.Add(ownerStrippedName);
                ProcessItem currProcessItem = null;
                sourceProcessItems.TryGetValue(ownerStrippedName, out currProcessItem);
                bool isAddingNewItem = false;
                if (currProcessItem == null)
                {
                    currProcessItem = new ProcessItem();
                    currProcessItem.Inputs.Add(new SemanticInformationItem("SourceLocation", ownerStrippedName));
                    currProcessItem.Inputs.Add(new SemanticInformationItem("InformationObjectType", candidateType));
                    currProcessItem.Inputs.Add(new SemanticInformationItem("SourceMD5", ""));
                    processItemsToAdd.Add(currProcessItem);
                    sourceProcessItems.Add(ownerStrippedName, currProcessItem);
                    isAddingNewItem = true;
                }
                bool sourceChangedSinceProcessing = currProcessItem.GetInputValue("SourceMD5") != sourceBlob.Properties.ContentMD5;
                if (!sourceChangedSinceProcessing)
                    continue;
                if(!isAddingNewItem)
                    processItemsToUpdate.Add(currProcessItem);
                currProcessItem.SetInputValue("SourceMD5", sourceBlob.Properties.ContentMD5);
                if (candidateType == "AaltoGlobalImpact.OIP.TextContent")
                {
                    
                } else if (candidateType == "AaltoGlobalImpact.OIP.MediaContent")
                {
                    
                }
            }
            var keysToDelete = sourceProcessItems.Keys.Where(key => processedBlobs.Contains(key) == false);
            processItemsToDelete.AddRange(keysToDelete.Select(key => sourceProcessItems[key]));
        }

        private static Dictionary<string, ProcessItem> getMigrationProcessItemDict(Process process)
        {
            var result = process.ProcessItems
                                .Where(pi => pi.Inputs.FirstOrDefault(inp => inp.ItemFullType == "SourceLocation") != null)
                                .ToDictionary(pi => pi.Inputs.FirstOrDefault(inp => inp.ItemFullType == "SourceLocation").ItemValue, pi => pi);
            return result;
        }

        public static CategoryCollection GetTargetItemCategories(CategoryCollection sourceCategories, CategoryCollection targetCategoryCollection, Dictionary<string, string> categoryMap)
        {
            var targetItemCategories = new CategoryCollection();
            //targetItemCategories.CollectionContent.AddRange(targetCategoryCollection.CollectionContent);
            var activeCategories = sourceCategories.GetIDSelectedArray();
            List<string> targetCategoryIDs = new List<string>();
            foreach (var activeCategory in activeCategories)
            {
                if (categoryMap.ContainsKey(activeCategory.ID))
                {
                    targetCategoryIDs.Add(categoryMap[activeCategory.ID]);
                }
            }
            targetCategoryIDs = targetCategoryIDs.Distinct().ToList();
            //targetItemCategories.SelectedIDCommaSeparated = string.Join(",", targetCategoryIDs.ToArray());
            targetItemCategories.CollectionContent
                                .AddRange(targetCategoryIDs
                                              .Select(catID => targetCategoryCollection.CollectionContent.FirstOrDefault(cat => cat.ID == catID))
                                              .Where(cat => cat != null));
            return targetItemCategories;
        }
    }
}