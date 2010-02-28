using System.Linq;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using DotNetServerApi;
using System.Reflection;
using System.IO;

namespace DotNetServer
{
    public class Program
    {
        [SecurityPermission(SecurityAction.LinkDemand, ControlDomainPolicy = true)]
        static void Initialize(string[] args)
        {
            DisplayInfo();

            bool exit = false;
            DisplayHelp();
            do
            {
                Console.WriteLine();
                Console.Write("> ");
                string commandLine = Console.ReadLine();

                var splitted = commandLine.Split(' ');
                string command = splitted[0];
                string[] commandArgs = new string[0];
                if (splitted.Length > 1)
                {
                    commandArgs = new string[splitted.Length - 1];
                    var all = splitted.ToList();
                    all.RemoveAt(0);
                    commandArgs = all.ToArray();
                }

                exit = ProcessCommand(command, commandArgs);

            } while (!exit);


            Console.WriteLine("End, Press a key to exit.");
            Console.ReadKey();
        }

        private static void DisplayInfo()
        {
            Console.WriteLine("OSVersion: {0}", Environment.OSVersion);
            Console.WriteLine("MachineName: {0}", Environment.MachineName);
            Console.WriteLine(".Net Version: {0}", Environment.Version);
            Console.WriteLine("CurrentDirectory: {0}", Environment.CurrentDirectory);
            Console.WriteLine("RelativeSearchPath: {0}", AppDomain.CurrentDomain.RelativeSearchPath);
            Console.WriteLine("ConfigurationFile: {0}", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            Console.WriteLine("PrivateBinPath: {0}", AppDomain.CurrentDomain.SetupInformation.PrivateBinPath);
            Console.WriteLine("PrivateBinPathProbe: {0}", AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe);
            Console.WriteLine("ApplicationBase: {0}", AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
        }

        static void Main(string[] args)
        {
            string path = Path.GetFullPath("Libs\\DotNetServerApi.dll");
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            setup.ApplicationBase = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //ApplicationIdentity identity = new ApplicationIdentity(AssemblyName.GetAssemblyName(path).Name);
            //setup.ActivationArguments = new ActivationArguments(identity, new string[] { p });
            setup.ApplicationName = "Main";

            setup.AppDomainInitializer = new AppDomainInitializer(Initialize);
            setup.AppDomainInitializerArguments = args;

            List<string> paths = new List<string>();
            paths.Add(Path.GetDirectoryName(path));
            paths.AddRange(Directory.GetDirectories(Path.Combine(setup.ApplicationBase, "Libs")));
            setup.PrivateBinPath = string.Join(";", paths.ToArray());

            var dom = AppDomain.CreateDomain(path, AppDomain.CurrentDomain.Evidence, setup);

            dom.ProcessExit += new EventHandler(Main_ProcessExit);            
        }

        private static void Main_ProcessExit(object sender, EventArgs e)
        {
            
        }

        private static bool ProcessCommand(string command, string[] args)
        {
            switch (command)
            {
                case "start":
                    if (args == null || args.Length != 1)
                        DisplayHelp();
                    else
                    {
                        try
                        {
                            BundleController.Start(args[0]);
                            Console.WriteLine("Started");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }                        
                    }
                    break;
                case "stop":
                    if (args == null || args.Length != 1)
                        DisplayHelp();
                    else
                    {
                        try
                        {
                            BundleController.Stop(args[0]);
                            Console.WriteLine("Stopped");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    break;
                case "list":
                    foreach(var item in BundleController.ApplicationLoaded)
                        Console.WriteLine(string.Format("{0} => {1}",item.Key, item.Value.AppDomain.SetupInformation.ApplicationName));
                    break;
                case "info":
                    DisplayInfo();
                    break;
                case "exit":
                    return true;
                default:
                    DisplayHelp();
                    break;
            }
            return false;
        }              

        private static void DisplayHelp()
        {
            Console.WriteLine("help : display this help");
            Console.WriteLine("start assemblyname : start an application");
            Console.WriteLine("stop assemblyname : stop an application");
            Console.WriteLine("install assemblyname [folder path|archive path] : install an application");
            Console.WriteLine("exit : exit the application");
        }        
    }
}
