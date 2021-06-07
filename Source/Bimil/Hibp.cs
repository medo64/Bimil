using System;
using System.Net;

namespace Bimil {
    internal static class Hibp {

        public static bool IsPassworPwned(string passwordHash) {
            var sha1Prefix = passwordHash.Substring(0, 5);
            var url = "https://pwned.medo64.com/range/" + System.Web.HttpUtility.UrlEncode(sha1Prefix);
            try {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2 - works only if .NET framework 4.5 is installed
                using (var client = new WebClient()) {
                    client.Headers["User-Agent"] = Medo.Reflection.EntryAssembly.Product + "/" + Medo.Reflection.EntryAssembly.Version.ToString();
                    var response = client.DownloadString(url);
                    var lines = response.Split(new string[] { "\n", "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines) {
                        if (line.StartsWith(passwordHash, StringComparison.OrdinalIgnoreCase)) {  // to ignore : if one is present
                            return true;
                        }
                    }
                    return false;
                }
            } catch (WebException ex) {
                if ((ex.Response is HttpWebResponse response) && (response.StatusCode == HttpStatusCode.NotFound)) {
                    return false;
                } else {
                    throw;
                }
            }
        }

    }
}
