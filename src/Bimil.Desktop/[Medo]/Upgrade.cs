/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2023-03-27: Removed GUI component
//            Added async methods
//2021-11-08: Refactored for .NET 6
//            Version uses only first three components (no revision)
//2019-11-16: Allowing for TLS 1.2 and 1.3 where available
//2015-12-31: Allowing for 301 redirect
//2013-12-28: Message box adjustments
//2012-03-13: UI adjustments.
//2012-03-05: Initial version

namespace Medo.Application;

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Handles upgrade procedure.
/// </summary>
/// <remarks>
/// On server side, query will be done to the following address (baseurl)/(app)/(version)/.
/// The following responses are valid:
/// * 204: No upgrade available (future use)
/// * 303: Upgrade available
/// * 403: No upgrade available (previous version)
/// * 500: Server error
/// </remarks>
public static class Upgrade {

    /// <summary>
    /// Returns upgrade file if there is one or null if there is no upgrade.
    /// </summary>
    /// <param name="serviceUrl">Service URL.</param>
    /// <exception cref="ArgumentNullException">Service URL cannot be null.</exception>
    /// <exception cref="InvalidDataException">Cannot check for upgrade at this time.</exception>
    public static UpgradeFile? GetUpgradeFile(Uri serviceUrl) {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        return GetUpgradeFile(serviceUrl, assembly);
    }

    /// <summary>
    /// Returns upgrade file if there is one or null if there is no upgrade.
    /// </summary>
    /// <param name="serviceUrl">Service URL.</param>
    /// <exception cref="ArgumentNullException">Service URL cannot be null.</exception>
    /// <exception cref="InvalidDataException">Cannot check for upgrade at this time.</exception>
    public static async Task<UpgradeFile?> GetUpgradeFileAsync(Uri serviceUrl) {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        return await GetUpgradeFileAsync(serviceUrl, assembly);
    }

    /// <summary>
    /// Returns upgrade file if there is one or null if there is no upgrade.
    /// </summary>
    /// <param name="serviceUrl">Service URL.</param>
    /// <exception cref="ArgumentNullException">Service URL cannot be null. -or- Assembly cannot be null.</exception>
    /// <exception cref="InvalidDataException">Cannot check for upgrade at this time.</exception>
    public static UpgradeFile? GetUpgradeFile(Uri serviceUrl, Assembly assembly) {
        if (assembly == null) { throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null."); }
        var applicationName = GetProduct(assembly);
        var applicationVersion = assembly.GetName().Version ?? new Version(0, 0, 0, 0);
        return GetUpgradeFile(serviceUrl, applicationName, applicationVersion);
    }

    /// <summary>
    /// Returns upgrade file if there is one or null if there is no upgrade.
    /// </summary>
    /// <param name="serviceUrl">Service URL.</param>
    /// <exception cref="ArgumentNullException">Service URL cannot be null. -or- Assembly cannot be null.</exception>
    /// <exception cref="InvalidDataException">Cannot check for upgrade at this time.</exception>
    public static async Task<UpgradeFile?> GetUpgradeFileAsync(Uri serviceUrl, Assembly assembly) {
        if (assembly == null) { throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null."); }
        var applicationName = GetProduct(assembly);
        var applicationVersion = assembly.GetName().Version ?? new Version(0, 0, 0, 0);
        return await GetUpgradeFileAsync(serviceUrl, applicationName, applicationVersion);
    }

    /// <summary>
    /// Returns upgrade file if there is one or null if there is no upgrade.
    /// </summary>
    /// <param name="serviceUri">Service URL (e.g. https://medo64.com/upgrade/).</param>
    /// <param name="applicationName">Application name.</param>
    /// <param name="applicationVersion">Application version.</param>
    /// <exception cref="ArgumentNullException">Service URL cannot be null. -or- Application name cannot be null.</exception>
    /// <exception cref="InvalidDataException">Cannot check for upgrade at this time.</exception>
    public static UpgradeFile? GetUpgradeFile(Uri serviceUri, string applicationName, Version applicationVersion) {
        if (serviceUri == null) { throw new ArgumentNullException(nameof(serviceUri), "Service URL cannot be null."); }
        if (applicationName == null) { throw new ArgumentNullException(nameof(applicationName), "Application name cannot be null."); }

        var url = GetUrl(serviceUri, applicationName, applicationVersion);

        try {
            return GetUpgradeFileFromUrl(url);
        } catch (InvalidDataException ex) {  // rewrap exception
            throw new InvalidDataException("Cannot check for upgrade at this time.", ex);
        }
    }

    /// <summary>
    /// Returns upgrade file if there is one or null if there is no upgrade.
    /// </summary>
    /// <param name="serviceUri">Service URL (e.g. https://medo64.com/upgrade/).</param>
    /// <param name="applicationName">Application name.</param>
    /// <param name="applicationVersion">Application version.</param>
    /// <exception cref="ArgumentNullException">Service URL cannot be null. -or- Application name cannot be null.</exception>
    /// <exception cref="InvalidDataException">Cannot check for upgrade at this time.</exception>
    public static async Task<UpgradeFile?> GetUpgradeFileAsync(Uri serviceUri, string applicationName, Version applicationVersion) {
        if (serviceUri == null) { throw new ArgumentNullException(nameof(serviceUri), "Service URL cannot be null."); }
        if (applicationName == null) { throw new ArgumentNullException(nameof(applicationName), "Application name cannot be null."); }

        var url = GetUrl(serviceUri, applicationName, applicationVersion);

        try {
            return await GetUpgradeFileFromUrlAsync(url);
        } catch (InvalidDataException ex) {  // rewrap exception
            throw new InvalidDataException("Cannot check for upgrade at this time.", ex);
        }
    }

    private static readonly Lazy<HttpClient> NoRedirectClient = new(() =>
        new HttpClient(new HttpClientHandler() {
            AllowAutoRedirect = false,
        })
    );

    private static UpgradeFile? GetUpgradeFileFromUrl(string url, int redirectCount = 0) {
        var client = NoRedirectClient.Value;
        using var request = new HttpRequestMessage(HttpMethod.Head, url) {
            Version = HttpVersion.Version11,  // we need to use async for 2.0 and above
            VersionPolicy = HttpVersionPolicy.RequestVersionOrLower,
        };
        using var response = client.Send(request);

        try {
            return ParseResponse(response, ref redirectCount);
        } catch (InvalidDataException) {
            throw;
        } catch (Exception ex) {
            throw new InvalidDataException(ex.Message, ex);
        }
    }

    private static async Task<UpgradeFile?> GetUpgradeFileFromUrlAsync(string url, int redirectCount = 0) {
        var client = NoRedirectClient.Value;
        using var request = new HttpRequestMessage(HttpMethod.Head, url) {
            Version = HttpVersion.Version20,
            VersionPolicy = HttpVersionPolicy.RequestVersionOrLower,
        };
        using var response = await client.SendAsync(request);

        try {
            return ParseResponse(response, ref redirectCount);
        } catch (InvalidDataException) {
            throw;
        } catch (Exception ex) {
            throw new InvalidDataException(ex.Message, ex);
        }
    }

    private static string GetUrl(Uri serviceUri, string applicationName, Version applicationVersion) {
        var url = new StringBuilder();
        url.Append(serviceUri.AbsoluteUri);
        if (!serviceUri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)) { url.Append('/'); }
        foreach (var ch in applicationName.Normalize(NormalizationForm.FormD)) {  // lowecase application name
            if (char.IsLetterOrDigit(ch)) { url.Append(char.ToLowerInvariant(ch)); }
        }
        url.Append('/');
        url.Append(applicationVersion.ToString(3));
        url.Append('/');
        return url.ToString();
    }

    private static UpgradeFile? ParseResponse(HttpResponseMessage response, ref int redirectCount) {
        if (response.StatusCode is HttpStatusCode.SeeOther) {  // 303: upgrade available
            var location = response.Headers.Location ?? throw new InvalidDataException("Cannot find upgrade.");
            return new UpgradeFile(location); //upgrade at Location
        } else if (response.StatusCode is HttpStatusCode.NoContent or HttpStatusCode.Forbidden) {  // 204: no upgrade is available (403 was used before)
            return null; //no upgrade
        } else if (response.StatusCode is HttpStatusCode.MovedPermanently) {
            if (redirectCount > 5) { throw new InvalidDataException("Redirect loop."); }
            var location = response.Headers.Location ?? throw new InvalidDataException("Invalid redirect.");
            return GetUpgradeFileFromUrl(location.ToString(), redirectCount++); //follow 301 redirect
        } else {
            throw new InvalidDataException("Unexpected answer from upgrade server.");
        }
    }

    private static string GetProduct(Assembly assembly) {
        var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
        if ((productAttributes != null) && (productAttributes.Length >= 1)) {
            return ((AssemblyProductAttribute)productAttributes[^1]).Product;
        } else {
            var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
            if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                return ((AssemblyTitleAttribute)titleAttributes[^1]).Title;
            } else {
                return assembly.GetName().Name ?? "unknown";
            }
        }
    }

}



/// <summary>
/// Handles upgrade file operations.
/// </summary>
public sealed class UpgradeFile {

    internal UpgradeFile(Uri uri) {
        Url = uri;
    }


    /// <summary>
    /// Gets upgrade file URL.
    /// </summary>
    public Uri Url { get; private set; }

    /// <summary>
    /// Gets file name.
    /// </summary>
    public string FileName { get { return Url.Segments[^1]; } }


    private static readonly Lazy<HttpClient> HttpClient = new(() => new HttpClient());

    /// <summary>
    /// Returns content stream.
    /// </summary>
    /// <exception cref="InvalidDataException">Invalid data.</exception>
    public async Task<Stream> GetStreamAsync() {
        var client = HttpClient.Value;
        var response = await client.GetAsync(Url);
        if (!response.IsSuccessStatusCode) { throw new InvalidDataException(((int)response.StatusCode).ToString(CultureInfo.InvariantCulture) + ": " + response.ReasonPhrase ?? "Unknown response" + "."); }
        ContentLength = (int?)response.Content.Headers.ContentLength;  // realistically 2GB is enough
        return await response.Content.ReadAsStreamAsync();
    }

    /// <summary>
    /// Returns content stream.
    /// </summary>
    /// <exception cref="InvalidDataException">Invalid data.</exception>
    public Stream GetStream() {
        var client = HttpClient.Value;
        using var request = new HttpRequestMessage(HttpMethod.Get, Url);
        var response = client.Send(request);
        if (!response.IsSuccessStatusCode) { throw new InvalidDataException(((int)response.StatusCode).ToString(CultureInfo.InvariantCulture) + ": " + response.ReasonPhrase ?? "Unknown response" + "."); }
        ContentLength = (int?)response.Content.Headers.ContentLength;  // realistically 2GB is enough
        return response.Content.ReadAsStream();
    }

    /// <summary>
    /// Gets the length of stream.
    /// Valid only once stream has been read.
    /// </summary>
    private int? ContentLength { get; set; }

    /// <summary>
    /// Retrieves the whole file into a byte array.
    /// </summary>
    /// <exception cref="InvalidDataException">Content length mismatch.</exception>
    public byte[] DownloadData() {
        using var outputStream = new MemoryStream(ContentLength ?? 4 * 1024 * 1024);  // reserve 4 MB if no better info
        CopyToStreamWithProgress(outputStream);
        return outputStream.ToArray();
    }

    /// <summary>
    /// Retrieves the whole file into a byte array.
    /// </summary>
    /// <exception cref="InvalidDataException">Content length mismatch.</exception>
    public async Task<byte[]> DownloadDataAsync() {
        using var outputStream = new MemoryStream(ContentLength ?? 4 * 1024 * 1024);  // reserve 4 MB if no better info
        await CopyToStreamWithProgressAsync(outputStream);
        return outputStream.ToArray();
    }


    /// <summary>
    /// Retrieves the whole file and returns FileInfo or null if download fails.
    /// </summary>
    /// <exception cref="InvalidDataException">Content length mismatch.</exception>
    public FileInfo? DownloadFile() {
        var fileName = Path.Combine(Path.GetTempPath(), FileName);
        if (DownloadFile(fileName)) {
            return new FileInfo(fileName);
        } else {
            return null;
        }
    }

    /// <summary>
    /// Retrieves the whole file and returns FileInfo or null if download fails.
    /// </summary>
    /// <exception cref="InvalidDataException">Content length mismatch.</exception>
    public async Task<FileInfo?> DownloadFileAsync() {
        var fileName = Path.Combine(Path.GetTempPath(), FileName);
        if (await DownloadFileAsync(fileName)) {
            return new FileInfo(fileName);
        } else {
            return null;
        }
    }


    /// <summary>
    /// Retrieves the whole file and returns if operation was successful.
    /// </summary>
    /// <exception cref="InvalidDataException">Content length mismatch.</exception>
    public bool DownloadFile(string fileName) {
        FileStream? outputStream = null;
        bool succeeded = false;
        try {
            outputStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            CopyToStreamWithProgress(outputStream);
            succeeded = true;
        } finally {
            if (outputStream != null) {
                outputStream.Close();
                if (!succeeded) { File.Delete(fileName); }
            }
        }
        return succeeded;
    }

    /// <summary>
    /// Retrieves the whole file and returns if operation was successful.
    /// </summary>
    /// <exception cref="InvalidDataException">Content length mismatch.</exception>
    public async Task<bool> DownloadFileAsync(string fileName) {
        FileStream? outputStream = null;
        bool succeeded = false;
        try {
            outputStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            await CopyToStreamWithProgressAsync(outputStream);
            succeeded = true;
        } finally {
            if (outputStream != null) {
                outputStream.Close();
                if (!succeeded) { File.Delete(fileName); }
            }
        }
        return succeeded;
    }


    /// <summary>
    /// Raised for each reported progress value.
    /// </summary>
    /// <remarks>
    /// Used for DownloadData and DownloadFile methods.
    /// </remarks>
    public event EventHandler<ProgressChangedEventArgs>? ProgressChanged;


    private void CopyToStreamWithProgress(Stream destination) {
        using var source = GetStream();
        var buffer = new byte[4096];

        while (true) {
            var read = source.Read(buffer, 0, buffer.Length);
            if (read > 0) {
                destination.Write(buffer, 0, read);
                if (ContentLength != null) {  // don't raise progress if content length was not returned
                    var curLen = destination.Length;
                    var maxLen = ContentLength;
                    ProgressChanged?.Invoke(this,
                        new ProgressChangedEventArgs(
                            (int)(curLen * 100 / maxLen),
                            (curLen, maxLen)
                        ));
                }
            } else {
                break;
            }
        }
        if ((ContentLength is not null) && (destination.Length != ContentLength)) { throw new InvalidDataException("Content length mismatch."); }
    }

    private async Task CopyToStreamWithProgressAsync(Stream destination) {
        using var source = await GetStreamAsync();
        var buffer = new byte[4096];

        while (true) {
            var read = await source.ReadAsync(buffer);
            if (read > 0) {
                destination.Write(buffer, 0, read);
                if (ContentLength != null) {  // don't raise progress if content length was not returned
                    var curLen = destination.Length;
                    var maxLen = ContentLength;
                    ProgressChanged?.Invoke(this,
                        new ProgressChangedEventArgs(
                            (int)(curLen * 100 / maxLen),
                            (curLen, maxLen)
                        ));
                }
            } else {
                break;
            }
        }
        if ((ContentLength is not null) && (destination.Length != ContentLength)) { throw new InvalidDataException("Content length mismatch."); }
    }

}
