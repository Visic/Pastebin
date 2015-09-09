using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Pastebin {
    public class PasteCommand : CommandBase {

        public override IEnumerable<string> Args() {
            return new string[] { "path" };
        }

        public override string Name() {
            return "Paste";
        }

        public override string Description() {
            return "Uploads the file(s) at [path] to Pastebin.\n" +
                   "If a Paste already exists with the same Title, it will be deleted first.";
        }

        public override Union<Success, Error> Cmd(string[] args) {
            var httpClient = new HttpClient();
            var maybeFilePath = GetParamAs<string>(args, 0);
            if (maybeFilePath.Type2Value.IsSome) return MakeError(maybeFilePath.Type2Value.Value.Msg);

            var filePath = maybeFilePath.Type1Value.Value;

            var paths = new string[0];
            if (Directory.Exists(filePath)) paths = Directory.GetFiles(filePath, "*", SearchOption.AllDirectories);
            else paths = new string[] { filePath };

            var deleteByTitleCmd = new DeleteByTitleCommand();
            foreach (var path in paths) {
                if (!VerifyFile(path)) continue;

                var fileName = path.Replace(Path.GetDirectoryName(filePath), "").Trim('\\');
                var fileContent = File.ReadAllText(path, Encoding.UTF8);
                var deleteResult = deleteByTitleCmd.Cmd(new string[] { fileName }); //delete matching files so we can replace them

                string.Format("Transferring: {0}", fileName).Println(ConsoleColor.Cyan);

                //if the file is empty, don't even bother with it, just replicate the error print out and continue
                if (string.IsNullOrEmpty(fileContent)) {
                    "Error: Cannot send an empty file.".Println(ConsoleColor.Red);
                    continue;
                }

                var request = new Dictionary<string, string>() { 
                    {Field.DevKey.MappedVal(), ApplicationState.Context[Field.DevKey.MappedVal()]},
                    {Field.ApiOption.MappedVal(), ApiFunction.Paste.MappedVal()},
                    {Field.Visibility.MappedVal(), Visibility.MappedVal(ApplicationState.Context[Field.Visibility.MappedVal()])},
                    {Field.Expiration.MappedVal(), Expiration.MappedVal(ApplicationState.Context[Field.Expiration.MappedVal()])},

                    {Field.Data.MappedVal(), fileContent},
                    {Field.Name.MappedVal(), fileName},
                };

                var format = FileExtensions.MappedVal(Path.GetExtension(path).Trim('.'));
                if (!string.IsNullOrEmpty(format)) request[Field.Format.MappedVal()] = format;

                var maybeSessionKey = ApplicationState.Context.TryGetValue(Field.SessionKey.MappedVal());
                if (maybeSessionKey.IsSome) request[Field.SessionKey.MappedVal()] = maybeSessionKey.Value;

                var response = httpClient.PostAsync(ApplicationState.PastebinPostUrl, new FormUrlEncodedContent(request)).Result;
                if (response.IsSuccessStatusCode) {
                    var content = response.Content.ReadAsStringAsync().Result;

                    //success response is indicated by a url to the paste
                    if (Uri.IsWellFormedUriString(content, UriKind.Absolute)) content.Println(ConsoleColor.Green);
                    else content.Println(ConsoleColor.Red);
                } else {
                    response.ReasonPhrase.Println(ConsoleColor.Red);
                }
            }

            return MakeSuccess("");
        }

        private static bool VerifyFile(string filePath) {
            if (!File.Exists(filePath)) return false;
            if (!FileExtensions.GetNames().Contains(Path.GetExtension(filePath).Trim('.'))) return false;
            return true;
        }
    }
}