using System;

namespace DotNetServerApi
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleAttribute : Attribute
    {
        public Type Activator { get; set; }

        public string[] Depends { get; set; }

        public BundleAttribute(Type activator)
        {
            this.Activator = activator;
        }

        public BundleAttribute(Type activator, params string[] depends)
        {
            this.Activator = activator;
            this.Depends = depends;
        }
    }
}