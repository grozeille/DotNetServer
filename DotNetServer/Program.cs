using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using DotNetServerApi;

namespace DotNetServer
{
    public class Program
    {
        /// <summary>
        /// Fake entry point, wich loads a new AppDomain to start the "Initialize" entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            setup.ApplicationBase = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            setup.ApplicationName = "Main";
            setup.AppDomainInitializer = new AppDomainInitializer(Initialize);
            setup.AppDomainInitializerArguments = args;

            // I have to create a new AppDomain to change the search path of assemblies
            List<string> paths = new List<string>();

            // add the main lib path
            string dotNetServerLibPath = Path.GetFullPath("Libs\\DotNetServerApi.dll");
            paths.Add(Path.GetDirectoryName(dotNetServerLibPath));

            // add all additional libs path (in libs folders)
            paths.AddRange(Directory.GetDirectories(Path.Combine(setup.ApplicationBase, "Libs")));
            setup.PrivateBinPath = string.Join(";", paths.ToArray());

            var dom = AppDomain.CreateDomain(dotNetServerLibPath, AppDomain.CurrentDomain.Evidence, setup);
        }

        /// <summary>
        /// The true entry point
        /// </summary>
        /// <param name="args"></param>
        [SecurityPermission(SecurityAction.LinkDemand, ControlDomainPolicy = true)]
        static void Initialize(string[] args)
        {
            // create a new BundleContext, which is a "cache" of all applications
            BundleContext context = new BundleContext();

            // load all installed Bundles
            BundleController.RefreshAllBundles(context);
            
            // start the main loop
            DisplayInfo();

            bool exit = false;
            DisplayHelp();
            do
            {
                Console.WriteLine();
                Console.Write("> ");
                string commandLine = Console.ReadLine();

                if (commandLine != null)
                {
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

                    exit = ProcessCommand(context, command, commandArgs);
                }

            } while (!exit);


            Console.WriteLine("End, Press a key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Display information about the server
        /// </summary>
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

        private static bool ProcessCommand(BundleContext context, string command, string[] args)
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
                            BundleController.Start(context, args[0]);
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
                            BundleController.Stop(context, args[0]);
                            Console.WriteLine("Stopped");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    break;
                case "list":
                    foreach (var item in context.BundleLoaded)
                        Console.WriteLine(string.Format("{0}:{1}\t[{2}]", item.Value.Name, item.Value.Version, item.Value.State));
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

        /// <summary>
        /// Display all possible commands
        /// </summary>
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
