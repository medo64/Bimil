using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Bimil {
    internal static class Hibp {

        public static IList<HibpBreach> GetAllBreaches(string account) {
            var url = "https://haveibeenpwned.com/api/v2/breachedaccount/" + System.Web.HttpUtility.UrlEncode(account);
            try {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2 - works only if .NET framework 4.5 is installed
                using (var client = new WebClient()) {
                    client.Headers["User-Agent"] = Medo.Reflection.EntryAssembly.Product + "/" + Medo.Reflection.EntryAssembly.Version.ToString();
                    var text = client.DownloadString(url);

                    var serializer = new DataContractJsonSerializer(typeof(HibpBreach[]));
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(text))) {
                        ms.Position = 0;
                        var breaches = serializer.ReadObject(ms) as HibpBreach[];
                        return new List<HibpBreach>(breaches).AsReadOnly();
                    }
                }
            } catch (WebException) {
                throw;
            }
        }

        public static bool IsPassworPwned(string passwordHash) {
            var url = "https://haveibeenpwned.com/api/v2/pwnedpassword/" + System.Web.HttpUtility.UrlEncode(passwordHash);
            try {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2 - works only if .NET framework 4.5 is installed
                using (var client = new WebClient()) {
                    client.Headers["User-Agent"] = Medo.Reflection.EntryAssembly.Product + "/" + Medo.Reflection.EntryAssembly.Version.ToString();
                    client.DownloadString(url);
                    return true;
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


    [DebuggerDisplay("Title")]
    [DataContract]
    internal struct HibpBreach {
        [DataMember] internal string Name;
        [DataMember] internal string Title;
        [DataMember] internal string Domain;
        [DataMember] internal string BreachDate;
        [DataMember] internal string Description;
        [DataMember] internal bool IsVerified;
        [DataMember] internal bool IsFabricated;
        internal bool IsApplicable(string url, DateTime lastPasswordModification, string title) {
            if (this.IsVerified && !this.IsFabricated) {
                if (!string.IsNullOrEmpty(this.Domain)) { //check only if we know the domain
                    if (DateTime.TryParseExact(this.BreachDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var breachDate)) { //only check if we do have a breach date
                        //check domain
                        if (url.IndexOf(this.Domain, StringComparison.OrdinalIgnoreCase) >= 0) { //check only if URL matches domain
                            return (breachDate > lastPasswordModification);
                        }

                        //check title - fuzzy
                        var domainComponents = this.Domain.Split('.');
                        var mainDomainComponent = (domainComponents.Length > 1) ? domainComponents[domainComponents.Length - 2] : null; //take only middle component of domain name
                        if (!string.IsNullOrEmpty(mainDomainComponent)) {
                            if (title.IndexOf(mainDomainComponent, StringComparison.OrdinalIgnoreCase) >= 0) { //only part of domain has to match title
                                return (breachDate > lastPasswordModification);
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
