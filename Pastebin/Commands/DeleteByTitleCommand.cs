using System;
using System.Collections.Generic;
using System.Linq;

namespace Pastebin {
    public class DeleteByTitleCommand : CommandBase {
        public override IEnumerable<string> Args() {
            return new string[] { "title", "Optional: allowPartialMatch true|false" };
        }

        public override string Name() {
            return "DeleteByTitle";
        }

        public override string Description() {
            return "Deletes the Paste that has the specified [title].\n" +
                   "If [allowPartialMatch] is specified as [true] then any Paste with a Title which 'contains' [title] will be deleted.\n" +
                   "If [allowPartialMatch] is not specified it will default to false.";
        }

        public override Union<Success, Error> Cmd(string[] args) {
            if (args.Length == 1) args = args.Append("false").ToArray();
            return GetParamAs<bool>(args, 1).Match<Union<Success, Error>>(
                allowPartialMatch => GetParamAs<string>(args, 0).Match<Union<Success, Error>>(
                    title => {
                        var format = string.Format("{0}:{1}", ListField.Title, ListField.Key);

                        var result = new ListCommand().Cmd(new string[] { title, format }).Match<Union<Success, Error>>(
                            list => {
                                var deleteByKeyCmd = new DeleteByKeyCommand();

                                var pastesToDelete = list.Msg.Split('\n')
                                                             .Select(x => x.Split(':'))
                                                             .Where(x => allowPartialMatch || x[0] == title);

                                foreach (var titleAndKey in pastesToDelete) {
                                    string.Format("Deleting: {0}  -  {1}", titleAndKey[0], titleAndKey[1]).Println(ConsoleColor.Cyan);
                                    deleteByKeyCmd.Cmd(new string[] { titleAndKey[1] }).Match(
                                        success => success.Msg.Println(ConsoleColor.Green),
                                        err => err.Msg.Println(ConsoleColor.Red)
                                    );
                                }

                                if (pastesToDelete.Count() == 0) return MakeError("No pastes with a matching title");
                                else return MakeSuccess("");
                            },
                            err => MakeError(err.Msg)
                        );
                        return result;
                    },
                    err => MakeError(err.Msg)
                ),
                err => MakeError(err.Msg)
            );
        }
    }
}