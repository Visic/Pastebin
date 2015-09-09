using System.Collections.Generic;

namespace Pastebin {
    public class SetUserNameCommand : ModifySettingCommand<string> {
        public override string Name() {
            return "UserName";
        }

        public override IEnumerable<string> Args() {
            return new string[] { "user name" };
        }

        public override Option<Field> GetField() {
            return new Option<Field>(Field.UserName);
        }

        public override bool Encrypted() {
            return true;
        }
    }
}