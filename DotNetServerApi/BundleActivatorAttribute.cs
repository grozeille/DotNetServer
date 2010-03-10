using System;

namespace DotNetServerApi
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleActivatorAttribute : Attribute
    {
        public Type Activator { get; set; }

        public BundleActivatorAttribute(Type activator)
        {
            this.Activator = activator;
        }
    }
}