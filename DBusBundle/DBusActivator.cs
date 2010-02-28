using System;
using System.Collections.Generic;
using System.Text;
using DotNetServerApi;
using NDesk.DBus;

namespace DBusBundle
{
    public class DBusActivator : IActivator
    {
        #region IActivator Members

        public void Start()
        {
            DBusDaemon.RunServer("tcp:host=localhost,port=12345");
        }

        public void Stop()
        {
        }

        #endregion
    }
}
