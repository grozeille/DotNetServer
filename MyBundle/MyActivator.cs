﻿using System;
using DotNetServerApi;
using MyBundle.Contracts;
using NDesk.DBus;
using org.freedesktop.DBus;

namespace MyBundle
{
    public class MyActivator : BundleActivator, IMyService
    {
        protected override void OnStart()
        {
            var bus = Bus.Open("tcp:host=localhost,port=12345");
            //var bus = Bus.Open("win:path=dbus-session");

            string bus_name = "org.mathias.test";
            ObjectPath path = new ObjectPath("/org/mathias/test");

            IMyService service;

            if (bus.RequestName(bus_name) == RequestNameReply.PrimaryOwner)
            {
                //create a new instance of the object to be exported
                service = new MyActivator();
                bus.Register(path, service);

                //run the main loop
                while (true)
                    bus.Iterate();
            }
        }

        protected override void OnStop()
        {
            Console.WriteLine("Stopped");
        }

        #region IMyService Members

        public string Say(Person prs)
        {
            return string.Format("Hello {0}", prs.Name);
        }

        #endregion
    }
}
