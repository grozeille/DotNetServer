using System.Configuration;

namespace DotNetServerApi.Configuration
{
    public class BundleConfigurationSection : ConfigurationSection
    {
        public BundleConfigurationSection()
        {
        }

        public BundleConfigurationSection(string bundlePath)
        {
            this.BundlePath = bundlePath;
        }

        [ConfigurationProperty("bundlePath", IsRequired = true)]
        public string BundlePath
        {
            get
            {
                return (string)this["bundlePath"];
            }
            set
            {
                this["bundlePath"] = value;
            }
        }
    }
}
