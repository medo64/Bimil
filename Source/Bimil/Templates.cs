using System.Collections.Generic;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal static class Templates {

        private static Template[] TemplatesCache = null;

        public static Template[] GetTemplates() {
            if (Templates.TemplatesCache == null) {
                var list = new List<Template> {
                    new Template("User name and password", RecordType.UserName, RecordType.Password, RecordType.Url, RecordType.Notes),
                    new Template("User name and password with 2FA", RecordType.UserName, RecordType.Password, RecordType.TwoFactorKey, RecordType.Url, RecordType.Notes),
                    new Template("Just password", RecordType.Password, RecordType.Notes),
#if !WINDOWS_STORE
                    new Template("Credit card", RecordType.CreditCardNumber, RecordType.CreditCardExpiration, RecordType.CreditCardVerificationValue, RecordType.CreditCardPin, RecordType.Notes),
#endif
                };

                Templates.TemplatesCache = list.ToArray();
            }
            return Templates.TemplatesCache;
        }

    }



    internal class Template {

        internal Template(string title, params RecordType[] recordTypes) {
            Title = title;
            RecordTypes = new List<RecordType>(recordTypes);
        }

        public string Title { get; private set; }
        public IList<RecordType> RecordTypes { get; private set; }

        public override string ToString() {
            return Title;
        }

    }

}
