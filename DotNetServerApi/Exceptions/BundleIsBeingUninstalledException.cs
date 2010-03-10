using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DotNetServerApi.Exceptions
{
    [Serializable]
    public class BundleIsBeingUninstalledException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public BundleIsBeingUninstalledException()
        {
        }

        public BundleIsBeingUninstalledException(string bundleName, Version requiredVersion)
            : base(string.Format("Bundle {0}:{1} found but is being uninstalled", bundleName, requiredVersion))
        {

        }

        protected BundleIsBeingUninstalledException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
