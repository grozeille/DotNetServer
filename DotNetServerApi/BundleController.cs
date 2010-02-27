using System;
using System.Collections.Generic;
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
    }
}