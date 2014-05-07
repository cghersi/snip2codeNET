using System.Configuration;

namespace OAuth2.Mvc.Configuration
{
    public class OAuthSection : ConfigurationSection
    {
        [ConfigurationProperty("defaultProvider")]
        public string DefaultProvider
        {
            get { return (string)base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }

        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return base["providers"] as ProviderSettingsCollection; }
        }

        [ConfigurationProperty("defaultService")]
        public string DefaultService
        {
            get { return (string)base["defaultService"]; }
            set { base["defaultService"] = value; }
        }

        [ConfigurationProperty("services")]
        public ProviderSettingsCollection Services
        {
            get { return base["services"] as ProviderSettingsCollection; }
        }
    }
}
