using System;
using System.Collections.Generic;
using System.Linq;

namespace Pastebin {
    class Program {
        static void Main(string[] args) {
            var encryptionKey = Utility.GetPassword("Encryption Key: ");
            Console.Clear();
            Console.WriteLine("Loading settings..");
            InitializeApplicationState(encryptionKey);
            Console.Clear();

            foreach (var str in Input(args.ToList())) {
                //if "cmd " isn't specified, then default to the PasteCommand
                var commandStr = "";
                if (str.StartsWith(CommandRegistry.CommandPrefix + " ")) commandStr = str.Remove(0, CommandRegistry.CommandPrefix.Length + 1);
                else commandStr = string.Format("{0} {1}", CommandRegistry.PasteCommandName, str);

                CommandRegistry.Execute(commandStr).Match(
                    success => { if (!string.IsNullOrEmpty(success.Msg)) success.Msg.Println(ConsoleColor.Green); },
                    err => err.Msg.Println("Error: {0}", ConsoleColor.Red)
                );

                Console.WriteLine();
                if (ApplicationState.Exiting) break;
            }
        }

        private static void InitializeApplicationState(string encryptionKey) {
            ApplicationState.EncryptionKey = encryptionKey;
            ApplicationState.PastebinLoginUrl = PersistentSettings.Recall<string>("PastebinLoginUrl");
            ApplicationState.PastebinPostUrl = PersistentSettings.Recall<string>("PastebinPostUrl");
            ApplicationState.PastebinRAWUrl = PersistentSettings.Recall<string>("PastebinRAWUrl");
            ApplicationState.Context[Field.DevKey.MappedVal()] = PersistentSettings.Recall<string>(Field.DevKey.ToString());
            ApplicationState.Context[Field.Expiration.MappedVal()] = PersistentSettings.Recall<string>(Field.Expiration.ToString());
            ApplicationState.Context[Field.Password.MappedVal()] = PersistentSettings.Recall<string>(Field.Password.ToString());
            ApplicationState.Context[Field.UserName.MappedVal()] = PersistentSettings.Recall<string>(Field.UserName.ToString());
            ApplicationState.Context[Field.Visibility.MappedVal()] = PersistentSettings.Recall<string>(Field.Visibility.ToString());
            ApplicationState.Context[Field.ListFormat.MappedVal()] = PersistentSettings.Recall<string>(Field.ListFormat.ToString());
        }

        private static IEnumerable<string> Input(List<string> cmdArgs) {
            //if the exit command is specified as a commandline arg, process it last
            if (cmdArgs.Any(x => x.ToLower() == CommandRegistry.ExitCommand.ToLower())) {
                cmdArgs.RemoveAll(x => x.ToLower() == CommandRegistry.ExitCommand.ToLower());
                cmdArgs.Add(CommandRegistry.ExitCommand);
            }

            foreach (var str in cmdArgs) {
                yield return str;
            }

            while (true) {
                Console.Write("Pastebin> ");
                yield return Console.ReadLine();
            }
        }
    }
}