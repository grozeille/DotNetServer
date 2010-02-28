using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetServerApi
{
    public class BundleInfo
    {
        public BundleController Boostrap { get; set; }

        public AppDomain AppDomain { get; set; }
    }
}