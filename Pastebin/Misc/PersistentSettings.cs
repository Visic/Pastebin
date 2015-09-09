using Pastebin.Properties;
using System.Linq;

namespace Pastebin {
    public static class PersistentSettings {
        static readonly string[] C_EncryptedSettingNames = new string[] { "DevKey", "UserName", "Password" };

        public static T Recall<T>(string settingName) {
            if (C_EncryptedSettingNames.Contains(settingName)) {
                return (T)Cipher.Decrypt((string)Settings.Default[settingName], ApplicationState.EncryptionKey);
            } else {
                return (T)Settings.Default[settingName];
            }
        }

        public static void Store<T>(string settingName, T value) {
            if (C_EncryptedSettingNames.Contains(settingName)) {
                Settings.Default[settingName] = Cipher.Encrypt(value, ApplicationState.EncryptionKey);
            } else {
                Settings.Default[settingName] = value;
            }

            Settings.Default.Save();
        }
    }
}