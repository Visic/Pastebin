using System;
using System.Collections.Generic;
using System.Linq;

namespace Pastebin {
    public class SettingsCommand : CommandBase {
        Func<IEnumerable<CommandBase>> _commands;
        public SettingsCommand(Func<IEnumerable<CommandBase>> commands) {
            _commands = commands;
        }

        public override string Name() {
            return "Settings";
        }

        public override string Description() {
            return "Displays all current Settings";
        }

        public override Union<Success, Error> Cmd(string[] args) {
            var commandsWithSettings = _commands().Select(x => new { cmd = x, setting = x.Setting() }).Where(x => x.setting.IsSome);
            var longestNameLength = commandsWithSettings.Max(x => x.cmd.Name().Length);

            Console.WriteLine("Settings: \n" + commandsWithSettings.Select(x => MakeSettingText(x.cmd, longestNameLength, x.setting.Value)).ToDelimitedString("\n"));
            return MakeSuccess("");
        }

        private string MakeSettingText(CommandBase cmd, int longestNameLength, object setting) {
            return string.Format("\t-{0} [{1}]", cmd.Name().PadRight(longestNameLength), setting);
        }
    }
}