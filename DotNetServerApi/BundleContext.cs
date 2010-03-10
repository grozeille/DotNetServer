using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetServerApi
{
    public class BundleContext : MarshalByRefObject, IBundleContext
    {
        private readonly IDictionary<string, BundleInfo> bundleLoaded = new Dictionary<string, BundleInfo>();

        public IDictionary<string, BundleInfo> BundleLoaded
        {
            get
            {
                IDictionary<string, BundleInfo> clone = new Dictionary<string, BundleInfo>();
                foreach (var item in bundleLoaded)
                    clone.Add(item.Key, item.Value);
                return clone;
            }
        }

        #region IBundleContext Members

        public void RegisterService(string name, object service)
        {
            throw new NotImplementedException();
        }

        public T GetServicer<T>(string name)
        {
            throw new NotImplementedException();
        }

        public BundleInfo GetBundle(string name)
        {
            BundleInfo info = null;
            this.bundleLoaded.TryGetValue(name.ToUpper(), out info);
            return info;
        }

        public void ChangeBundleState(string name, BundleState state)
        {
            BundleInfo info = null;
            if (this.bundleLoaded.TryGetValue(name.ToUpper(), out info))
                info.State = state;
        }

        public void RegisterBundle(BundleInfo bundle)
        {
            bundleLoaded[bundle.Name.ToUpper()] = bundle;
        }

        public void Unregister(BundleInfo bundle)
        {
            bundleLoaded.Remove(bundle.Name.ToUpper());
        }

        #endregion
    }
}
