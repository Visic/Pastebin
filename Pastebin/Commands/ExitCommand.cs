namespace Pastebin {
    public class ExitCommand : CommandBase {
        public override string Name() {
            return "Exit";
        }

        public override string Description() {
            return "Exits the application";
        }

        public override Union<Success, Error> Cmd(string[] args) {
            ApplicationState.Exiting = true;
            return MakeSuccess("");
        }
    }
}