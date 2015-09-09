using System;

namespace Pastebin {
    public enum Field {
        DevKey,
        UserName,
        Password,
        SessionKey,
        ApiOption,
        Visibility,
        Expiration,
        Data,
        Name,
        Format,
        ResultsLimit,
        ListFormat,
        PasteKey
    }

    public static class FieldExtensions {
        public static string MappedVal(this Field val) {
            switch (val) {
            case Field.DevKey:
                return "api_dev_key";
            case Field.UserName:
                return "api_user_name";
            case Field.Password:
                return "api_user_password";
            case Field.SessionKey:
                return "api_user_key";
            case Field.ApiOption:
                return "api_option";
            case Field.Visibility:
                return "api_paste_private";
            case Field.Expiration:
                return "api_paste_expire_date";
            case Field.Data:
                return "api_paste_code";
            case Field.Name:
                return "api_paste_name";
            case Field.Format:
                return "api_paste_format";
            case Field.ResultsLimit:
                return "api_results_limit";
            case Field.ListFormat:
                return "ListFormat";
            case Field.PasteKey:
                return "api_paste_key";
            default:
                throw new Exception("Value not defined");
            }
        }
    }
}