using System;

namespace Pastebin {
    public static class ApiFunction {
        public enum Val {
            Paste,
            List,
            Delete
        }

        public static Val Paste { get { return Val.Paste; } }
        public static Val List { get { return Val.List; } }
        public static Val Delete { get { return Val.Delete; } }

        public static string MappedVal(this Val val) {
            switch (val) {
            case ApiFunction.Val.Paste:
                return "paste";
            case ApiFunction.Val.List:
                return "list";
            case ApiFunction.Val.Delete:
                return "delete";
            default:
                throw new Exception("Not defined.");
            }
        }

        public static Val ReverseMap(string val) {
            switch (val.ToLower()) {
            case "paste":
                return Val.Paste;
            case "list":
                return Val.List;
            case "delete":
                return Val.Delete;
            default:
                throw new Exception("Not defined.");
            }
        }

        public static string[] GetNames() {
            return Enum.GetNames(typeof(Val));
        }
    }
}