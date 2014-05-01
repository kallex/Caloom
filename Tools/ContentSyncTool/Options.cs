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
            ListConnectionsVerb = new EmptySubOptions();
            SelfTestVerb = new EmptySubOptions();
            SetConnectionRootLocationsVerb = new ConnectionRootLocationSubOptions();
            SetConnectionSyncFoldersVerb = new ConnectionSyncFoldersSubOptions();
            UpSyncVerb = new ConnectionUpSyncSubOptions();
            DownSyncVerb = new ConnectionDownSyncSubOptions();
        }

        [VerbOption("createConnection", HelpText = "Create connection")]
        public CreateConnectionSubOptions CreateConnectionVerb { get; set; }

        [VerbOption("deleteConnection", HelpText = "Delete connection")]
        public DeleteConnectionSubOptions DeleteConnectionVerb { get; set; }

        [VerbOption("listConnections", HelpText = "List current connections")]
        public EmptySubOptions ListConnectionsVerb { get; set; }

        [VerbOption("selfTest", HelpText = "Self test tool and executing environment")]
        public EmptySubOptions SelfTestVerb { get; set; }

        [VerbOption("setConnectionRootLocations", HelpText = "Set connection sync root locations")]
        public ConnectionRootLocationSubOptions SetConnectionRootLocationsVerb { get; set; }

        [VerbOption("setConnectionSyncFolders", HelpText = "Set connection sync folders")]
        public ConnectionSyncFoldersSubOptions SetConnectionSyncFoldersVerb { get; set; }

        [VerbOption("upsync", HelpText = "Upload sync connection with predefined folders")]
        public ConnectionUpSyncSubOptions UpSyncVerb { get; set; }

        [VerbOption("downsync", HelpText = "Download sync connection with predefined folders")]
        public ConnectionDownSyncSubOptions DownSyncVerb { get; set; }
    }

    class ConnectionDownSyncSubOptions : NamedConnectionSubOptions
    {
        public override void ExecuteCommand(string verb)
        {
            CommandImplementation.downsync(this);
        }
    }

    class ConnectionUpSyncSubOptions : NamedConnectionSubOptions
    {
        public override void ExecuteCommand(string verb)
        {
            CommandImplementation.upsync(this);
        }
    }

    class ConnectionSyncFoldersSubOptions : NamedConnectionSubOptions
    {
        [Option('d', "downSyncFolders", HelpText = "Comma separated remote folders to sync to predefined down root", Required = false)]
        public string DownSyncFolders { get; set; }

        [Option('u', "upSyncFolders", HelpText = "Comma separated local folders to sync from predefined upsync root", Required = false)]
        public string UpSyncFolders { get; set; }

        public override void ExecuteCommand(string verb)
        {
            CommandImplementation.setConnectionSyncFolders(this);
        }
    }

    class ConnectionRootLocationSubOptions : NamedConnectionSubOptions
    {
        [Option('u', "upSyncRoot", HelpText = "Up sync root location", Required = false)]
        public string UpSyncRoot { get; set; }

        [Option('d', "downSyncRoot", HelpText = "Down sync root location", Required = false)]
        public string DownSyncRoot { get; set; }

        public void Validate()
        {
            if (String.IsNullOrEmpty(UpSyncRoot) == false)
            {
                if(Directory.Exists(UpSyncRoot) == false)
                    throw new ArgumentException("Invalid up sync root value (directory not found): " + UpSyncRoot);
            }
            if (String.IsNullOrEmpty(DownSyncRoot) == false)
            {
                if(Directory.Exists(DownSyncRoot) == false)
                    throw new ArgumentException("Invalid down sync root value (directory not found): " + DownSyncRoot);
            }
        }

        public override void ExecuteCommand(string verb)
        {
            CommandImplementation.setConnectionRootLocations(this);
        }
    }

    class DeleteConnectionSubOptions : NamedConnectionSubOptions
    {
        [Option('f', "force", HelpText = "Force deletion in case of remote deletion error", Required = false)]
        public bool Force { get; set; }

        public override void ExecuteCommand(string verb)
        {
            CommandImplementation.deleteConnection(this);
        }
    }

    internal interface ICommandExecution
    {
        void ExecuteCommand(string verb);
    }

    class EmptySubOptions : ICommandExecution
    {
        public void ExecuteCommand(string verb)
        {
            CommandImplementation.listConnections(this);
        }
    }

    class CreateConnectionSubOptions : NamedConnectionSubOptions
    {
        [Option('g', "groupID", HelpText = "Group ID to establish connection against", Required = true)]
        public string GroupID { get; set; }

        [Option('h', "hostName", HelpText = "Host name of the Ball instance to connect to", Required = true)]
        public string HostName { get; set; }

        public override void ExecuteCommand(string verb)
        {
            CommandImplementation.createConnection(this);
        }
    }

    abstract class NamedConnectionSubOptions : ICommandExecution
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
        }

        public abstract void ExecuteCommand(string verb);
    }

}

