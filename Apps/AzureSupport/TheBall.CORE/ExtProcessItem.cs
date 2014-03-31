using System.Collections.Generic;
using System.Linq;

namespace TheBall.CORE
{
    public static class ExtProcessItem
    {
        public static string GetInputValue(this ProcessItem processItem, string itemFullType)
        {
            return processItem.Inputs.GetSemanticItemValue(itemFullType);
        }

        public static string GetOutputValue(this ProcessItem processItem, string itemFullType)
        {
            return processItem.Outputs.GetSemanticItemValue(itemFullType);
        }

        public static void SetInputValue(this ProcessItem processItem, string itemFullType, string itemValue)
        {
            processItem.Inputs.SetSemanticItemValue(itemFullType, itemValue);
        }

        public static void SetOutputValue(this ProcessItem processItem, string itemFullType, string itemValue)
        {
            processItem.Outputs.SetSemanticItemValue(itemFullType, itemValue);
        }

        private static void SetSemanticItemValue(this List<SemanticInformationItem> semanticItems, string itemFullType, string itemValue)
        {
            var currItem = semanticItems.GetSemanticItem(itemFullType);
            if (currItem != null)
                currItem.ItemValue = itemValue;
            else
            {
                currItem = new SemanticInformationItem(itemFullType, itemValue);
                semanticItems.SetSemanticItem(currItem);
            }
        }

        private static void SetSemanticItem(this List<SemanticInformationItem> semanticItems, SemanticInformationItem semanticInformationItem)
        {
            string itemFullTypeToSet = semanticInformationItem.ItemFullType;
            var currItem = semanticItems.GetSemanticItem(itemFullTypeToSet);
            if (currItem != null)
                semanticItems.Remove(currItem);
            semanticItems.Add(semanticInformationItem);
        }

        private static SemanticInformationItem GetSemanticItem(this List<SemanticInformationItem> semanticItems, string itemFullType)
        {
            var itemInput = semanticItems.FirstOrDefault(inp => inp.ItemFullType == itemFullType);
            return itemInput;
        }

        private static string GetSemanticItemValue(this List<SemanticInformationItem> semanticItems, string itemFullType)
        {
            var semanticItem = semanticItems.GetSemanticItem(itemFullType);
            if (semanticItem == null)
                return null;
            return semanticItem.ItemValue;
        }

    }
}