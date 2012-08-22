using System;

namespace TheBall
{
    public class ErrorItem
    {
        public readonly Exception ErrorException;
        public readonly StackContextItem CurrentStackItem;
        public string CurrentLine;
        public string CurrentContextName;
        public string ErrorMessage;

        public ErrorItem()
        {
            
        }

        public ErrorItem(Exception errorException, StackContextItem currentStackItem, string currentLine)
        {
            ErrorException = errorException;
            CurrentStackItem = currentStackItem;
            CurrentLine = currentLine;
        }
    }
}