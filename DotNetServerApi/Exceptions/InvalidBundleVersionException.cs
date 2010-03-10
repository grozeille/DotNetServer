using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DotNetServerApi.Exceptions
{
    [Serializable]
    public class InvalidBundleVersionException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidBundleVersionException()
        {
        }

        public InvalidBundleVersionException(string bundleName, Version requiredVersion, Version existingVersion)
            : base(string.Format("The bundle {0} was found with incorrect version: required {1} but found {2}", bundleName, requiredVersion, existingVersion))
        {

        }

        protected InvalidBundleVersionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}