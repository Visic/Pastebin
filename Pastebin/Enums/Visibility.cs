using System;

namespace Pastebin {
    public static class Visibility {
        public enum Val {
            Public,
            Unlisted,
            Private
        }

        public static Val Public { get { return Val.Public; } }
        public static Val Unlisted { get { return Val.Unlisted; } }
        public static Val Private { get { return Val.Private; } }

        public static string MappedVal(this Val val) {
            return MappedVal(val.ToString());
        }

        public static string MappedVal(string val) {
            switch (val.ToLower()) {
            case "public":
                return "0";
            case "unlisted":
                return "1";
            case "private":
                return "2";
            default:
                throw new Exception("Not defined.");
            }
        }

        public static Val ReverseMap(string val) {
            switch (val) {
            case "0":
                return Val.Public;
            case "1":
                return Val.Unlisted;
            case "2":
                return Val.Private;
            default:
                throw new Exception("Not defined.");
            }
        }

        public static string[] GetNames() {
            return Enum.GetNames(typeof(Val));
        }
    }
}