using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Pastebin {
    public class GetByKeyCommand : CommandBase {
        public override IEnumerable<string> Args() {
            return new string[] { "key", "Optional- output path" };
        }

        public override string Name() {
            return "GetByKey";
        }

        public override string Description() {
            return "Downloads the Paste that has the specified [key].\n" +
                   "If [output path] is specified, the Paste will be downloaded into that location, otherwise it will be downloaded to the desktop.";
        }

        public override Union<Success, Error> Cmd(string[] args) {
            return GetParamAs<string>(args, 0).Match<Union<Success, Error>>(
                key => {
                    var maybeOutputPath = GetParamAs<string>(args, 1).Type1Value;
                    var rootPath = maybeOutputPath.IsSome ? maybeOutputPath.Value : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    return DeterminePath(key).Match<Union<Success, Error>>(
                        path => GetByKey(key, Path.Combine(rootPath, path.Msg)),
                        err => MakeError(err.Msg)
                    );

                },
                err => MakeError(err.Msg)
            );
        }

        public Union<Success, Error> GetByKey(string key, string filePath) {
            var httpClient = new HttpClient();
            var result = httpClient.GetAsync(ApplicationState.PastebinRAWUrl + key).Result;
            if (!result.IsSuccessStatusCode) return MakeError(result.ReasonPhrase);

            var response = result.Content.ReadAsStringAsync().Result;

            var directoryPath = Path.GetDirectoryName(filePath);
            if (File.Exists(filePath)) File.Delete(filePath);
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
            File.WriteAllText(filePath, response);

            return MakeSuccess("Download successful");
        }

        private Union<Success, Error> DeterminePath(string key) {
            var format = string.Format("{0}:{1}:{2}", ListField.Title, ListField.Key, ListField.Visibility);

            var result = new ListCommand().Cmd(new string[] { "", format }).Match<Union<Success, Error>>(
                list => {
                    var pasteInfo = list.Msg.Split('\n').Select(x => x.Split(':').ToArray()).FirstOrDefault(x => x[1] == key);
                    if (pasteInfo == null) return MakeError("Key not found");
                    if (pasteInfo[2] == Visibility.Private.ToString()) return MakeError("Cannot download a Private paste");
                    else return MakeSuccess(pasteInfo[0]);
                },
                err => MakeError(err.Msg)
            );
            return result;
        }
    }
}