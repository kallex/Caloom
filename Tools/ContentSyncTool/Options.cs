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
            ListConnectionsVerb = new ListConnectionsSubOptions();
            SelfTestVerb = new SelfTestSubOptions();
            SyncVerb = new SyncFolderSubOptions();
            AddSyncFolder = new AddSyncFolderSubOptions();
            RemoveSyncFolder = new RemoveSyncFolderSubOptions();
        }

        [VerbOption("createConnection", HelpText = "Create connection")]
        public CreateConnectionSubOptions CreateConnectionVerb { get; set; }

        [VerbOption("deleteConnection", HelpText = "Delete connection")]
        public DeleteConnectionSubOptions DeleteConnectionVerb { get; set; }

        [VerbOption("listConnections", HelpText = "List current connections")]
        public ListConnectionsSubOptions ListConnectionsVerb { get; set; }

        [VerbOption("selfTest", HelpText = "Self test tool and executing environment")]
        public SelfTestSubOptions SelfTestVerb { get; set; }

        [VerbOption("sync", HelpText = "Sync connection folder")]
        public SyncFolderSubOptions SyncVerb { get; set; }

        [VerbOption("addSyncFolder", HelpText = "Add sync folder")]
        public AddSyncFolderSubOptions AddSyncFolder { get; set; }

        [VerbOption("removeSyncFolder", HelpText = "Remove sync folder")]
        public RemoveSyncFolderSubOptions RemoveSyncFolder { get; set; }

    }

    class SyncFolderSubOptions : ConnectionSyncFolderSubOptions
    {
        public override void ExecuteCommand(string verb)
        {
            CommandImplementation.syncFolder(this);
        }
    }

    class RemoveSyncFolderSubOptions : ConnectionSyncFolderSubOptions
    {
        public override void ExecuteCommand(string verb)
        {
            CommandImplementation.removeSyncFolder(this);
        }
    }

    class AddSyncFolderSubOptions : ConnectionSyncFolderSubOptions
    {
        [Option('t', "syncType", HelpText = "Sync type: DEV, wwwsite", Required = true)]
        public string SyncType { get; set; }

        [Option('l', "localFullPath", HelpText = "Local full path to folder", Required = true)]
        public string LocalFullPath { get; set; }

        [Option('r', "remoteFolder", HelpText = "Remote folder name", Required = true)]
        public string RemoteFolder { get; set; }

        [Option('d', "syncDirection", HelpText = "Sync direction UP or DOWN", Required = true)]
        public string SyncDirection { get; set; }



        public override void ExecuteCommand(string verb)
        {
            CommandImplementation.addSyncFolder(this);
        }
    }

    abstract class ConnectionSyncFolderSubOptions : NamedConnectionSubOptions
    {
        [Option('s', "syncName", HelpText = "syncName", Required = true)]
        public string SyncName { get; set; }
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

    class SelfTestSubOptions : ICommandExecution
    {
        public void ExecuteCommand(string verb)
        {
            CommandImplementation.selfTest(this);
        }
    }

    class ListConnectionsSubOptions : ICommandExecution
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
        [Option('c', "connectionName", HelpText = "Connection name used to identify/shortcut connection", Required = true)]
        public string ConnectionName { get; set; }

        public abstract void ExecuteCommand(string verb);
    }

}

