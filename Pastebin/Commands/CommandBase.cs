using System.Collections.Generic;

namespace Pastebin {
    public abstract class CommandBase {
        public const string C_DefaultDesc = "No description available";

        public abstract string Name();

        //implementation of the actual command which
        public abstract Union<Success, Error> Cmd(string[] args);

        public virtual IEnumerable<string> Args() {
            return new string[0];
        }

        public virtual Option<object> Setting() {
            return new Option<object>();
        }

        public virtual string Description() {
            return C_DefaultDesc;
        }

        protected Union<T, Error> GetParamAs<T>(string[] parameters, int index) {
            if (index > parameters.Length - 1) return MakeError<T>("Too few parameters. Check [help] for usage.");

            if (typeof(T) == typeof(bool)) {
                if (parameters[index].ToLower() == "true") return MakeResult((T)(object)true);
                if (parameters[index].ToLower() == "false") return MakeResult((T)(object)false);
                return MakeError<T>("Expected [true] or [false] but found [{0}]", parameters[index]);
            } else if (typeof(T) == typeof(string)) {
                return MakeResult((T)(object)parameters[index]);
            }

            return MakeError<T>("typeof({0}) is not implemented in GetParamAs<T>", typeof(T).Name);
        }

        protected Union<Success, Error> MakeError(string msg, params string[] args) {
            return Union<Success, Error>.Create(new Error(string.Format(msg, args)));
        }

        protected Union<Success, Error> MakeSuccess(string msg, params string[] args) {
            return Union<Success, Error>.Create(new Success(string.Format(msg, args)));
        }

        protected Union<Success, Error> MakeSuccess(string msg) {
            return Union<Success, Error>.Create(new Success(msg));
        }

        private Union<T, Error> MakeError<T>(string msg, params string[] args) {
            return Union<T, Error>.Create(new Error(string.Format(msg, args)));
        }

        private Union<T, Error> MakeResult<T>(T val) {
            return Union<T, Error>.Create(val);
        }
    }
}