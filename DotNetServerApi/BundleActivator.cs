using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetServerApi
{
    public abstract class BundleActivator : IBundleActivator
    {
        #region IActivator Members

        public void Start()
        {
            if (this.OnStarted != null)
                this.OnStarted(this, new BundleEventArgs());
            this.OnStart();
        }

        
        public void Stop()
        {
            this.OnStop();
            if (this.OnStopped != null)
                this.OnStopped(this, new BundleEventArgs());
        }

        protected abstract void OnStart();

        protected abstract void OnStop();

        public event EventHandler<BundleEventArgs> OnStarted;

        public event EventHandler<BundleEventArgs> OnStopped;

        #endregion
    }
}
