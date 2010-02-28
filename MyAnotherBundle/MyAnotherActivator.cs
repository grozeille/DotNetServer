using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DotNetServerApi;
using MyBundleContacts;
using NDesk.DBus;

namespace MyAnotherBundle
{
    public class MyAnotherActivator : IActivator
    {
        #region IActivator Members

        private bool running;

        public void Start()
        {
            var bus = Bus.Open("tcp:host=localhost,port=12345");
            //var bus = Bus.Open("win:path=dbus-session");

            string bus_name = "org.mathias.test";
            ObjectPath path = new ObjectPath("/org/mathias/test");

            IMyService service = bus.GetObject<IMyService>(bus_name, path);

            running = true;
            while (running)
            {
                Console.WriteLine(service.Say(new Person {Name = "Mathias"}));
                Thread.Sleep(1000*2);
            }
        }

        public void Stop()
        {
            running = false;
        }

        #endregion
    }
}
