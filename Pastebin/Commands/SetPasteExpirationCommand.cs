using System.Collections.Generic;
using System.Linq;

namespace Pastebin {
    public class SetPasteExpirationCommand : ModifySettingCommand<string> {
        public override Union<Success, Error> Cmd(string[] args) {
            var maybeError = GetParamAs<string>(args, 0).Match<Option<Error>>(
                arg => {
                    if (Expiration.GetNames().Select(x => x.ToLower()).Contains(arg.ToLower())) {
                        return new Option<Error>();
                    } else {
                        return new Option<Error>(new Error(string.Format("{0} is not a valid expiration time.", arg)));
                    }
                },
                err => new Option<Error>(err)
            );

            if (maybeError.IsSome) return MakeError(maybeError.Value.Msg);
            return base.Cmd(args);
        }

        public override string Name() {
            return "Expiration";
        }

        public override IEnumerable<string> Args() {
            return new string[] { Expiration.GetNames().ToDelimitedString("|") };
        }

        public override Option<Field> GetField() {
            return new Option<Field>(Field.Expiration);
        }
    }
}