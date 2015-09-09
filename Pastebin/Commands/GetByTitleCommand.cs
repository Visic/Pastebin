using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pastebin {
    public class GetByTitleCommand : CommandBase {
        public override IEnumerable<string> Args() {
            return new string[] { "title", "Optional- output path" };
        }

        public override string Name() {
            return "GetByTitle";
        }

        public override string Description() {
            return "Downloads the Pastes that have a Title which 'contains' [title].\n" +
                   "If [output path] is specified, the Pastes will be downloaded into that location, otherwise they will be downloaded to the desktop.";
        }

        public override Union<Success, Error> Cmd(string[] args) {
            return GetParamAs<string>(args, 0).Match<Union<Success, Error>>(
                title => {
                    var maybeOutputPath = GetParamAs<string>(args, 1).Type1Value;
                    var format = string.Format("{0}:{1}:{2}", ListField.Title, ListField.Key, ListField.Visibility);

                    return new ListCommand().Cmd(new string[] { title, format }).Match<Union<Success, Error>>(
                        list => {
                            var getByKeyCmd = new GetByKeyCommand();
                            var pastesToGet = list.Msg.Split('\n').Select(x => x.Split(':'));

                            foreach (var pasteInfo in pastesToGet) {
                                string.Format("Downloading: {0}  -  {1}", pasteInfo[0], pasteInfo[1]).Println(ConsoleColor.Cyan);
                                if (pasteInfo[2] == Visibility.Private.ToString()) {
                                    "Error: Cannot download a Private paste".Println(ConsoleColor.Red);
                                    continue;
                                }

                                var rootPath = maybeOutputPath.IsSome ? maybeOutputPath.Value : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                                getByKeyCmd.GetByKey(pasteInfo[1], Path.Combine(rootPath, pasteInfo[0])).Match(
                                    success => success.Msg.Println(ConsoleColor.Green),
                                    err => err.Msg.Println(ConsoleColor.Red)
                                );
                            }

                            if (pastesToGet.Count() == 0) return MakeError("No pastes with a matching title");
                            else return MakeSuccess("");
                        },
                        err => MakeError(err.Msg)
                    );
                },
                err => MakeError(err.Msg)
            );
        }
    }
}