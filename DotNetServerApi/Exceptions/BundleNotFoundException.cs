using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DotNetServerApi.Exceptions
{
    [Serializable]
    public class BundleNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public BundleNotFoundException()
        {
        }

        public BundleNotFoundException(string bundleName, Version requiredVersion)
            : base(string.Format("No bundle found with name {0} and version {1}", bundleName, requiredVersion))
        {

        }

        protected BundleNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
