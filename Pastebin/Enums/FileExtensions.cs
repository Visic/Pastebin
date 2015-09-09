using System;

namespace Pastebin {
    public static class FileExtensions {
        public enum Val {
            cs,
            cpp,
            h,
            txt,
            jl,
            lua
        }

        public static Val cs { get { return Val.cs; } }
        public static Val cpp { get { return Val.cpp; } }
        public static Val h { get { return Val.h; } }
        public static Val txt { get { return Val.txt; } }
        public static Val jl { get { return Val.jl; } }
        public static Val lua { get { return Val.lua; } }

        public static string MappedVal(string ext) {
            switch (ext) {
            case "cpp":
            case "h":
                return "cpp";
            case "txt":
            case "jl":
                return "";
            case "cs":
                return "csharp";
            case "lua":
                return "lua";

            default:
                throw new Exception("Value not defined");
            }
        }

        public static string[] GetNames() {
            return Enum.GetNames(typeof(Val));
        }
    }
}