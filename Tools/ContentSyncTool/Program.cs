using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using TheBall.Support.DeviceClient;

namespace ContentSyncTool
{
    class Program
    {
        private static Options options = new Options();
        private static void Main(string[] args)
        {
            // Debugger.Launch();
            bool success = CommandLine.Parser.Default.ParseArguments(args, options, OnVerbCommand);
            if (!success)
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

        }

        private static void OnVerbCommand(string verb, object verbSubOptions)
        {
            if (verbSubOptions == null)
                return;
            try
            {
                // Debugger.Launch();
                UserSettings.GetCurrentSettings();
                ICommandExecution executionSupport = verbSubOptions as ICommandExecution;
                if (executionSupport != null)
                {
                    ClientExecute.ExecuteWithSettings(executionAction: userSettings =>
                    {
                        executionSupport.ExecuteCommand(verb);
                    }, exceptionHandling: (Exception ex) =>
                    {
                        Console.WriteLine("Error: " + ex.ToString());
                    });
                }
                else // Custom verb implementation
                {
                    switch (verb)
                    {
                        default:
                            throw new ArgumentException("Not implemented verb: " + verb);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
        }

    }
}
