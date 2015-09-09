using System.Collections.Generic;

namespace Pastebin {
    public class SetListFormatCommand : ModifySettingCommand<string> {
        //format any number of anystring|ListField.ToString()
        public override string Name() {
            return "ListFormat";
        }

        public override Union<Success, Error> Cmd(string[] args) {
            //take all args as the single string format arg (so you don't have to put quotes around the whole format each time)
            return base.Cmd(new string[] { args.ToDelimitedString(" ") });
        }

        public override IEnumerable<string> Args() {
            return new string[] { string.Format("[{0}] delimiter...", ListField.GetNames().ToDelimitedString("|")) };
        }

        public override Option<Field> GetField() {
            return new Option<Field>(Field.ListFormat);
        }
    }
}