using System;

namespace TheBall
{
    public class StackContextItem
    {
        public StackContextItem(object content, Type itemType, string memberName, bool isRoot, bool isCollection)
        {
            Content = content;
            //TypeName = typeName;
            ItemType = itemType;
            MemberName = memberName;
            IsRoot = isRoot;
            IsCollection = isCollection;
        }

        public bool IsInvalidContext = false;
        public object Content;
        //public string TypeName;
        public Type ItemType;
        public string MemberName;
        public bool IsRoot = false;
        public bool IsCollection = false;
        public int CurrCollectionItem = 0;
    }
}