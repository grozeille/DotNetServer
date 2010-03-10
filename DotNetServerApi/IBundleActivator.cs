using System;

namespace DotNetServerApi
{
    public interface IBundleActivator
    {
        void Start();

        void Stop();

        event EventHandler<BundleEventArgs> OnStarted;

        event EventHandler<BundleEventArgs> OnStopped;
    }
}