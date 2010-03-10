using System;
using DotNetServerApi;
using System.Threading;

namespace DBusBundle
{
    public class DBusActivator : BundleActivator
    {
        protected override void OnStart()
        {          
            DBusDaemon.RunServer("tcp:host=localhost,port=12345");
        }

        protected override void OnStop()
        {
        
        }
    }
}
