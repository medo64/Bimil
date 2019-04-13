using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Bimil {
    internal static class BadPasswords {

        public static IEnumerable<string> Passwords;

        public static bool IsCommon(string password, out string matchedPassword) {
            if (Settings.ShowCommonPasswordWarnings) {
                if (Passwords == null) {
                    var sw = Stopwatch.StartNew();

                    var passwords = new List<string>();
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Common.passwords"))
                    using (var textStream = new StreamReader(stream)) {
                        var items = textStream.ReadToEnd().Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in items) {
                            if (item.StartsWith("#", StringComparison.Ordinal)) { continue; } //omit comments
                            passwords.Add(item);
                        }
                    }
                    Passwords = passwords;

                    Debug.WriteLine($"Loaded {passwords.Count} common passwords in {sw.ElapsedMilliseconds} ms.");
                }

                var swVerify = Stopwatch.StartNew();
                try {
                    foreach (var commonPassword in Passwords) {
                        if (commonPassword.Length * 3 > password.Length) { //only check passwords over minimum length
                            if (password.IndexOf(commonPassword, StringComparison.OrdinalIgnoreCase) >= 0) {
                                matchedPassword = commonPassword;
                                return true;
                            }
                        }
                    }
                } finally {
                    Debug.WriteLine($"Common password search done in {swVerify.ElapsedMilliseconds} ms.");
                }
            }

            matchedPassword = null;
            return false;
        }

    }
}
