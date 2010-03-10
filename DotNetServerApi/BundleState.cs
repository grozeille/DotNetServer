using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetServerApi
{
    public enum BundleState
    {
        Installed,
        Resolved,
        Starting,
        Active,
        Stopping,
        Uninstalled
    }
}
