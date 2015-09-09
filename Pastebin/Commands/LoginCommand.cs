using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Pastebin {
    public class LoginCommand : CommandBase {
        public override string Name() {
            return "Login";
        }

        public override string Description() {
            return "Logs into Pastebin with the credentials specified in the current Settings.";
        }

        public override Union<Success, Error> Cmd(string[] args) {
            var httpClient = new HttpClient();
            var loginRequest = new Dictionary<string, string>() { 
                {Field.DevKey.MappedVal(), PersistentSettings.Recall<string>(Field.DevKey.ToString())},
                {Field.UserName.MappedVal(), HttpUtility.UrlEncode(PersistentSettings.Recall<string>(Field.UserName.ToString()))},
                {Field.Password.MappedVal(), HttpUtility.UrlEncode(PersistentSettings.Recall<string>(Field.Password.ToString()))},
            };

            var resp = httpClient.PostAsync(ApplicationState.PastebinLoginUrl, new FormUrlEncodedContent(loginRequest)).Result;
            if (!resp.IsSuccessStatusCode) return MakeError("login failed");
            var result = resp.Content.ReadAsStringAsync().Result;

            //valid session id's don't contain spaces, but other error responses do
            if (result.Contains(' ')) return MakeError("login failed");

            ApplicationState.Context[Field.SessionKey.MappedVal()] = result;
            return MakeSuccess("login successful!");
        }
    }
}