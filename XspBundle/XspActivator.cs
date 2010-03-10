using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DotNetServerApi;
using Mono.WebServer;
using System.IO;
using NDesk.DBus;
using org.freedesktop.DBus;
using XspBundle.Contracts;
using System.Reflection;
using System.Threading;

namespace XspBundle
{
    public class XspActivator : BundleActivator, IXspService, IDisposable
    {
        public const string BusName = "org.mathias.xspservice";

        private XSPWebSource websource;
        private ApplicationServer webAppServer;
        private int port;
        private Bus bus;
        private bool listen;
        private Mutex mutex = new Mutex(false);

        #region IActivator Members

        protected override void OnStart()
        {
            this.bus = Bus.Open("tcp:host=localhost,port=12345");
            //var bus = Bus.Open("win:path=dbus-session");            
            var path = new ObjectPath("/org/mathias/xspservice");

            IXspService service;
            
            if (bus.RequestName(BusName) == RequestNameReply.PrimaryOwner)
            {              
                this.port = 1254;
                this.websource = new XSPWebSource(IPAddress.Any, this.port);
                this.webAppServer = new ApplicationServer(this.websource);
                //this.webAppServer.AddApplicationsFromCommandLine(string.Format("localhost:{0}:{1}:{2}", port, "/", "Bundles/WebAspBundle"));
                this.webAppServer.Start(true);
                //create a new instance of the object to be exported
                bus.Register(path, this);

                //this.webAppServer.Start(true);
                this.listen = true;
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    //run the main loop
                    while (true)
                    {
                        mutex.WaitOne();
                        try
                        {
                            if (!this.listen)
                                break;
                            bus.Iterate();
                        }
                        finally
                        {
                            mutex.ReleaseMutex();
                        }                        
                    }
                }, null);                
            }
        }

        protected override void OnStop()
        {
            mutex.WaitOne();
            try
            {
                this.listen = false;
                this.bus.Close();
                this.bus = null;

                this.webAppServer.UnloadAll();
                this.webAppServer.Stop();
                this.webAppServer = null;

                this.websource.Dispose();
                this.websource = null;
            }
            finally
            {
                mutex.ReleaseMutex();
            }           
        }

        #endregion

        #region IXspService Members

        public void StartWebApp(string path, string virtualPath, int port)
        {
            if (virtualPath[0] != ('/'))
                virtualPath = '/' + virtualPath;

            if (virtualPath[virtualPath.Length-1] != ('/'))
                virtualPath = virtualPath + '/';

            this.webAppServer.AddApplication("localhost",port, virtualPath, path);
            
            //if (path.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
            //  path = path.Substring(AppDomain.CurrentDomain.BaseDirectory.Length+1);
            //path = path.Replace('\\', '/');
            //this.webAppServer.AddApplicationsFromCommandLine(string.Format("localhost:{0}:{1}:{2}", port, virtualPath, path));
        }

        public void StopWebApp(string path, string virtualPath, int port)
        {
            var existing = this.webAppServer.GetApplicationForPath("localhost", port, path, true);
            if(existing != null)
                existing.UnloadHost();
        }

        public int DefaultPort()
        {
            return this.port;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.websource != null)
                this.websource.Dispose();
        }

        #endregion
    }
}
