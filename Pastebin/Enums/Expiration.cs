using System;

namespace Pastebin {
    public static class Expiration {
        public enum Val {
            Never,
            TenMinutes,
            OneHour,
            OneDay,
            OneWeek,
            TwoWeeks,
            OneMonth
        }

        public static Val Never { get { return Val.Never; } }
        public static Val TenMinutes { get { return Val.TenMinutes; } }
        public static Val OneHour { get { return Val.OneHour; } }
        public static Val OneDay { get { return Val.OneDay; } }
        public static Val OneWeek { get { return Val.OneWeek; } }
        public static Val TwoWeeks { get { return Val.TwoWeeks; } }
        public static Val OneMonth { get { return Val.OneMonth; } }

        public static string MappedVal(this Val val) {
            return MappedVal(val.ToString());
        }

        public static string MappedVal(string val) {
            switch (val.ToLower()) {
            case "never":
                return "N";
            case "tenminutes":
                return "10M";
            case "onehour":
                return "1H";
            case "oneday":
                return "1D";
            case "oneweek":
                return "1W";
            case "twoweeks":
                return "2W";
            case "onemonth":
                return "1M";
            default:
                throw new Exception("Not defined.");
            }
        }

        public static Val ReverseMap(string val) {
            switch (val.ToUpper()) {
            case "N":
            case "0": //0 seems to be an oddity, it happens as a result from the list command
                return Val.Never;
            case "10M":
                return Val.TenMinutes;
            case "1H":
                return Val.OneHour;
            case "1D":
                return Val.OneDay;
            case "1W":
                return Val.OneWeek;
            case "2W":
                return Val.TwoWeeks;
            case "1M":
                return Val.OneMonth;
            default:
                throw new Exception("Not defined.");
            }
        }

        public static string[] GetNames() {
            return Enum.GetNames(typeof(Val));
        }
    }
}