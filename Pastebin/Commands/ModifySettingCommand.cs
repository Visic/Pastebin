namespace Pastebin {
    public abstract class ModifySettingCommand<T> : CommandBase {
        public override Union<Success, Error> Cmd(string[] args) {
            return GetParamAs<T>(args, 0).Match<Union<Success, Error>>(
                setting => {
                    PersistentSettings.Store(Name(), setting);

                    if (GetField().IsSome) ApplicationState.Context[GetField().Value.MappedVal()] = "" + setting;
                    return MakeSuccess("Setting applied");
                },
                err => MakeError(err.Msg)
            );
        }

        public override Option<object> Setting() {
            return new Option<object>(
                PersistentSettings.Recall<object>(Name())
            );
        }

        public virtual Option<Field> GetField() {
            return new Option<Field>();
        }

        public override string Description() {
            var desc = string.Format("Sets the {0} setting to [{1}].", Name(), Args().ToDelimitedString(", "));
            if (Encrypted()) desc = desc + "\n--This setting gets encrypted for storage.";
            return desc;
        }

        public virtual bool Encrypted() {
            return false;
        }
    }
}