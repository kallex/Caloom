using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Web;
using TheBall.CORE;
using AaltoGlobalImpact.OIP;

namespace TheBall
{
    public static class ModifyInformationSupport
    {
        public static void ExecuteOwnerWebPOST(IContainerOwner containerOwner, NameValueCollection form, HttpFileCollection fileContent)
        {
            bool isCancelButton = form["btnCancel"] != null;
            if (isCancelButton)
                return;

            string operationName = form["ExecuteOperation"];
            if (operationName != null)
            {
                executeOperationWithFormValues(containerOwner, operationName, form, fileContent);
                return;
            }

            string contentSourceInfo = form["ContentSourceInfo"];
            string[] contentSourceInfos = contentSourceInfo.Split(',');
            NameValueCollection fileEntries = new NameValueCollection();
            NameValueCollection fieldEntries = new NameValueCollection();
            NameValueCollection objectEntries = new NameValueCollection();
            foreach (var key in form.AllKeys)
            {
                var value = form[key];
                if (key.StartsWith("File_"))
                    fileEntries.Add(key, value);
                else if (key.StartsWith("Object_"))
                    objectEntries.Add(key, value);
                else
                    fieldEntries.Add(key, value);
            
            }
            foreach (var key in fileContent.AllKeys)
            {
                if (key.StartsWith("File_") && fileEntries.AllKeys.Contains(key) == false)
                    fileEntries.Add(key, "");
            }
            foreach (string sourceInfo in contentSourceInfos)
            {
                string relativeLocation;
                string oldETag;
                retrieveDataSourceInfo(sourceInfo, out relativeLocation, out oldETag);
                VirtualOwner verifyOwner = VirtualOwner.FigureOwner(relativeLocation);
                if (verifyOwner.IsSameOwner(containerOwner) == false)
                    throw new SecurityException("Mismatch in ownership of data submission");
                IInformationObject rootObject = StorageSupport.RetrieveInformation(relativeLocation, oldETag,
                                                                                   containerOwner);
                if (oldETag != rootObject.ETag)
                {
                    throw new InvalidDataException("Information under editing was modified during display and save");
                }
                // TODO: Proprely validate against only the object under the editing was changed (or its tree below)
                SetFieldValues(rootObject, fieldEntries);
                SetBinaryContent(rootObject, fileEntries, fileContent, containerOwner);
                SetObjectLinks(rootObject, objectEntries);

                /* Operation bridge model below - not used/needed with field assignment solution */
                /*
                var removeMediaList = form["cmdRemoveMedia"];
                if (String.IsNullOrWhiteSpace(removeMediaList) == false)
                {
                    string[] removeList = removeMediaList.Split(',');
                    foreach (string contentInfo in removeList)
                    {
                        SetBinaryContent(containerOwner, contentInfo, rootObject, null);
                    }
                }
                 * */
                rootObject.StoreInformationMasterFirst(containerOwner, false);
            }

        }

        private static void executeOperationWithFormValues(IContainerOwner containerOwner, string operationName, NameValueCollection form, HttpFileCollection fileContent)
        {
            var filterFields = new string[] {"ExecuteOperation", "ObjectDomainName", "ObjectName", "ObjectID"};
            switch (operationName)
            {
                case "InviteMemberToGroup":
                    {
                        if (containerOwner.IsGroupContainer() == false)
                            throw new InvalidOperationException("Group invitation is only supported in group context");
                        string emailAddress = form["EmailAddress"];
                        string emailRootID = TBREmailRoot.GetIDFromEmailAddress(emailAddress);
                        TBREmailRoot emailRoot = TBREmailRoot.RetrieveFromDefaultLocation(emailRootID);
                        if(emailRoot == null)
                            throw new NotSupportedException("Email used for group invitation is not yet registered to the system");
                        string groupID = containerOwner.LocationPrefix;
                        InviteMemberToGroup.Execute(new InviteMemberToGroupParameters { GroupID = groupID, MemberEmailAddress = emailAddress });
                        break;
                    }

                case "CreateGroupWithTemplates":
                    {
                        var owningAccount = containerOwner as TBAccount;
                        if(owningAccount == null)
                            throw new NotSupportedException("Creating a group is only supported by account");
                        CreateGroupWithTemplatesParameters parameters = new CreateGroupWithTemplatesParameters
                            {
                                AccountID = owningAccount.ID,
                                GroupName = form["GroupName"],
                                RedirectUrlAfterCreation = form["RedirectUrlAfterCreation"],
                                TemplateNameList = form["TemplateNameList"]
                            };
                        CreateGroupWithTemplates.Execute(parameters);
                        break;
                    }
                case "UnregisterEmailAddress":
                    {
                        var owningAccount = containerOwner as TBAccount;
                        if(owningAccount == null)
                            throw new NotSupportedException("Unregistering email address is only supported for accounts");
                        UnregisterEmailAddressParameters parameters = new UnregisterEmailAddressParameters
                            {
                                AccountID = owningAccount.ID,
                                EmailAddress = form["EmailAddress"],
                            };
                        UnregisterEmailAddress.Execute(parameters);
                        break;
                    }
                case "BeginAccountEmailAddressRegistration":
                    {
                        var owningAccount = containerOwner as TBAccount;
                        if(owningAccount == null)
                            throw new NotSupportedException("Email address registration is only supported for accounts");
                        BeginAccountEmailAddressRegistrationParameters parameters = new BeginAccountEmailAddressRegistrationParameters
                            {
                                AccountID = owningAccount.ID,
                                RedirectUrlAfterValidation = form["RedirectUrlAfterValidation"],
                                EmailAddress = form["EmailAddress"],
                            };
                        BeginAccountEmailAddressRegistration.Execute(parameters);
                        break;
                    }
                case "CreateInformationInput":
                    {
                        CreateInformationInputParameters parameters = new CreateInformationInputParameters
                            {
                                InputDescription = form["InputDescription"],
                                LocationURL = form["LocationURL"],
                                Owner = containerOwner
                            };
                        var createdInformationInput = CreateInformationInput.Execute(parameters);
                        var owningAccount = containerOwner as TBAccount;
                        TBCollaboratingGroup owningGroup = null;
                        if (owningAccount == null)
                        {
                            TBRGroupRoot groupRoot =
                                TBRGroupRoot.RetrieveFromDefaultLocation(containerOwner.LocationPrefix);
                            owningGroup = groupRoot.Group;
                        }
                        CreateAndSendEmailValidationForInformationInputConfirmationParameters emailParameters = new CreateAndSendEmailValidationForInformationInputConfirmationParameters
                            {
                                OwningAccount = owningAccount,
                                OwningGroup = owningGroup,
                                InformationInput = createdInformationInput.InformationInput,
                            };
                        CreateAndSendEmailValidationForInformationInputConfirmation.Execute(emailParameters);
                        break;
                    }
                case "CreateSpecifiedInformationObjectWithValues":
                    {
                        CreateSpecifiedInformationObjectWithValuesParameters parameters = new CreateSpecifiedInformationObjectWithValuesParameters
                            {
                                Owner = containerOwner,
                                ObjectDomainName = form["ObjectDomainName"],
                                ObjectName = form["ObjectName"],
                                HttpFormData = filterForm(form, filterFields),
                                HttpFileData = fileContent,
                            };
                        CreateSpecifiedInformationObjectWithValues.Execute(parameters);
                        break;
                    }
                case "DeleteSpecifiedInformationObject":
                    {
                        DeleteSpecifiedInformationObjectParameters parameters = new DeleteSpecifiedInformationObjectParameters
                            {
                                Owner = containerOwner,
                                ObjectDomainName = form["ObjectDomainName"],
                                ObjectName = form["ObjectName"],
                                ObjectID = form["ObjectID"],
                            };
                        DeleteSpecifiedInformationObject.Execute(parameters);
                        break;
                    }
                default:
                    throw new NotSupportedException("Operation not (yet) supported: " + operationName);
            }
        }

        private static NameValueCollection filterForm(NameValueCollection form, params string[] keysToFilter)
        {
            var filteredForm = new NameValueCollection();
            foreach (var key in form.AllKeys)
            {
                if (keysToFilter.Contains(key))
                    continue;
                filteredForm.Add(key, form[key]);
            }
            return filteredForm;
        }

        public static void SetObjectLinks(IInformationObject rootObject, NameValueCollection objectEntries)
        {
            foreach (var objectKey in objectEntries.AllKeys)
            {
                string objectInfo = objectKey.Substring(7); // Substring("Object_".Length);
                int firstIX = objectInfo.IndexOf('_');
                if (firstIX < 0)
                    throw new InvalidDataException("Invalid field data on binary content");
                string containerID = objectInfo.Substring(0, firstIX);
                string containerField = objectInfo.Substring(firstIX + 1);
                string objectIDCommaSeparated = objectEntries[objectKey] ?? "";
                string[] objectIDList = objectIDCommaSeparated.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                rootObject.SetObjectContent(containerID, containerField, objectIDList);
            }
        }

        public static void SetBinaryContent(IInformationObject rootObject, NameValueCollection fileEntries, HttpFileCollection fileContent, IContainerOwner containerOwner)
        {
            foreach (string fileKey in fileEntries.AllKeys)
            {
                HttpPostedFile postedFile = null;
                if (fileContent.AllKeys.Contains(fileKey))
                {
                    postedFile = fileContent[fileKey];
                }
                //if (String.IsNullOrWhiteSpace(postedFile.FileName))
                //    continue;
                string contentInfo = fileKey.Substring(5); // Substring("File_".Length);
                SetBinaryContent(containerOwner, contentInfo, rootObject, postedFile);
            }
        }

        public static void SetFieldValues(IInformationObject rootObject, NameValueCollection fieldEntries)
        {
            NameValueCollection internalObjectFixedEntries = new NameValueCollection();
            foreach (string key in fieldEntries.AllKeys)
            {
                string fieldValue = fieldEntries[key];
                if (key.Contains("___"))
                {
                    int underscoreIndex = key.IndexOf('_');
                    string containingObjectID = key.Substring(0, underscoreIndex);
                    List<IInformationObject> foundObjects= new List<IInformationObject>();
                    rootObject.FindObjectsFromTree(foundObjects, iObj => iObj.ID == containingObjectID, false);
                    if(foundObjects.Count == 0)
                        throw new InvalidDataException("Containing object with ID not found: " + containingObjectID);
                    var containingRootObject = foundObjects[0];
                    string chainedPropertyName = key.Substring(underscoreIndex + 1);
                    object actualContainingObject;
                    string actualPropertyName;
                    InitializeChainAndReturnPropertyOwningObject(containingRootObject, chainedPropertyName,
                                                                 out actualContainingObject, out actualPropertyName);
                    IInformationObject actualIObj = (IInformationObject) actualContainingObject;
                    string actualKey = actualIObj.ID + "_" + actualPropertyName;
                    internalObjectFixedEntries.Add(actualKey, fieldValue);
                }
                else
                {
                    internalObjectFixedEntries.Add(key, fieldValue);
                }
            }
            rootObject.SetValuesToObjects(internalObjectFixedEntries);
        }

        private static void retrieveDataSourceInfo(string sourceInfo, out string relativeLocation, out string oldETag)
        {
            string[] infoParts = sourceInfo.Split(':');
            relativeLocation = infoParts[0];
            oldETag = infoParts[1];
        }

        public static void SetBinaryContent(IContainerOwner containerOwner, string contentInfo, IInformationObject rootObject,
                                HttpPostedFile postedFile)
        {
            int firstIX = contentInfo.IndexOf('_');
            if (firstIX < 0)
                throw new InvalidDataException("Invalid field data on binary content");
            string containerID = contentInfo.Substring(0, firstIX);
            string containerField = contentInfo.Substring(firstIX + 1);
            rootObject.SetMediaContent(containerOwner, containerID, containerField, postedFile);
        }

        public static void InitializeChainAndReturnPropertyOwningObject(IInformationObject createdObject, string objectProp, out object actualContainingObject, out string fieldPropertyName)
        {
            actualContainingObject = null;
            fieldPropertyName = null;
            if (objectProp.Contains("___") == false)
                return;
            string[] objectChain = objectProp.Split(new[] { "___" }, StringSplitOptions.None);
            fieldPropertyName = objectChain[objectChain.Length - 1];
            Stack<string> objectChainStack = new Stack<string>(objectChain.Reverse().Skip(1));
            actualContainingObject = createdObject;
            while (objectChainStack.Count > 0)
            {
                Type currType = actualContainingObject.GetType();
                string currObjectProp = objectChainStack.Pop();
                PropertyInfo prop = currType.GetProperty(currObjectProp);
                if (prop == null)
                    throw new InvalidDataException("Property not found by name: " + currObjectProp);
                var currPropValue = prop.GetValue(actualContainingObject, null);
                if (currPropValue == null)
                {
                    var currPropType = prop.PropertyType;
                    currPropValue = Activator.CreateInstance(currPropType);
                    prop.SetValue(actualContainingObject, currPropValue, null);
                }
                actualContainingObject = currPropValue;
            }
        }
    }
}