using System;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace ContentSyncTool
{
    class Options
    {
        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }

        public Options()
        {
            CreateConnectionVerb = new CreateConnectionSubOptions();
            DeleteConnectionVerb = new DeleteConnectionSubOptions();
            ListConnectionVerb = new EmptySubOptions();
            SelfTestVerb = new EmptySubOptions();
        }

        [VerbOption("createConnection", HelpText = "Create connection")]
        public CreateConnectionSubOptions CreateConnectionVerb { get; set; }

        [VerbOption("deleteConnection", HelpText = "Delete connection")]
        public DeleteConnectionSubOptions DeleteConnectionVerb { get; set; }

        [VerbOption("listConnections", HelpText = "List current connections")]
        public EmptySubOptions ListConnectionVerb { get; set; }

        [VerbOption("selfTest", HelpText = "Self test tool and executing environment")]
        public EmptySubOptions SelfTestVerb { get; set; }

    }

    class DeleteConnectionSubOptions : NamedConnectionSubOptions
    {
        
    }

    class ListConnectionSubOptions
    {
        
    }

    class EmptySubOptions
    {
        
    }

    class CreateConnectionSubOptions : NamedConnectionSubOptions
    {
        [Option('g', "groupID", HelpText = "Group ID to establish connection against", Required = true)]
        public string GroupID { get; set; }

        [Option('h', "hostName", HelpText = "Host name of the Ball instance to connect to", Required = true)]
        public string HostName { get; set; }
    }

    abstract class NamedConnectionSubOptions
    {
        [Option('n', "connectionName", HelpText = "Connection name used to identify/shortcut connection", Required = true)]
        public string ConnectionName { get; set; }

        // Common auto-calculated values
        protected virtual void validateSelf()
        {
            
        }
        public void Validate()
        {
            if(string.IsNullOrEmpty(ConnectionName))
                throw new ArgumentException("Connection name is required");
            /*
            DirectoryInfo dirInfo = new DirectoryInfo(CatalogueRepositoryRoot);
            if(dirInfo.Exists == false)
                throw new ArgumentException("Invalid CatalogueRepositoryRoot value (directory not found): " + CatalogueRepositoryRoot);
            validateSelf();
          * */
        }
    }

}

