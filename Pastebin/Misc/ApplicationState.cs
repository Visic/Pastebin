using System.Collections.Generic;

namespace Pastebin {
    public static class ApplicationState {
        public static Dictionary<string, string> Context = new Dictionary<string, string>();
        public static string EncryptionKey;
        public static string PastebinLoginUrl;
        public static string PastebinPostUrl;
        public static string PastebinRAWUrl;
        public static bool Exiting = false;
    }
}