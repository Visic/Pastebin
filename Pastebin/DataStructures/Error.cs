namespace Pastebin {
    public class Error {
        public Error(string msg) { Msg = msg; }
        public string Msg { get; private set; }
    }
}