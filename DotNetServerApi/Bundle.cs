using System;

namespace DotNetServerApi
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleAttribute : Attribute
    {
        public Type Activator { get; set; }

        public BundleAttribute(Type activator)
        {
            this.Activator = activator;
        }
    }
}