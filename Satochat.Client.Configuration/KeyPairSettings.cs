using System;
using System.Configuration;

namespace Satochat.Client.Configuration {
    public class KeyPairSettings : ConfigurationSection {
        [ConfigurationProperty("PrivateKey")]
        public String PrivateKey {
            get => (String)this["PrivateKey"];
            set => this["PrivateKey"] = value;
        }

        [ConfigurationProperty("PublicKey")]
        public String PublicKey {
            get => (String)this["PublicKey"];
            set => this["PublicKey"] = value;
        }

        [ConfigurationProperty("PublicKeyUploaded", IsRequired = true)]
        public Boolean PublicKeyUploaded {
            get => (Boolean)this["PublicKeyUploaded"];
            set => this["PublicKeyUploaded"] = value;
        }

        public static KeyPairSettings GetConfig() {
            const string name = "userSettings/Satochat.Client.Configuration.KeyPairSettings";
            return (KeyPairSettings)ConfigurationManager.GetSection(name) ?? new KeyPairSettings();
        }
    }
}
