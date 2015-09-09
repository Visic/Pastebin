using System.Collections.Generic;
using System.Net.Http;

namespace Pastebin {
    public class DeleteByKeyCommand : CommandBase {
        public override IEnumerable<string> Args() {
            return new string[] { "key" };
        }

        public override string Name() {
            return "DeleteByKey";
        }

        public override string Description() {
            return @"Deletes the Paste that has the specified [key].";
        }

        public override Union<Success, Error> Cmd(string[] args) {
            return GetParamAs<string>(args, 0).Match<Union<Success, Error>>(
                key => {
                    var httpClient = new HttpClient();
                    var maybeSessionKey = ApplicationState.Context.TryGetValue(Field.SessionKey.MappedVal());
                    if (maybeSessionKey.IsNone) return MakeError("You must be logged in to perform this action.");

                    var request = new Dictionary<string, string>() { 
                        {Field.DevKey.MappedVal(), ApplicationState.Context[Field.DevKey.MappedVal()]},
                        {Field.ApiOption.MappedVal(), ApiFunction.Delete.MappedVal()},
                        {Field.SessionKey.MappedVal(), maybeSessionKey.Value},
                        {Field.PasteKey.MappedVal(), key},
                    };

                    var result = httpClient.PostAsync(ApplicationState.PastebinPostUrl, new FormUrlEncodedContent(request)).Result;
                    if (!result.IsSuccessStatusCode) return MakeError(result.ReasonPhrase);

                    var response = result.Content.ReadAsStringAsync().Result;
                    if (response != "Paste Removed") return MakeError(response);

                    return MakeSuccess("Delete successful");
                },
                err => MakeError(err.Msg)
            );
        }
    }
}