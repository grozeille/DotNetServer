using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetServerApi
{
    public interface IBundleContext
    {
        void RegisterService(string name, object service);

        T GetServicer<T>(string name);

        BundleInfo GetBundle(string name);

        void ChangeBundleState(string name, BundleState state);

        void RegisterBundle(BundleInfo bundle);

        void Unregister(BundleInfo bundle);
    }
}
