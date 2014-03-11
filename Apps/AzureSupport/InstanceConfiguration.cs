using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;

namespace TheBall
{
    public static class InstanceConfiguration
    {
        public static readonly string VersionString = "v1.0.2b";
        public static readonly string AWSAccessKey;
        public static readonly string AWSSecretKey;
        public static readonly string EmailFromAddress;
        public static readonly string EmailValidationSubjectFormat;
        public static readonly string EmailValidationMessageFormat;
        public static readonly string EmailDeviceJoinSubjectFormat;
        public static readonly string EmailDeviceJoinMessageFormat;
        public static readonly string EmailGroupJoinSubjectFormat;
        public static readonly string EmailGroupJoinMessageFormat;
        public static readonly string EmailInputJoinSubjectFormat;
        public static readonly string EmailInputJoinMessageFormat;
        public static readonly string EmailOutputJoinSubjectFormat;
        public static readonly string EmailOutputJoinMessageFormat;
        public static readonly string EmailValidationURLWithoutID;
        public static readonly string AzureStorageConnectionString;
        public static readonly string WorkerActiveContainerName;
        public static readonly string RedirectFromFolderFileName;
        public static readonly string[] DefaultAccountTemplateList;
        public static readonly string AccountDefaultRedirect;
        public static readonly string[] DefaultGroupTemplateList;
        public static readonly string GroupDefaultRedirect;
        public static readonly string AzureStorageKey;
        public static readonly string AzureAccountName;
        public static readonly string AdminGroupID;

        // Infrastructure content/fields
        public static readonly string CloudDriveContainerName;
        public static readonly int CloudDriveTotalCacheSizeInMB;
        public static readonly string LocalStorageResourceName;
        public static readonly int HARDCODED_StatusUpdateExpireSeconds = 300;

        static InstanceConfiguration()
        {
            # region Infrastructure

            LocalStorageResourceName = "LocalCache";
            CloudDriveContainerName = "clouddrives";
            CloudDriveTotalCacheSizeInMB = 2048;
            #endregion
            #region Data storage

            const string ConnStrFileName = @"C:\users\kalle\work\ConnectionStringStorage\caloomdemoconnstr.txt";
            if (File.Exists(ConnStrFileName))
                AzureStorageConnectionString = File.ReadAllText(ConnStrFileName);
            else
                AzureStorageConnectionString = CloudConfigurationManager.GetSetting("DataConnectionString");
            var connStrSplits = AzureStorageConnectionString.Split(new[] {";AccountKey="}, StringSplitOptions.None);
            AzureStorageKey = connStrSplits[1];
            connStrSplits = connStrSplits[0].Split(new[] {";AccountName="}, StringSplitOptions.None);
            AzureAccountName = connStrSplits[1];
            WorkerActiveContainerName = CloudConfigurationManager.GetSetting("WorkerActiveContainerName");
            #endregion

            #region System Level

            RedirectFromFolderFileName = CloudConfigurationManager.GetSetting("RedirectFromFolderFileName");
            AccountDefaultRedirect = CloudConfigurationManager.GetSetting("AccountDefaultRedirect");
            GroupDefaultRedirect = CloudConfigurationManager.GetSetting("GroupDefaultRedirect");
            AdminGroupID = CloudConfigurationManager.GetSetting("AdminGroupID");

            #endregion

            #region Email

            try
            {
                const string SecretFileName = @"C:\users\kalle\work\ConnectionStringStorage\amazonses.txt";
                string configString;
                if (File.Exists(SecretFileName))
                    configString = File.ReadAllText(SecretFileName);
                else
                    configString = CloudConfigurationManager.GetSetting("AmazonSESAccessInfo");
                string[] strValues = configString.Split(';');
                AWSAccessKey = strValues[0];
                AWSSecretKey = strValues[1];
            }
            catch // Neutral credentials - will revert to queue put when message send is failing at EmailSupport
            {
                AWSAccessKey = "";
                AWSSecretKey = "";
            }

            EmailFromAddress = CloudConfigurationManager.GetSetting("EmailFromAddress"); // "no-reply-theball@msunit.citrus.fi"
            EmailDeviceJoinMessageFormat = CloudConfigurationManager.GetSetting("EmailDeviceJoinMessageFormat");
            EmailDeviceJoinSubjectFormat = CloudConfigurationManager.GetSetting("EmailDeviceJoinSubjectFormat");
            EmailInputJoinSubjectFormat = CloudConfigurationManager.GetSetting("EmailInputJoinSubjectFormat");
            EmailInputJoinMessageFormat = CloudConfigurationManager.GetSetting("EmailInputJoinMessageFormat");
            EmailOutputJoinSubjectFormat = CloudConfigurationManager.GetSetting("EmailOutputJoinSubjectFormat");
            EmailOutputJoinMessageFormat = CloudConfigurationManager.GetSetting("EmailOutputJoinMessageFormat");
            EmailValidationSubjectFormat = CloudConfigurationManager.GetSetting("EmailValidationSubjectFormat");
            EmailValidationMessageFormat = CloudConfigurationManager.GetSetting("EmailValidationMessageFormat");
            EmailGroupJoinSubjectFormat = CloudConfigurationManager.GetSetting("EmailGroupJoinSubjectFormat");
            EmailGroupJoinMessageFormat = CloudConfigurationManager.GetSetting("EmailGroupJoinMessageFormat");
            EmailValidationURLWithoutID = CloudConfigurationManager.GetSetting("EmailValidationURLWithoutID");
#if hardcoded
            EmailDeviceJoinMessageFormat = @"Your confirmation is required to trust the following device '{0}' to be joined to trust within {1} ID {2}. 

Click the following link to confirm this action:
{3}";
            EmailInputJoinMessageFormat = @"Your confirmation is required to allow the following information source '{0}' to be fetched within {1} ID {2}. 

Click the following link to confirm this action:
{3}";

            EmailValidationMessageFormat = @"Welcome to The Open Innovation Platform!

You have just joined the collaboration platform by Aalto Global Impact. Your email address '{0}' has been registered on the OIP system. Before you start your collaboration we simply need to confirm that you did register your email. Please follow the link below during which you might be redirected to perform the authentication on OIP.

Use the following link to complete your registration (the link is valid for 30 minutes after which you need to resend the validation):
{1}

Wishing you all the best from OIP team!";

            EmailGroupJoinMessageFormat = @"You have been invited to join in the collaboration platform by Aalto Global Impact to collaborate in the group: {0}. 

Use the following link to accept the invitation and join the group:
{1}

The link is valid for 14 days, after which you need to request new invitation.";
#endif

            #endregion

            var defaultAccountTemplateList = CloudConfigurationManager.GetSetting("DefaultAccountTemplateList");
            if(defaultAccountTemplateList != null)
                DefaultAccountTemplateList = defaultAccountTemplateList.Split(',');
            var defaultGroupTemplateList = CloudConfigurationManager.GetSetting("DefaultGroupTemplateList");
            if(defaultGroupTemplateList != null)
                DefaultGroupTemplateList = defaultGroupTemplateList.Split(',');


        }
    }
}
