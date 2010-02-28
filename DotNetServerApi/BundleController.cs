using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using DotNetServerApi;

namespace DotNetServerApi
{
    public class BundleController : MarshalByRefObject
    {
        private IActivator instance;

        public IActivator CurrentActivator
        {
            get
            {
                return this.instance;
            }
        }

        public void StartActivator()
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationName;            
            var name = AssemblyName.GetAssemblyName(path);
            var assembly = AppDomain.CurrentDomain.Load(name);
            var attribute = (BundleAttribute)assembly.GetCustomAttributes(typeof(BundleAttribute), false).FirstOrDefault();

            // if there's some dependencies, load them
            if(attribute.Depends != null)
            {
                foreach(string bundle in attribute.Depends)
                {
                    if(!applicationLoaded.ContainsKey(bundle))
                    {
                        Console.WriteLine("Loading dependency: {0}", bundle);
                        BundleController.Start(bundle);
                    }
                }
            }

            var activatorType = Type.GetType(attribute.Activator.AssemblyQualifiedName);
            var ctor = activatorType.GetConstructor(new Type[0]);
            this.instance = (IActivator)ctor.Invoke(new object[0]);

            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => ((IActivator)o).Start()), this.instance);
        }

        public void StopActivator()
        {
            lock (this.instance)
            {
                this.instance.Stop();
            }
        }

        private static IDictionary<string, BundleInfo> applicationLoaded = new Dictionary<string, BundleInfo>();

        public static IDictionary<string, BundleInfo> ApplicationLoaded
        {
            get
            {
                IDictionary<string, BundleInfo> clone = new Dictionary<string, BundleInfo>();
                foreach(var item in applicationLoaded)
                    clone.Add(item.Key, item.Value);
                return clone;                
            }
        }

        public static void Stop(string assemblyName)
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

        public static void Start(string assemblyName)
        {
            if (applicationLoaded.ContainsKey(assemblyName))
            {
                Console.WriteLine("Application {0} already loaded", assemblyName);
                return;
            }

            string path = Path.GetFullPath(string.Format("Bundles\\{0}\\{0}.dll", assemblyName));

            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            ApplicationIdentity identity = new ApplicationIdentity(AssemblyName.GetAssemblyName(path).Name);
            //setup.ActivationArguments = new ActivationArguments(identity, new string[] { p });
            setup.ApplicationName = path;

            setup.AppDomainInitializer = null;
            //setup.AppDomainInitializer = new AppDomainInitializer(Initialize);
            //setup.AppDomainInitializerArguments = new string[]{ path };

            List<string> paths = new List<string>();
            paths.Add(Path.GetDirectoryName(path));
            paths.Add(Path.Combine(setup.ApplicationBase, "Libs"));
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

            BundleController boot = (BundleController)dom.CreateInstanceAndUnwrap(typeof(BundleController).Assembly.FullName, typeof(BundleController).FullName);

            applicationLoaded.Add(assemblyName, new BundleInfo { AppDomain = dom, Boostrap = boot });

            boot.StartActivator();

            //dom.DoCallBack(Start);            
        }

        static void dom_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

        }

        static void dom_DomainUnload(object sender, EventArgs e)
        {
            var unload = ((AppDomain)sender);
        }

        static void dom_ProcessExit(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}