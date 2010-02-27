using System;
using System.Collections.Generic;
using System.Text;
using DotNetServerApi;
using System.Threading;

namespace MyBundle
{
    public class MyActivator : IActivator
    {
        public void Start()
        {
            Console.WriteLine("Started");
            for (int i = 0; i < 10; i++)
            {
                //Console.WriteLine("{0}...",i);
                Thread.Sleep(1000);
            }
            //throw new Exception("Haha");
        }

        public void Stop()
        {
            Console.WriteLine("Stopped");
        }
    }
}
