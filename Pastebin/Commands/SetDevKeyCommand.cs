using System.Collections.Generic;

namespace Pastebin {
    public class SetDevKeyCommand : ModifySettingCommand<string> {
        public override string Name() {
            return "DevKey";
        }

        public override IEnumerable<string> Args() {
            return new string[] { "dev key" };
        }

        public override Option<Field> GetField() {
            return new Option<Field>(Field.DevKey);
        }

        public override bool Encrypted() {
            return true;
        }
    }
}