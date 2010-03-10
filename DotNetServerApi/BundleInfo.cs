using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetServerApi
{
    [Serializable]
    public class BundleInfo
    {
        public BundleController Boostrap { get; set; }

        public AppDomain AppDomain { get; set; }

        public Version Version { get; set; }

        public BundleState State { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }
    }
}