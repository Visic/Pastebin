using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Pastebin {
    public class XmlHandler {
        XDocument _doc;

        public XmlHandler(Stream xmlContent) {
            _doc = XDocument.Load(xmlContent);
        }

        public List<Dictionary<string, string>> GetFromUri(string root, params string[] uri) {
            return _doc.Descendants(root).Select(
                       x => uri.Select(
                           curUri => new { key = curUri, value = Parse_Rec(x, curUri) }
                       ).ToDictionary(e => e.key, e => e.value)
                   ).ToList();
        }

        private string Parse_Rec(XElement ele, string uri) {
            if (ele == null) return null;
            string[] parts = uri.Split(new char[] { '/' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return (parts.Length == 1) ? (string)ele.Element(parts[0]) : Parse_Rec(ele.Element(parts[0]), parts[1]);
        }
    }
}