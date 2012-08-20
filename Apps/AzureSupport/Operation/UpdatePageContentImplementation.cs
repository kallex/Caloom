using System;
using System.IO;

namespace AaltoGlobalImpact.OIP
{
    public static class UpdatePageContentImplementation
    {
        public static void ExecuteMethod_UpdatePage()
        {
            throw new NotImplementedException();
        }
    }

    public partial class TBReferenceEvent
    {
        public void ParsePropertyValue(string propertyName, string value)
        {
            switch (propertyName)
            {
                case "Title":
                    Title = value;
                    break;
                case "Description":
                    Description = value;
                    break;
                case "AttendeeCount":
                    AttendeeCount = long.Parse(value);
                    break;
                case "EnoughToAttend":
                    EnoughToAttend = bool.Parse(value);
                    break;
                case "DueTime":
                    DueTime = DateTime.Parse(value);
                    break;
                default:
                    throw new InvalidDataException("Property name not found: " + propertyName);
            }
        }

    }

}