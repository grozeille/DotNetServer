using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DotNetServerApi;
using Mono.WebServer;
using System.IO;

namespace XspBundle
{
    public class XspActivator : IActivator
    {
        private XSPWebSource websource;
        private ApplicationServer webAppServer;

        #region IActivator Members

        public void Start()
        {
            int Port = 1254;
            this.websource = new XSPWebSource(IPAddress.Any, Port);
            this.webAppServer = new ApplicationServer(this.websource);

            string path = "Work\\TestWebApp";
            //"[[hostname:]port:]VPath:realpath"
            string cmdLine = Port + ":/TestWebApp/:" + path;
            this.webAppServer.AddApplicationsFromCommandLine(cmdLine);
            //this.webAppServer.AddApplication(Dns.GetHostName(), Port, "/TestWebApp", path);
            

            path = "Work\\TestWebApp2";
            //"[[hostname:]port:]VPath:realpath"
            cmdLine = Port + ":/TestWebApp2/:" + path;
            this.webAppServer.AddApplicationsFromCommandLine(cmdLine);
            //this.webAppServer.AddApplication(Dns.GetHostName(), Port, "/TestWebApp2", path);*/

            this.webAppServer.Start(true);
            Console.WriteLine("Mono.WebServer running");
        }

        public void Stop()
        {
            this.webAppServer.Stop();
        }

        #endregion
    }
}
