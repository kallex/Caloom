using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;

namespace TheBall
{
    public static class InstanceConfiguration
    {
        public static readonly string EmailMessageFormat;

        static InstanceConfiguration()
        {
            EmailMessageFormat = CloudConfigurationManager.GetSetting("EmailMessageFormat");

        }
    }
}
