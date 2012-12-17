using System;

namespace TheBall
{
    public enum StackContextItemType
    {
        Object = 1,
        Root = 2,
        CollectionUnfiltered = 3,
        CollectionFiltered = 4,
    }

    public class StackContextItem
    {
        public StackContextItem(object content, StackContextItem parent, Type itemType, string memberName, StackContextItemType contextitemType, string rootName = null)
        {
            bool isRoot = contextitemType == StackContextItemType.Root;
            bool isCollection = contextitemType == StackContextItemType.CollectionFiltered || contextitemType == StackContextItemType.CollectionUnfiltered;
                
            _content = content;
            Parent = parent;
            //TypeName = typeName;
            ItemType = itemType;
            MemberName = memberName;
            IsRoot = isRoot;
            IsCollection = isCollection;
            if(isCollection && content.GetType() != typeof(string))
            {
                bool isFilteredCollection = contextitemType == StackContextItemType.CollectionFiltered;
                dynamic dyn = content;
                if (isFilteredCollection)
                {
                    CurrArray = dyn.GetIDSelectedArray();
                } else
                {
                    dynamic collContent = dyn.CollectionContent;
                    CurrArray = collContent.ToArray();
                }
                ItemType = CurrArray.GetType().GetElementType();
            }
            if (isCollection)
                CollectionContainer = content;
            RootName = rootName;
        }

        public bool IsNotFullyProcessed
        {
            get
            {
                bool currArrayUnProcessed = CurrArray != null && CurrArray.Length > CurrCollectionItem;
                if (currArrayUnProcessed)
                    return true;
                //if (Parent != null)
                //    return Parent.IsNotFullyProcessed;
                return false;
            }
        }

        /// <summary>
        /// Non-null when collection, storing the pure collection object
        /// </summary>
        public object CollectionContainer { get; set; }

        /// <summary>
        /// Current content; reflecting also the collection iteration with accessing index
        /// </summary>
        public object CurrContent
        {
            get
            {
                if (CurrArray != null)
                    return CurrArray.GetValue(CurrCollectionItem);
                return _content;
            }
        }

        public string RootName;
        public bool IsInvalidContext = false;
        private object _content;
        //public string TypeName;
        public Type ItemType;
        public string MemberName;
        public bool IsRoot = false;
        public bool IsCollection = false;
        public int CurrCollectionItem = 0;
        public Array CurrArray = null;
        public StackContextItem Parent;

        public StackContextItem GetContextRoot()
        {
            if (IsRoot)
                return this;
            return Parent.GetContextRoot();
        }
    }
}