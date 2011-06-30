//Copyright (c) 2010 Josip Medved <jmedved@jmedved.com>

//2010-05-24: Initial version.
//2011-06-30: Compatible with .NET Framework 2.0.


using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace Medo.Web.GData {

    /// <summary>
    /// Retrieving authenticator for Google API services.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "ClientLogin is term used by Google.")]
    public class GDataClientLogin : IDisposable {

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="email">User's e-mail.</param>
        /// <param name="password">User's password.</param>
        public GDataClientLogin(string email, string password)
            : this(email, password, GDataAccountTypes.Hosted | GDataAccountTypes.Google) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="email">User's e-mail.</param>
        /// <param name="password">User's password.</param>
        /// <param name="accountType">Determines which account type is to be used.</param>
        /// <exception cref="System.ArgumentNullException">E-mail cannot be null. -or- Password cannot be null. -or- At least one account type must be specified.</exception>
        public GDataClientLogin(string email, string password, GDataAccountTypes accountType) {
            if (email == null) { throw new ArgumentNullException("email", "E-mail cannot be null."); }
            if (password == null) { throw new ArgumentNullException("password", "Password cannot be null."); }
            if (accountType == 0) { throw new ArgumentOutOfRangeException("accountType", "At least one account type must be specified."); }

            if (!email.Contains("@")) { email += "@gmail.com"; } //for compatibility with gmail user names.

            this.Email = email;
            this.Password = password;
            this.AccountType = accountType;
            this.Source = GetDefaultSource();
        }


        /// <summary>
        /// Gets user's e-mail.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Gets user's password.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Get user's account type.
        /// If both Google and Hosted are specified, Hosted takes precedence.
        /// </summary>
        public GDataAccountTypes AccountType { get; private set; }

        /// <summary>
        /// Short source text.
        /// </summary>
        public string Source { get; private set; }


        /// <summary>
        /// Returns authorization token for Google Analytics Data APIs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetAnalyticsDataAuthorizationToken() {
            return GetAuthorizationToken("analytics");
        }

        /// <summary>
        /// Returns authorization token for Google Apps Provisioning APIs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetAppsProvisioningAuthorizationToken() {
            return GetAuthorizationToken("apps");
        }

        /// <summary>
        /// Returns authorization token for Google Base Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetBaseDataAuthorizationToken() {
            return GetAuthorizationToken("gbase");
        }

        /// <summary>
        /// Returns authorization token for Google Sites Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetSitesDataAuthorizationToken() {
            return GetAuthorizationToken("jotspot");
        }

        /// <summary>
        /// Returns authorization token for Blogger Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetBloggerDataAuthorizationToken() {
            return GetAuthorizationToken("blogger");
        }

        /// <summary>
        /// Returns authorization token for Book Search Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetBookSearchDataAuthorizationToken() {
            return GetAuthorizationToken("print");
        }

        /// <summary>
        /// Returns authorization token for Calendar Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetCalendarDataAuthorizationToken() {
            return GetAuthorizationToken("cl");
        }

        /// <summary>
        /// Returns authorization token for Google Code Search Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetCodeSearchDataAuthorizationToken() {
            return GetAuthorizationToken("codesearch");
        }

        /// <summary>
        /// Returns authorization token for Contacts Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetContactsDataAuthorizationToken() {
            return GetAuthorizationToken("cp");
        }

        /// <summary>
        /// Returns authorization token for Documents List Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetDocumentsListDataAuthorizationToken() {
            return GetAuthorizationToken("writely");
        }

        /// <summary>
        /// Returns authorization token for Finance Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetFinanceDataAuthorizationToken() {
            return GetAuthorizationToken("finance");
        }

        /// <summary>
        /// Returns authorization token for Gmail Atom feed.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetGmailAtomFeedAuthorizationToken() {
            return GetAuthorizationToken("mail");
        }

        /// <summary>
        /// Returns authorization token for Health Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetHealthDataAuthorizationToken() {
            return GetAuthorizationToken("health");
        }

        /// <summary>
        /// Returns authorization token for Health Data API (H9 sandbox).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetHealthDataH9AuthorizationToken() {
            return GetAuthorizationToken("weaver");
        }

        /// <summary>
        /// Returns authorization token for Maps Data APIs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetMapsDataAuthorizationToken() {
            return GetAuthorizationToken("local");
        }

        /// <summary>
        /// Returns authorization token for Picasa Web Albums Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetPicasaWebAlbumsDataAuthorizationToken() {
            return GetAuthorizationToken("lh2");
        }

        /// <summary>
        /// Returns authorization token for Sidewiki Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sidewiki", Justification = "Identifier is spelled as it was desired.")]
        public string GetSidewikiDataAuthorizationToken() {
            return GetAuthorizationToken("annotateweb");
        }

        /// <summary>
        /// Returns authorization token for Spreadsheets Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetSpreadsheetsDataAuthorizationToken() {
            return GetAuthorizationToken("wise");
        }

        /// <summary>
        /// Returns authorization token for Webmaster Tools API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetWebmasterToolsAuthorizationToken() {
            return GetAuthorizationToken("wise");
        }

        /// <summary>
        /// Returns authorization token for YouTube Data API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method takes long to execute and thus is not appropriate candidate for property.")]
        public string GetYouTubeDataAuthorizationToken() {
            return GetAuthorizationToken("youtube");
        }


        /// <summary>
        /// Returns authorization token.
        /// </summary>
        /// <param name="service">Service designator.</param>
        /// <exception cref="System.InvalidOperationException">Authorization failed. -or- Authorization token was not received.</exception>
        public string GetAuthorizationToken(string service) {
            StringBuilder postData = new StringBuilder();
            if ((((this.AccountType & GDataAccountTypes.Hosted) == GDataAccountTypes.Hosted) && ((this.AccountType & GDataAccountTypes.Google) == GDataAccountTypes.Google))) {
                postData.AppendFormat(CultureInfo.InvariantCulture, "accountType={0}", Uri.EscapeUriString("HOSTED_OR_GOOGLE"));
            } else if ((this.AccountType & GDataAccountTypes.Hosted) == GDataAccountTypes.Hosted) {
                postData.AppendFormat(CultureInfo.InvariantCulture, "accountType={0}", Uri.EscapeUriString("HOSTED"));
            } else if ((this.AccountType & GDataAccountTypes.Google) == GDataAccountTypes.Google) {
                postData.AppendFormat(CultureInfo.InvariantCulture, "accountType={0}", Uri.EscapeUriString("GOOGLE "));
            }

            postData.AppendFormat(CultureInfo.InvariantCulture, "&Email={0}", Uri.EscapeUriString(this.Email));
            postData.AppendFormat(CultureInfo.InvariantCulture, "&Passwd={0}", Uri.EscapeUriString(this.Password));
            if (service != null) {
                postData.AppendFormat(CultureInfo.InvariantCulture, "&service={0}", Uri.EscapeUriString(service));
            }
            postData.AppendFormat(CultureInfo.InvariantCulture, "&source={0}", Uri.EscapeUriString(this.Source));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("https://www.google.com/accounts/ClientLogin"));
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.Proxy = WebRequest.DefaultWebProxy;
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;

            byte[] postDataBytes = Encoding.UTF8.GetBytes(postData.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataBytes.Length;
            request.GetRequestStream().Write(postDataBytes, 0, postDataBytes.Length);


            HttpWebResponse response;
            try {
                response = (HttpWebResponse)request.GetResponse();
            } catch (WebException ex) {
                throw new InvalidOperationException("Authorization failed.", ex);
            }

            using (var stream = new StreamReader(response.GetResponseStream())) {
                while (!stream.EndOfStream) {
                    string line = stream.ReadLine();
                    if (line.StartsWith("Auth=", StringComparison.Ordinal)) {
                        return line.Substring(5);
                    }
                }
            }

            throw new System.InvalidOperationException("Authorization token was not received.");
        }


        #region IDisposable Members

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing) {
        }

        #endregion


        private static string GetDefaultSource() {
            Assembly assembly = Assembly.GetEntryAssembly();
            AssemblyName assemblyName = Assembly.GetEntryAssembly().GetName();

            string companyText;
            object[] companyAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
            if ((companyAttributes != null) && (companyAttributes.Length >= 1)) {
                companyText = GetCamelCased(((AssemblyCompanyAttribute)companyAttributes[companyAttributes.Length - 1]).Company);
            } else {
                companyText = "";
            }

            string productText;
            object[] productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
            if ((productAttributes != null) && (productAttributes.Length >= 1)) {
                productText = GetCamelCased(((AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product);
            } else {
                object[] titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
                if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                    productText = GetCamelCased(((AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title);
                } else {
                    productText = GetCamelCased(assemblyName.Name);
                }
            }

            return companyText + "-" + productText + "-" + assemblyName.Version.ToString(2);
        }

        private static string GetCamelCased(string text) {
            var sb = new StringBuilder();
            bool capitalizeNextChar = true;
            foreach (char c in text) {
                if (Char.IsLetterOrDigit(c)) {
                    if (capitalizeNextChar) {
                        capitalizeNextChar = false;
                        sb.Append(Char.ToUpperInvariant(c));
                    } else {
                        sb.Append(c);
                    }
                } else {
                    capitalizeNextChar = true;
                }
            }
            return sb.ToString();
        }

    }





    /// <summary>
    /// Google account type.
    /// </summary>
    [Flags()]
    public enum GDataAccountTypes {
        /// <summary>
        /// Authorization for a google account.
        /// </summary>
        Google = 1,
        /// <summary>
        /// Authorization for a hosted account.
        /// </summary>
        Hosted = 2,
    }

}