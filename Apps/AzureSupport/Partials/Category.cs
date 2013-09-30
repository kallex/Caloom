using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace AaltoGlobalImpact.OIP
{
    partial class CategoryCollection : IAdditionalFormatProvider
    {
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }
    }

    partial class Category : IBeforeStoreHandler
    {
        public void PerformBeforeStoreUpdate()
        {
            ValidateNonCircularParentLinks();
            if (Title == null)
                Title = "";
            char[] arr = Title.ToCharArray();
            arr = Array.FindAll<char>(arr, c => char.IsLetterOrDigit(c) && (int)c < 128);
            CategoryName = "cat" + new string(arr);
        }

        private void ValidateNonCircularParentLinks()
        {
            Dictionary<string, bool> traversedIDDict = new Dictionary<string, bool>();
            var currCat = this;
            traversedIDDict.Add(currCat.ID, true);
            while (currCat.ParentCategory != null)
            {
                string parentID = currCat.ParentCategory.ID;
                if(traversedIDDict.ContainsKey(parentID))
                    throw new InvalidDataException("Circular reference in parent categories - aborting save");
                traversedIDDict.Add(parentID, true);
                currCat = currCat.ParentCategory;
            }
        }
    }

    partial class Category : IAdditionalFormatProvider
    {
        AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore(string masterBlobETag)
        {
            return this.GetFormattedContentToStore(masterBlobETag, AdditionalFormatSupport.WebUIFormatExtensions);
        }

        string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
        {
            return this.GetFormatExtensions(AdditionalFormatSupport.WebUIFormatExtensions);
        }

    }

}