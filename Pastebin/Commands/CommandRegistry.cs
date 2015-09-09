using System;
using System.Collections.Generic;
using System.Linq;

namespace Pastebin {
    public static class CommandRegistry {
        static readonly Dictionary<string, CommandBase> _commands = new Dictionary<string, CommandBase>();

        static CommandRegistry() {
            Register(new HelpCommand(() => _commands.Values));
            Register(new SettingsCommand(() => _commands.Values));
            Register(new LoginCommand());
            Register(new ExitCommand());
            Register(new ListCommand());
            Register(new DeleteByKeyCommand());
            Register(new DeleteByTitleCommand());
            Register(new GetByKeyCommand());
            Register(new GetByTitleCommand());
            Register(new SetListFormatCommand());
            Register(new SetDevKeyCommand());
            Register(new SetUserNameCommand());
            Register(new SetPasswordCommand());
            Register(new SetPasteVisibilityCommand());
            Register(new SetPasteExpirationCommand());

            var pasteCommand = new PasteCommand();
            PasteCommandName = pasteCommand.Name();
            Register(pasteCommand);
        }

        public static string CommandPrefix { get { return "cmd"; } }
        public static string HelpCommand { get { return string.Format("{0} {1}", CommandPrefix, "help"); } }
        public static string SettingsCommand { get { return string.Format("{0} {1}", CommandPrefix, "settings"); } }
        public static string ExitCommand { get { return string.Format("{0} {1}", CommandPrefix, "exit"); } }
        public static string PasteCommandName { get; private set; }

        public static Union<Success, Error> Execute(string cmdText) {
            var cmdAndArgs = LexCmdAndArgs_Rec(cmdText.Trim()).ToArray();
            CommandBase cmd = null;

            if (cmdAndArgs.Length == 0) {
                return Union<Success, Error>.Create(new Error("No command specified."));
            } else if (_commands.TryGetValue(cmdAndArgs[0].ToLower(), out cmd)) {
                return cmd.Cmd(cmdAndArgs.Skip(1).ToArray());
            } else {
                return Union<Success, Error>.Create(new Error(string.Format("{0} is not a known command.", cmdAndArgs.First())));
            }
        }

        private static void Register(CommandBase cmd) {
            _commands[cmd.Name().ToLower()] = cmd;
        }

        //handle quoted strings and a single arg and space delimit everything else
        private static IEnumerable<string> LexCmdAndArgs_Rec(string str) {
            if (string.IsNullOrEmpty(str)) return new List<string>();

            IEnumerable<string> argAndRest = new string[0];
            if (str.StartsWith("\"")) argAndRest = str.Split(new char[] { '"' }, 2, StringSplitOptions.RemoveEmptyEntries);
            else argAndRest = str.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

            return LexCmdAndArgs_Rec(argAndRest.Skip(1).FirstOrDefault("").Trim()).Prepend(argAndRest.First());
        }
    }
}