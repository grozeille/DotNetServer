using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.DBus;

namespace XspBundle.Contracts
{
    [Interface("org.mathias.xspservice")]
    public interface IXspService
    {
        void StartWebApp(string path, string virtualPath, int port);

        void StopWebApp(string path, string virtualPath, int port);

        int DefaultPort();
    }
}
