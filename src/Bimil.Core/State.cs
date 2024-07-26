namespace Bimil.Core;

using System;
using System.IO;
using System.Security;
using Medo.Security.Cryptography.PasswordSafe;

/// <summary>
/// State of the application.
/// </summary>
public static class State {

    /// <summary>
    /// Gets currently open document.
    /// </summary>
    public static Document? Document { get; private set; }

    /// <summary>
    /// Gets currently open file.
    /// </summary>
    public static FileInfo? File { get; private set; }

    /// <summary>
    /// Raised whenever state needs an update.
    /// </summary>
    public static event EventHandler<EventArgs>? StateChanged;

    /// <summary>
    /// Creates a new file.
    /// </summary>
    /// <param name="passphrase">Passphrase.</param>
    public static void NewFile(string passphrase) {
        Document = new Document(passphrase);
        File = null;
        StateChanged?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>
    /// Open an existing file.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="passphrase">Passphrase.</param>
    public static bool OpenFile(FileInfo file, string passphrase, bool @readonly) {
        Document = Document.Load(file.FullName, passphrase);
        Document.IsReadOnly = @readonly;
        File = file;

        StateChanged?.Invoke(null, EventArgs.Empty);
        return true;
    }

    public static void ForceStateChange() {
        StateChanged?.Invoke(null, EventArgs.Empty);
    }

}
