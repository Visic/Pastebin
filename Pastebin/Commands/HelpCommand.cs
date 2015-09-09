using System;
using System.Collections.Generic;
using System.Linq;

namespace Pastebin {
    public class HelpCommand : CommandBase {
        Func<IEnumerable<CommandBase>> _commands;
        public HelpCommand(Func<IEnumerable<CommandBase>> commands) {
            _commands = commands;
        }

        public override IEnumerable<string> Args() {
            return new string[] { "Optional- command name" };
        }

        public override string Name() {
            return "Help";
        }

        public override string Description() {
            return "Displays detailed usage information for [command name] unless [command name] is not specified, in which case Help displays general information for all known commands.";
        }

        public override Union<Success, Error> Cmd(string[] args) {
            return GetParamAs<string>(args, 0).Match<Union<Success, Error>>(
                commandName => {
                    if (!_commands().Select(x => x.Name().ToLower()).Contains(commandName.ToLower())) return MakeError("Command does not exist");
                    var desc = _commands().First(x => x.Name().ToLower() == commandName.ToLower()).Description();
                    if (desc != CommandBase.C_DefaultDesc) return MakeSuccess(desc);
                    else return MakeError(desc);
                },
                all => {
                    var longestNameLength = _commands().Max(x => x.Name().Length);
                    return MakeSuccess("Commands: \n\t-" + _commands().Select(x => MakeDesc(x, longestNameLength)).ToDelimitedString("\n\t-")
                                     + "\n\nFor detailed usage about a command type \"help [command name]\"");
                }
            );
        }

        private string MakeDesc(CommandBase cmd, int longestNameLength) {
            var args = cmd.Args().Select(x => string.Format("[{0}]", x)).ToDelimitedString(" ");

            return string.Format("{0} {1}", cmd.Name().PadRight(longestNameLength), args);
        }
    }
}