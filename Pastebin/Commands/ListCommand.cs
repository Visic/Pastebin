using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Pastebin {
    public class ListCommand : CommandBase {
        public override IEnumerable<string> Args() {
            return new string[] { "Optional: limit 1-1000", "Optional: title", "Optional: format" };
        }

        public override string Name() {
            return "List";
        }

        public override string Description() {
            return "Gets a list of up to [limit] Pastes which have a title that contains [title] and displays the result as [format].\n" +
                   "If [limit] is not specified, the default is 50 results.\n" +
                   "If [title] is not specified, it will not be used as a filter.\n" +
                   "If [format] is not specified, the Format in your current Settings will be used.";
        }

        public override Union<Success, Error> Cmd(string[] args) {
            var httpClient = new HttpClient();
            var maybeLimit = GetParamAs<string>(args, 0).Type1Value;
            var maybeTitle = GetParamAs<string>(args, 1).Type1Value;
            var maybeFormat = GetParamAs<string>(args, 2).Type1Value;

            if (!VerifyLimit(maybeLimit)) {
                if (maybeTitle.IsNone) {
                    maybeTitle = maybeLimit;
                    maybeLimit = new Option<string>("1000");
                } else if (maybeFormat.IsNone) {
                    maybeFormat = maybeTitle;
                    maybeTitle = maybeLimit;
                    maybeLimit = new Option<string>("1000");
                } else {
                    return MakeError("Invalid limit: {0}", maybeLimit.Value);
                }
            }

            var maybeSessionKey = ApplicationState.Context.TryGetValue(Field.SessionKey.MappedVal());
            if (maybeSessionKey.IsNone) return MakeError("You must be logged in to perform this action.");

            var request = new Dictionary<string, string>() { 
                {Field.DevKey.MappedVal(), ApplicationState.Context[Field.DevKey.MappedVal()]},
                {Field.ApiOption.MappedVal(), ApiFunction.List.MappedVal()},
                {Field.SessionKey.MappedVal(), maybeSessionKey.Value}
            };

            if (maybeLimit.IsSome) request.Add(Field.ResultsLimit.MappedVal(), maybeLimit.Value);

            var result = httpClient.PostAsync(ApplicationState.PastebinPostUrl, new FormUrlEncodedContent(request)).Result;
            if (!result.IsSuccessStatusCode) return MakeError(result.ReasonPhrase);

            var format = maybeFormat.IsSome ? maybeFormat.Value : ApplicationState.Context[Field.ListFormat.MappedVal()];
            var wrappedXml = string.Format("<results>{0}</results>", result.Content.ReadAsStringAsync().Result);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(wrappedXml))) {
                var relevantFields = new XmlHandler(stream).GetFromUri("paste", GetRelevantUris(format).Prepend(ListField.Title.MappedVal()).Distinct().ToArray());

                //turn the field type name into our name for it and then 
                //use the mappedTranslationFunction to turn the value into our value
                var translatedValues = relevantFields.Where(x => maybeTitle.IsNone || x[ListField.Title.MappedVal()].Contains(maybeTitle.Value))
                                                     .Select(
                                                         x => x.ToDictionary(
                                                             y => ListField.ReverseMap(y.Key).ToString(),
                                                             y => ListField.MappedTranslationFunc(ListField.ReverseMap(y.Key))(y.Value)
                                                         )
                                                     );

                if (translatedValues.Count() == 0) return MakeError("There were no matching pastes");

                //return the translated values in the correct format
                return MakeSuccess(
                    translatedValues.Select(
                        x => x.MoveLast(y => y.Key == ListField.Title.ToString()).Aggregate(
                            format,
                            (acc, y) => acc.Replace_IgnoreCase(y.Key, y.Value)
                        )
                    ).ToDelimitedString("\n")
                );
            }
        }

        private static bool VerifyLimit(Option<string> maybeLimit) {
            if (maybeLimit.IsNone) return true;

            int limit = -1;
            if (!int.TryParse(maybeLimit.Value, out limit)) return false;

            return limit > 0 && limit < 1001;
        }

        //regex match all the keys out of the ListFormat and return their enum mapped values
        private IEnumerable<string> GetRelevantUris(string format) {
            //key1|key2|key3
            var keyMatch = ListField.GetNames().ToDelimitedString("|");
            foreach (Match match in Regex.Matches(format, keyMatch, RegexOptions.IgnoreCase)) {
                yield return ListField.MappedVal(match.Value).ToString();
            }
        }
    }
}