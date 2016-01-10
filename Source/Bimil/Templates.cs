using System.Collections.Generic;
using Medo.Security.Cryptography.Bimil;

namespace Bimil {
    internal static class Templates {

        private static Template[] TemplatesCache = null;

        public static Template[] GetTemplates() {
            if (Templates.TemplatesCache == null) {
                var list = new List<Template>();

                list.Add(new Template("User name and password", "User name", BimilRecordFormat.Text, "Password", BimilRecordFormat.Password, "URL", BimilRecordFormat.Url, "Notes", BimilRecordFormat.MultilineText));
                list.Add(new Template("Simple Password", "Password", BimilRecordFormat.Password, "Notes", BimilRecordFormat.MultilineText));
                list.Add(new Template("Credit card", "Card number", BimilRecordFormat.Text, "Expiration date", BimilRecordFormat.Text, "Security code", BimilRecordFormat.Password, "PIN", BimilRecordFormat.Password, "Notes", BimilRecordFormat.MultilineText));
                list.Add(new Template("Bank card", "Card number", BimilRecordFormat.Text, "Expiration date", BimilRecordFormat.Text, "Account number", BimilRecordFormat.Text, "PIN", BimilRecordFormat.Password, "Notes", BimilRecordFormat.MultilineText));

                Templates.TemplatesCache = list.ToArray();
            }
            return Templates.TemplatesCache;
        }

    }



    internal class Template {

        internal Template(string title, params object[] keysAndFormats) {
            this.Title = title;
            this.Records = new List<KeyValuePair<string, BimilRecordFormat>>();
            for (int i = 0; i < keysAndFormats.Length; i += 2) {
                this.Records.Add(new KeyValuePair<string, BimilRecordFormat>((string)keysAndFormats[i], (BimilRecordFormat)keysAndFormats[i + 1]));
            }
        }

        public string Title { get; private set; }
        public IList<KeyValuePair<string, BimilRecordFormat>> Records { get; private set; }

        public override string ToString() {
            return this.Title;
        }

    }

}
