using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Configuration;
using Common.Logging;
using DotNetServerApi.Configuration;
using DotNetServerApi.Exceptions;

namespace DotNetServerApi
{
    public class BundleController : MarshalByRefObject
    {
        private static ILog logger = LogManager.GetLogger(typeof (BundleController));

        private IBundleActivator instance;

        public IBundleActivator CurrentActivator
        {
            get
            {
                return this.instance;
            }
        }

        public void StartActivator(IBundleContext context)
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationName;            
            var name = AssemblyName.GetAssemblyName(path);

            context.ChangeBundleState(name.Name, BundleState.Starting);

            // load the assembly in the AppDomain
            var assembly = AppDomain.CurrentDomain.Load(name);           

            // get the meta-data of the bundle
            var activatorAttribute = (BundleActivatorAttribute)assembly.GetCustomAttributes(typeof(BundleActivatorAttribute), false).FirstOrDefault();
            var dependencyAttributes = assembly.GetCustomAttributes(typeof(BundleDependencyAttribute), false).Cast<BundleDependencyAttribute>();

            // if there's some dependencies, load them
            if (dependencyAttributes != null)
            {
                foreach (var bundle in dependencyAttributes)
                {
                    var existingBundle = context.GetBundle(bundle.Name);
                    if (existingBundle == null)
                    {                        
                        throw new BundleNotFoundException(bundle.Name, bundle.Version);                       
                    }
                    else
                    {
                        if (existingBundle.Version != bundle.Version)
                        {
                            throw new InvalidBundleVersionException(bundle.Name, bundle.Version, existingBundle.Version);
                        }
                        else if (existingBundle.State == BundleState.Installed)
                        {
                            BundleController.Start(context, bundle.Name);
                        }
                        else if (existingBundle.State == BundleState.Resolved)
                        {
                            BundleController.Start(context, bundle.Name);
                        }
                        else if (existingBundle.State == BundleState.Starting)
                        {
                            BundleController.WaitFor(context, bundle.Name, BundleState.Active);
                        }
                        else if (existingBundle.State == BundleState.Stopping)
                        {
                            BundleController.WaitFor(context, bundle.Name, BundleState.Resolved);
                            BundleController.Start(context, bundle.Name);
                        }
                        else if (existingBundle.State == BundleState.Uninstalled)
                            throw new BundleIsBeingUninstalledException(existingBundle.Name, existingBundle.Version);

                        logger.InfoFormat("Loading dependency: {0}", bundle);
                    }
                }
            }

            // create a new instance of the activator
            var activatorType = Type.GetType(activatorAttribute.Activator.AssemblyQualifiedName);
            var ctor = activatorType.GetConstructor(new Type[0]);
            this.instance = (IBundleActivator)ctor.Invoke(new object[0]);
            
            // start the bundle
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => ((IBundleActivator)o).Start()), this.instance);

            context.ChangeBundleState(name.Name, BundleState.Active);
        }

        private static void WaitFor(IBundleContext context, string bundleName, BundleState bundleState)
        {
            while(context.GetBundle(bundleName).State != bundleState)
                Thread.Sleep(1000);
        }

        public void StopActivator(IBundleContext context)
        {
            this.instance.Stop();
        }

        public static void Stop(IBundleContext context, string bundleName)
        {
            var bundle = context.GetBundle(bundleName);
            if (bundle == null)
            {
                logger.InfoFormat("Application {0} not loaded", bundleName);
                return;
            }
            context.ChangeBundleState(bundle.Name, BundleState.Stopping);
            bundle.Boostrap.StopActivator(context);
            context.ChangeBundleState(bundle.Name, BundleState.Resolved);
            AppDomain.Unload(bundle.AppDomain);
        }

        public static void Start(IBundleContext context, string bundleName)
        {
            // load all installed bundle
            RefreshAllBundles(context);

            // TODO : handle installation of new bundle?
            var bundleInfo = context.GetBundle(bundleName);
            if(bundleInfo == null)
                throw new BundleNotFoundException(bundleName, null);

            if (bundleInfo.State == BundleState.Active)
            {
                logger.InfoFormat("Application {0} already loaded", bundleName);
                return;
            }

            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            ApplicationIdentity identity = new ApplicationIdentity(bundleInfo.Name);
            //setup.ActivationArguments = new ActivationArguments(identity, new string[] { p });
            setup.ApplicationName = bundleInfo.Path;

            setup.AppDomainInitializer = null;
            //setup.AppDomainInitializer = new AppDomainInitializer(Initialize);
            //setup.AppDomainInitializerArguments = new string[]{ path };

            List<string> paths = new List<string>();
            paths.Add(Path.GetDirectoryName(bundleInfo.Path));
            paths.Add(Path.Combine(setup.ApplicationBase, "Libs"));
            paths.AddRange(Directory.GetDirectories(Path.Combine(setup.ApplicationBase, "Libs")));
            setup.PrivateBinPath = string.Join(";", paths.ToArray());

            var dom = AppDomain.CreateDomain(bundleInfo.Path, AppDomain.CurrentDomain.Evidence, setup);

            dom.ProcessExit += new EventHandler(dom_ProcessExit);
            dom.DomainUnload += new EventHandler(dom_DomainUnload);
            dom.UnhandledException += new UnhandledExceptionEventHandler(dom_UnhandledException);

            bundleInfo.Boostrap = (BundleController)dom.CreateInstanceAndUnwrap(typeof(BundleController).Assembly.FullName, typeof(BundleController).FullName);
            bundleInfo.AppDomain = dom;
            
            bundleInfo.State = BundleState.Resolved;
            context.RegisterBundle(bundleInfo);

            bundleInfo.Boostrap.StartActivator(context);
        }

        public static void RefreshAllBundles(IBundleContext context)
        {
            // parse each folders to register new bundles
            foreach (var folderPath in Directory.GetDirectories(Path.GetFullPath(string.Format("Bundles"))))
            {
                var bundleName = new DirectoryInfo(folderPath).Name;
                var bundleInfo = context.GetBundle(bundleName);
                if (bundleInfo == null)
                {
                    string path = Path.GetFullPath(string.Format("Bundles\\{0}\\{0}.dll", bundleName));

                    // the assembly doesn't exist, try the bundle.config? Web.config?
                    if (!File.Exists(path))
                    {
                        System.Configuration.Configuration configuration = null;

                        if (File.Exists(Path.GetFullPath(string.Format("Bundles\\{0}\\{0}.dll.config", bundleName))))
                        {
                            configuration = ConfigurationManager.OpenExeConfiguration(path);
                        }
                        else if (File.Exists(Path.GetFullPath(string.Format("Bundles\\{0}\\web.config", bundleName))))
                        {
                            VirtualDirectoryMapping vdm =
                                new VirtualDirectoryMapping(
                                    Path.GetFullPath(string.Format("Bundles\\{0}", bundleName)), true);
                            WebConfigurationFileMap wcfm = new WebConfigurationFileMap();
                            wcfm.VirtualDirectories.Add("/", vdm);

                            // Get the Web application configuration object.
                            configuration = WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
                        }
                        else
                        {
                            throw new BundleNotFoundException(bundleName, new Version());
                        }

                        // TODO : better error handling
                        var section = (BundleConfigurationSection) configuration.GetSection("bundleConfiguration");

                        path = Path.GetFullPath(string.Format("Bundles\\{0}\\{1}", bundleName, section.BundlePath));
                        if (!File.Exists(path))
                            throw new BundleNotFoundException(bundleName, new Version());
                    }
                   
                    // create the bundle with all information needed
                    var assembly = Assembly.ReflectionOnlyLoadFrom(path);
                    var version = assembly.GetName().Version;
                    bundleInfo = new BundleInfo
                    {
                        Path = path,
                        Name = bundleName,
                        Version = assembly.GetName().Version,
                        State = BundleState.Installed
                    };

                    // and then, register it
                    context.RegisterBundle(bundleInfo);
                }
            }
        }

        static void dom_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Fatal("Unhandled exception", e.ExceptionObject as Exception);
        }

        static void dom_DomainUnload(object sender, EventArgs e)
        {
            logger.InfoFormat("Unloading domain {0}", sender);
            var unload = ((AppDomain)sender);
            // TODO : change state to "resolved"
        }

        static void dom_ProcessExit(object sender, EventArgs e)
        {
            logger.InfoFormat("Exiting process {0}", sender);
        }
    }    
}