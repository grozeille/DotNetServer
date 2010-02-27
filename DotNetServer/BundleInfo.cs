using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetServerApi;

namespace DotNetServer
{
    public class BundleInfo
    {
        public BundleController Boostrap { get; set; }

        public AppDomain AppDomain { get; set; }
    }
}
