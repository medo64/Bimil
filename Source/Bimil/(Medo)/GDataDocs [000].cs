//Copyright (c) 2011 Josip Medved <jmedved@jmedved.com>

//2011-07-01: Initial version.

//http://code.google.com/apis/documents/docs/3.0/developers_guide_protocol.html


using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Medo.Web.GData {

    public class GDataDocs : IDisposable {

        public GDataDocs(string authorizationToken) {
            this.AuthorizationToken = authorizationToken;

            ServicePointManager.ServerCertificateValidationCallback += delegate(
                object sender,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors) {
                return true;
            };
        }


        public string AuthorizationToken { get; private set; }


        public IEnumerable<GDataDocsEntry> GetEntries() {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://docs.google.com/feeds/default/private/full");
            request.ProtocolVersion = HttpVersion.Version10;
            request.Headers.Add("GData-Version: 3.0");
            request.Headers.Add("Authorization: GoogleLogin auth=" + this.AuthorizationToken);
            request.Proxy = WebRequest.DefaultWebProxy;
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();

            string xml;
            using (var stream = new StreamReader(response.GetResponseStream())) {
                xml = stream.ReadToEnd();
            }

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode iNode in doc.SelectNodes("/*[local-name()='feed']/*[local-name()='entry']")) {
                var id = iNode.SelectSingleNode("./*[local-name()='id']").InnerText;
                var title = iNode.SelectSingleNode("./*[local-name()='title']").InnerText;
                var contentType = iNode.SelectSingleNode("./*[local-name()='content']").Attributes["type"].Value;
                var contentSource = iNode.SelectSingleNode("./*[local-name()='content']").Attributes["src"].Value;
                yield return new GDataDocsEntry(this, id, title, contentType, contentSource);
            }
        }


        public string GetRawFeedContent() {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://docs.google.com/feeds/default/private/full");
            request.ProtocolVersion = HttpVersion.Version10;
            request.Headers.Add("GData-Version: 3.0");
            request.Headers.Add("Authorization: GoogleLogin auth=" + this.AuthorizationToken);
            request.Proxy = WebRequest.DefaultWebProxy;
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;




            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();

            using (var stream = new StreamReader(response.GetResponseStream())) {
                return stream.ReadToEnd();
            }
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

    }



    public class GDataDocsEntry {

        public GDataDocsEntry(GDataDocs parent, string id, string title, string contentType, string contentSource) {
            this.Parent = parent;
            this.Id = id;
            this.Title = title;
            this.ContentType = contentType;
            this.ContentSource = contentSource;
        }


        public GDataDocs Parent { get; private set; }
        public string Id { get; private set; }
        public string Title { get; private set; }
        public string ContentType { get; private set; }
        public string ContentSource { get; private set; }

        public byte[] GetContent() {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.ContentSource);
            request.ProtocolVersion = HttpVersion.Version10;
            request.Headers.Add("GData-Version: 3.0");
            request.Headers.Add("Authorization: GoogleLogin auth=" + this.Parent.AuthorizationToken);
            request.Proxy = WebRequest.DefaultWebProxy;
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();

            using (var memoryStream = new MemoryStream(1024*1024))
            using (var stream = response.GetResponseStream()) {
                byte[] buffer = new byte[1024];
                int bytes;
                while ((bytes = stream.Read(buffer, 0, buffer.Length)) > 0) {
                    memoryStream.Write(buffer, 0, bytes);
                }
                return memoryStream.ToArray();
            }
        }

    }

}
