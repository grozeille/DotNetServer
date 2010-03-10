using System;
using System.Threading;
using DotNetServerApi;
using MyBundle.Contracts;
using NDesk.DBus;

namespace MyAnotherBundle
{
    public class MyAnotherActivator : BundleActivator
    {
        #region IActivator Members

        private bool running;

        protected override void OnStart()
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

        protected override void OnStop()
        {
            running = false;
        }

        #endregion
    }
}
