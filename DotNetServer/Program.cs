using System.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Permissions;
using System.Text;
using DotNetServerApi;
using Mono.WebServer;
using System.Reflection;
using System.IO;
using System.Runtime.Hosting;
using System.Security.Policy;
using System.Diagnostics;
using System.Threading;

namespace DotNetServer
{
    public class Program
    {/*
        static private void Start()
        {            
            BundleBoostrap.Current.StartActivator();
        }

        static private void Stop()
        {
            BundleBoostrap.Current.StopActivator();
        }*/
        /*
        static void Initialize(string[] args)
        {
            var path = args[0];
            var name = AssemblyName.GetAssemblyName(path);
            var assembly = AppDomain.CurrentDomain.Load(name);
            var attribute = (BundleAttribute)assembly.GetCustomAttributes(typeof (BundleAttribute), false).FirstOrDefault();

            var activator = AppDomain.CurrentDomain.CreateInstance(name.Name, attribute.Activator.Name);
            ((IActivator)activator).Start();
        }
        */

        [SecurityPermission(SecurityAction.LinkDemand, ControlDomainPolicy = true)]
        static void Initialize(string[] args)
        {
            DisplayInfo();

            // ActivatorContext.Current.StartActivator();
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

            /*
            byte[] assembly = File.ReadAllBytes(p);

            var assemblyName = AssemblyName.GetAssemblyName(p);
            var reflection = Assembly.ReflectionOnlyLoad(assembly);

            var tt = CustomAttributeData.GetCustomAttributes(reflection);

            var attribute = (BundleAttribute)reflection.GetCustomAttributes(typeof(BundleAttribute), false).FirstOrDefault();
            var instance = dom.CreateInstance(assemblyName.Name, attribute.Activator.FullName);
            ((IActivator)instance.Unwrap()).Start();*/
            //LoadDependencies(dom, reflection);
            
            //bundle = dom.Load(assembly);
            //Console.WriteLine(bundle.GetModule("MyBundle"));

            //dom.DoCallBack(new CrossAppDomainDelegate(Do));





            //var bundle = Assembly.LoadFile(p);


            //var ctor = attribute.Activator.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0],
            //                                   new ParameterModifier[0]);
            //var activator = (IActivator)ctor.Invoke(new object[0]);
            //var handle = dom.CreateInstanceFrom("MyBundle", attribute.Activator.Name);
            //((IActivator)handle.Unwrap()).Start();
            /*dom.DoCallBack(() =>
                           {
                               var ctor = attribute.Activator.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0],
                                               new ParameterModifier[0]);
                               var activator = (IActivator)ctor.Invoke(new object[0]);
                               activator.Start();
                               Console.WriteLine("Mono.WebServer running. Press enter to exit...");
                               Console.ReadLine();
                               activator.Stop();
                           });
            //activator.Start();
            Console.WriteLine("Mono.WebServer running. Press enter to exit...");
            Console.ReadLine();
            //activator.Stop();
            //((IActivator)handle.Unwrap()).Stop();

            Console.WriteLine("End");
            Console.ReadKey();

            int Port = 1254;
            string path = "..\\..\\..\\TestWebApp";
            XSPWebSource websource = new XSPWebSource(IPAddress.Any, Port);
            ApplicationServer WebAppServer = new ApplicationServer(websource);
            //"[[hostname:]port:]VPath:realpath"
            string cmdLine = Port + ":/:" + path;
            WebAppServer.AddApplicationsFromCommandLine(cmdLine);
            WebAppServer.Start(true);
            Console.WriteLine("Mono.WebServer running. Press enter to exit...");
            Console.ReadLine();
            WebAppServer.Stop();*/
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
                            Start(args[0]);
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
                            Stop(args[0]);
                            Console.WriteLine("Stopped");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    break;
                case "list":
                    foreach(var item in applicationLoaded)
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

        private static IDictionary<string, BundleInfo> applicationLoaded = new Dictionary<string, BundleInfo>();

        private static void Stop(string assemblyName)
        {
            if (!applicationLoaded.ContainsKey(assemblyName))
            {
                Console.WriteLine("Application {0} not loaded", assemblyName);
                return;
            }

            //applicationLoaded[assemblyName].DoCallBack(Stop);
            applicationLoaded[assemblyName].Boostrap.StopActivator();
            AppDomain.Unload(applicationLoaded[assemblyName].AppDomain);
            applicationLoaded.Remove(assemblyName);
        }

        private static void Start(string assemblyName)
        {
            if (applicationLoaded.ContainsKey(assemblyName))
            {
                Console.WriteLine("Application {0} already loaded", assemblyName);
                return;
            }

            string path = Path.GetFullPath(string.Format("Bundles\\{0}\\{0}.dll", assemblyName));
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            setup.ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ApplicationIdentity identity = new ApplicationIdentity(AssemblyName.GetAssemblyName(path).Name);
            //setup.ActivationArguments = new ActivationArguments(identity, new string[] { p });
            setup.ApplicationName = path;

            setup.AppDomainInitializer = null;
            //setup.AppDomainInitializer = new AppDomainInitializer(Initialize);
            //setup.AppDomainInitializerArguments = new string[]{ path };

            List<string> paths = new List<string>();
            paths.Add(Path.GetDirectoryName(path));
            paths.AddRange(Directory.GetDirectories(Path.Combine(setup.ApplicationBase, "Libs")));
            setup.PrivateBinPath = string.Join(";", paths.ToArray());

            var dom = AppDomain.CreateDomain(path, AppDomain.CurrentDomain.Evidence, setup);
            
            //dom.AssemblyResolve += new ResolveEventHandler(dom_AssemblyResolve);
            //dom.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(dom_AssemblyResolve);

            //Console.WriteLine(string.Join(Environment.NewLine, dom.GetAssemblies().Select(a => a.FullName).ToArray()));
            /*
            var tmp = dom.CreateInstance(AssemblyName.GetAssemblyName(p).Name, "MyBundle.MyActivator");
            ((IActivator) tmp.Unwrap()).Start();*/
            dom.ProcessExit += new EventHandler(dom_ProcessExit);
            dom.DomainUnload += new EventHandler(dom_DomainUnload);
            dom.UnhandledException += new UnhandledExceptionEventHandler(dom_UnhandledException);

            BundleController boot = (BundleController)dom.CreateInstanceAndUnwrap(typeof (BundleController).Assembly.FullName, typeof (BundleController).FullName);

            applicationLoaded.Add(assemblyName, new BundleInfo{ AppDomain = dom, Boostrap = boot});

            boot.StartActivator();
            
            //dom.DoCallBack(Start);            
        }

        static void dom_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }

        static void dom_DomainUnload(object sender, EventArgs e)
        {
            var unload = ((AppDomain) sender);
        }

        static void dom_ProcessExit(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("help : display this help");
            Console.WriteLine("start assemblyname : start an application");
            Console.WriteLine("stop assemblyname : stop an application");
            Console.WriteLine("install assemblyname [folder path|archive path] : install an application");
            Console.WriteLine("exit : exit the application");
        }
        /*
        private static void LoadDependencies(AppDomain dom, Assembly reflection)
        {
            foreach(var assembly in reflection.GetReferencedAssemblies())
            {
                if(!dom.GetAssemblies().Where(a => a.FullName.Equals(assembly)).Any())
                {
                    try
                    {
                        string fileName = FindAssembly(dom, assembly.Name);
                        var reflection2 = Assembly.ReflectionOnlyLoadFrom(fileName);
                        LoadDependencies(dom, reflection2);
                    }
                    catch (FileNotFoundException)
                    {
                        dom.Load(assembly);
                    }                    
                }
            }

            var loaded = File.ReadAllBytes(reflection.Location);
            dom.Load(loaded);
        }

        private static string FindAssembly(AppDomain domain, string name)
        {
            if (File.Exists(Path.Combine(domain.BaseDirectory, name+".dll")))
                return Path.Combine(domain.BaseDirectory, name);
            else
            {
                if (domain.SetupInformation.PrivateBinPath != null)
                {
                    foreach (var path in domain.SetupInformation.PrivateBinPath.Split(';'))
                    {
                        var file = Path.Combine(path, name + ".dll");
                        if (File.Exists(file))
                            return file;
                    }
                }
            }

            throw new FileNotFoundException("Cannot find file", name);
        }

        static Assembly dom_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine(args.Name);
            Debugger.Launch();
            return Assembly.LoadFile(Path.GetDirectoryName(p) + "\\" + args.Name);
        }*/
    }
}
