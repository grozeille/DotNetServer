using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetServerApi
{
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple = true)]
    public class BundleDependencyAttribute : Attribute
    {
        public Version Version { get; set; }

        public string Name { get; set; }

        public BundleDependencyAttribute(string name, string version)
        {
            this.Name = name;
            this.Version = new Version(version);
        }
    }
}
