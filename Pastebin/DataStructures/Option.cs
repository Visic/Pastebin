using System;

namespace Pastebin {
    public struct Option<T> {
        public Option(T value)
            : this() {
            Value = value;
        }

        public bool IsSome { get; private set; }
        public bool IsNone { get { return !IsSome; } }

        T _value;
        public T Value {
            get {
                if (IsNone) throw new InvalidOperationException("Value is not defined on None.");
                return _value;
            }
            private set {
                _value = value;
                IsSome = true;
            }
        }

        public Option<RT> Apply<RT>(Func<T, RT> func) {
            return Apply(x => new Option<RT>(func(x)));
        }

        public Option<RT> Apply<RT>(Func<T, Option<RT>> func) {
            return IsSome ? func(Value) : new Option<RT>();
        }
    }
}