using System.Collections.Generic;

namespace Pastebin {
    public class SetPasswordCommand : ModifySettingCommand<string> {
        public override string Name() {
            return "Password";
        }

        public override IEnumerable<string> Args() {
            return new string[] { "password" };
        }

        public override Option<Field> GetField() {
            return new Option<Field>(Field.Password);
        }

        public override bool Encrypted() {
            return true;
        }
    }
}