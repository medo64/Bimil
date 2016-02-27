using System.Collections.Generic;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal static class Templates {

        private static Template[] TemplatesCache = null;

        public static Template[] GetTemplates() {
            if (Templates.TemplatesCache == null) {
                var list = new List<Template>();

                list.Add(new Template("User name and password", RecordType.UserName, RecordType.Password, RecordType.Url, RecordType.Notes));
                list.Add(new Template("User name and password with 2FA", RecordType.UserName, RecordType.Password, RecordType.TwoFactorKey, RecordType.Url, RecordType.Notes));
                list.Add(new Template("Just password", RecordType.Password, RecordType.Notes));
                list.Add(new Template("Credit card", RecordType.CreditCardNumber, RecordType.CreditCardExpiration, RecordType.CreditCardVerificationValue, RecordType.CreditCardPin, RecordType.Notes));
                //list.Add(new Template("Bank card", "Card number", BimilRecordFormat.Text, "Expiration date", BimilRecordFormat.Text, "Account number", BimilRecordFormat.Text, "PIN", BimilRecordFormat.Password, "Notes", BimilRecordFormat.MultilineText));

                Templates.TemplatesCache = list.ToArray();
            }
            return Templates.TemplatesCache;
        }

    }



    internal class Template {

        internal Template(string title, params RecordType[] recordTypes) {
            this.Title = title;
            this.RecordTypes = new List<RecordType>(recordTypes);
        }

        public string Title { get; private set; }
        public IList<RecordType> RecordTypes { get; private set; }

        public override string ToString() {
            return this.Title;
        }

    }

}
