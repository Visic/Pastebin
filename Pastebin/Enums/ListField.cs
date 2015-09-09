using System;

namespace Pastebin {
    public static class ListField {
        public enum Val {
            Key,
            Date,
            Title,
            Size,
            Expiration,
            Visibility,
            Format_Long,
            Format_Short,
            Url,
            Hits
        }

        public static Val Key { get { return Val.Key; } }
        public static Val Date { get { return Val.Date; } }
        public static Val Title { get { return Val.Title; } }
        public static Val Size { get { return Val.Size; } }
        public static Val Expiration { get { return Val.Expiration; } }
        public static Val Visibility { get { return Val.Visibility; } }
        public static Val Format_Long { get { return Val.Format_Long; } }
        public static Val Format_Short { get { return Val.Format_Short; } }
        public static Val Url { get { return Val.Url; } }
        public static Val Hits { get { return Val.Hits; } }

        public static string MappedVal(this Val val) {
            return MappedVal(val.ToString());
        }

        public static string MappedVal(string val) {
            switch (val.ToLower()) {
            case "key":
                return "paste_key";
            case "date":
                return "paste_date";
            case "title":
                return "paste_title";
            case "size":
                return "paste_size";
            case "expiration":
                return "paste_expire_date";
            case "visibility":
                return "paste_private";
            case "format_long":
                return "paste_format_long";
            case "format_short":
                return "paste_format_short";
            case "url":
                return "paste_url";
            case "hits":
                return "paste_hits";
            default:
                throw new Exception("Not defined.");
            }
        }

        public static Val ReverseMap(string val) {
            switch (val) {
            case "paste_key":
                return Val.Key;
            case "paste_date":
                return Val.Date;
            case "paste_title":
                return Val.Title;
            case "paste_size":
                return Val.Size;
            case "paste_expire_date":
                return Val.Expiration;
            case "paste_private":
                return Val.Visibility;
            case "paste_format_long":
                return Val.Format_Long;
            case "paste_format_short":
                return Val.Format_Short;
            case "paste_url":
                return Val.Url;
            case "paste_hits":
                return Val.Hits;
            default:
                throw new Exception("Not defined.");
            }
        }

        public static Func<string, string> MappedTranslationFunc(Val val) {
            switch (val) {
            case Val.Expiration:
                return x => Pastebin.Expiration.ReverseMap(x).ToString();
            case Val.Visibility:
                return x => Pastebin.Visibility.ReverseMap(x).ToString();
            default:
                return x => x;
            }
        }

        public static string[] GetNames() {
            return Enum.GetNames(typeof(Val));
        }
    }
}