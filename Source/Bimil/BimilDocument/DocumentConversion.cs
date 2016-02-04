using System;
using Medo.Security.Cryptography.PasswordSafe;
using BimilDocument = Medo.Security.Cryptography.Bimil;
using System.Diagnostics;

namespace Bimil {
    internal static class DocumentConversion {

        internal static Document ConvertFromBimil(BimilDocument.BimilDocument legacyDoc, string password) {
            var doc = new Document(password);

            foreach (var item in legacyDoc.Items) {
                var entry = new Entry();
                entry.Title = item.Name;
                entry.Group = item.CategoryRecord.Value.Text;
                entry[RecordType.Password] = null;

                foreach (var record in item.Records) {
                    if (record.Format == BimilDocument.BimilRecordFormat.System) { continue; }
                    var newRecord = ConvertBimilRecord(record);
                    if (newRecord != null) {
                        if ((newRecord.RecordType == 0) || entry.Records.Contains(newRecord.RecordType)) {
                            if (entry.Notes.Length > 0) { entry.Notes += "\r\n"; }
                            entry.Notes += newRecord.Text;
                        } else {
                            entry.Records.Add(newRecord);
                        }
                    }
                }

                if (entry.Records.Contains(RecordType.Notes)) {
                    var notes = entry.Records[RecordType.Notes];
                    entry.Records[RecordType.Notes] = null;
                    entry.Records.Add(notes);
                }

                doc.Entries.Add(entry);
            }

            return doc;
        }

        private static Record ConvertBimilRecord(BimilDocument.BimilRecord record) {
            if (record.Key.Text.Equals("User name", StringComparison.OrdinalIgnoreCase)
                || record.Key.Text.Equals("Username", StringComparison.OrdinalIgnoreCase)
                || record.Key.Text.Equals("User", StringComparison.OrdinalIgnoreCase)) {
                return new Record(RecordType.UserName) { Text = record.Value.Text };
            } else if (record.Key.Text.Equals("Password", StringComparison.OrdinalIgnoreCase)) {
                return new Record(RecordType.Password) { Text = record.Value.Text };
            } else if (record.Key.Text.Equals("Notes", StringComparison.OrdinalIgnoreCase)) {
                return new Record(RecordType.Notes) { Text = record.Value.Text };
            } else if (record.Key.Text.Equals("URL", StringComparison.OrdinalIgnoreCase)
                || record.Key.Text.Equals("Web address", StringComparison.OrdinalIgnoreCase)) {
                return new Record(RecordType.Url) { Text = record.Value.Text };
            } else if (record.Key.Text.Equals("Key", StringComparison.OrdinalIgnoreCase)) {
                return new Record(RecordType.BimilTwoFactorKey) { Text = record.Value.Text };
            } else if (record.Key.Text.Equals("Card number", StringComparison.OrdinalIgnoreCase)) {
                return new Record(RecordType.BimilCreditCardNumber) { Text = record.Value.Text };
            } else if (record.Key.Text.Equals("Expiration date", StringComparison.OrdinalIgnoreCase)) {
                return new Record(RecordType.BimilCreditCardExpiration) { Text = record.Value.Text };
            } else if (record.Key.Text.Equals("Security code", StringComparison.OrdinalIgnoreCase)
                || record.Key.Text.Equals("CVV", StringComparison.OrdinalIgnoreCase)
                || record.Key.Text.Equals("CVV2", StringComparison.OrdinalIgnoreCase)
                || record.Key.Text.Equals("CID", StringComparison.OrdinalIgnoreCase)
                || record.Key.Text.Equals("CSC", StringComparison.OrdinalIgnoreCase)) {
                return new Record(RecordType.BimilCreditCardSecurityCode) { Text = record.Value.Text };
            } else if (record.Key.Text.Equals("PIN", StringComparison.OrdinalIgnoreCase)) {
                return new Record(RecordType.BimilCreditCardPin) { Text = record.Value.Text };
            } else {
                Debug.WriteLine("Not auto-converting field \"" + record.Key.Text + "\"");
                if (record.Value.Text.Length > 0) {
                    return new Record(0) { Text = record.Key.Text + ": " + record.Value.Text };
                } else {
                    return null;
                }
            }
        }

    }
}
