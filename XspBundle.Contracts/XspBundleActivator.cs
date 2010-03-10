using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetServerApi;
using NDesk.DBus;
using System.Reflection;
using System.IO;

namespace XspBundle.Contracts
{
    public abstract class XspBundleActivator : BundleActivator
    {
        protected IXspService XspService;

        private string physiqualPath;

        protected override void OnStart()
        {
            var bus = Bus.Open("tcp:host=localhost,port=12345");
            //var bus = Bus.Open("win:path=dbus-session");

            string bus_name = "org.mathias.xspservice";
            ObjectPath path = new ObjectPath("/org/mathias/xspservice");

            this.XspService = bus.GetObject<IXspService>(bus_name, path);
            
            this.XspService.StartWebApp(this.PhysicalPath, this.VirtualPath, this.Port);
        }

        protected virtual string VirtualPath
        { 
            get
            {
                AssemblyName name = new AssemblyName(Assembly.GetAssembly(this.GetType()).FullName);
                return string.Format("/{0}/", name.Name);
            }
        }

        protected virtual int Port
        {
            get
            {
                return this.XspService.DefaultPort();
            }
        }

        protected virtual string PhysicalPath
        {
            get
            {
                // must be in "..\" as the bundle is in the "bin" folder
                if (this.physiqualPath == null)                
                    this.physiqualPath= Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetAssembly(this.GetType()).Location));
                return this.physiqualPath;
            }
        }

        protected override void OnStop()
        {
            this.XspService.StopWebApp(this.PhysicalPath, this.VirtualPath, this.Port);
        }
    }
}
