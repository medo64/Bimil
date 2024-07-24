/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2024-07-23: Rewritten for file-based storage

namespace Medo.Configuration;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

/// <summary>
/// Provides access to storing most recent files.
/// This class is thread-safe.
/// </summary>
/// <remarks>
/// File name is the same as name of the &lt;executable&gt;.recent.cfg under windows or .&lt;executable&gt;.recent under Linux.
/// File is UTF-8 and contains one file name per line.
/// * hash characters (#) denote comment.
/// * backslash (\) is used for escaping.
/// </remarks>
/// <example>
/// <code>
/// RecentFiles.Add("C:\test\file.txt");
/// foreach (var file in RecentFiles.GetFiles()) {}
/// </code>
/// </example>
public static class RecentFiles {

    private static readonly object Lock = new();

    /// <summary>
    /// Clears all files.
    /// </summary>
    public static void Clear() {
        lock (Lock) {
            WriteAll([]);
        }
    }

    /// <summary>
    /// Adds file to the top of the recently used list.
    /// Full path will be used.
    /// </summary>
    /// <param name="filePath">File.</param>
    /// <exception cref="ArgumentNullException">File cannot be null.</exception>
    public static void Add(FileInfo file) {
        if (file == null) { throw new ArgumentNullException(nameof(file), "File cannot be null."); }

        lock (Lock) {
            var files = ReadAll();
            for (var i = files.Count - 1; i >= 0; i--) {
                if (string.Equals(files[i].FullName, file.FullName, StringComparison.Ordinal)) {
                    files.RemoveAt(i);
                }
            }
            files.Insert(0, file);
            WriteAll(files);
        }
    }

    /// <summary>
    /// Returns true if given file was successfully removed from the recently used list.
    /// Full path will be used.
    /// </summary>
    /// <param name="file">File.</param>
    /// <exception cref="ArgumentNullException">File cannot be null.</exception>
    public static bool Remove(FileInfo file) {
        if (file == null) { throw new ArgumentNullException(nameof(file), "File cannot be null."); }

        lock (Lock) {
            var files = ReadAll();
            var anyChanges = false;
            for (var i = files.Count - 1; i >= 0; i--) {
                if (string.Equals(files[i].FullName, file.FullName, StringComparison.Ordinal)) {
                    files.RemoveAt(i);
                    anyChanges = true;
                }
            }
            if (anyChanges) { WriteAll(files); }
            return anyChanges;
        }
    }


    /// <summary>
    /// Returns list of recently used files.
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyList<FileInfo> GetFiles() {
        return ReadAll().AsReadOnly();
    }


    private static int _maximumCount = 10;
    /// <summary>
    /// Gets/sets maximum file count.
    /// Count is adjusted upon the next update.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Value must be between 1 and 100.</exception>
    public static int MaximumCount {
        get { return _maximumCount; }
        set {
            if (value is < 1 or > 100) { throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 1 and 100."); }
            _maximumCount = value;
        }
    }


    private static bool _isAssumedInstalled;
    /// <summary>
    /// Gets/sets if application is assumed to be installed.
    /// Application is considered installed if it is located in Program Files directory (or opt).
    /// Setting value to true before loading files will force assumption of installation status.
    /// </summary>
    public static bool IsAssumedInstalled {
        get {
            lock (LockInitialize) {
                if (!IsInitialized) { Initialize(); }
                return _isAssumedInstalled;
            }
        }
        set {
            if (IsInitialized) { throw new InvalidOperationException("Cannot set value once config has been loaded."); }
            _isAssumedInstalled = value;
        }
    }

    private static string? _filePath;
    /// <summary>
    /// Gets/sets the name of the file used for recent files.
    /// Settings are always written to this file but reading might be done from override file first
    /// If executable is located under Program Files, properties file will be in Application Data.
    /// If executable is located in some other directory, a local file will be used.
    /// </summary>
    /// <exception cref="ArgumentNullException">Value cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Value is not a valid path.</exception>
    public static string FileName {
        get {
            lock (LockInitialize) {
                if (!IsInitialized) { Initialize(); }
                return _filePath!;
            }
        }
        set {
            lock (LockInitialize) {
                if (!IsInitialized) { Initialize(); }
                if (value == null) {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                } else if (value.IndexOfAny(Path.GetInvalidPathChars()) >= 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value is not a valid path.");
                } else {
                    _filePath = value;
                }
            }
        }
    }


    private static readonly Encoding Utf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    private static readonly string[] EolSeparators = ["\r\n", "\n", "\r"];

    private static List<FileInfo> ReadAll() {
        var files = new List<FileInfo>();

        string? allText = null;
        try {  // ignore errors during write
            allText = File.ReadAllText(FileName, Utf8Encoding);
        } catch (PathTooLongException) {
        } catch (DirectoryNotFoundException) {
        } catch (IOException) {
        } catch (UnauthorizedAccessException) {
        } catch (NotSupportedException) {
        } catch (SecurityException) { }

        if (allText != null) {
            var lines = allText.Split(EolSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines) {
                try {  // ignore errors during write
                    files.Add(new FileInfo(UnescapeText(line)));
                } catch (SecurityException) {
                } catch (UnauthorizedAccessException) {
                } catch (PathTooLongException) {
                } catch (NotSupportedException) { }
            }
        }

        if (files.Count > MaximumCount) {
            for (var i = files.Count - 1; i >= MaximumCount; i--) {
                files.RemoveAt(i);
            }
        }
        return files;
    }

    private static void WriteAll(List<FileInfo> files) {
        if (files.Count > MaximumCount) {
            for (var i = files.Count - 1; i >= MaximumCount; i--) {
                files.RemoveAt(i);
            }
        }

        var sb = new StringBuilder();
        foreach (var file in files) {
            sb.Append(EscapeText(file.FullName));
            sb.Append(Environment.NewLine);
        }

        try {  // ignore errors during write
            File.WriteAllText(FileName, sb.ToString(), Utf8Encoding);
        } catch (PathTooLongException) {
        } catch (DirectoryNotFoundException) {
        } catch (IOException) {
        } catch (UnauthorizedAccessException) {
        } catch (NotSupportedException) {
        } catch (SecurityException) { }
    }


    #region Initialize

    private static bool IsInitialized { get; set; }
    private static readonly object LockInitialize = new();
    private static bool IsOSWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    private static void Initialize() {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

        string? companyValue = null;
        string? productValue = null;
        string? titleValue = null;

        var attributes = assembly.GetCustomAttributes();
        foreach (var attribute in attributes) {
            if (attribute is AssemblyCompanyAttribute companyAttribute) { companyValue = companyAttribute.Company.Trim(); }
            if (attribute is AssemblyProductAttribute productAttribute) { productValue = productAttribute.Product.Trim(); }
            if (attribute is AssemblyTitleAttribute titleAttribute) { titleValue = titleAttribute.Title.Trim(); }
        }

        var company = companyValue ?? "";
        var application = productValue ?? titleValue ?? assembly.GetName().Name ?? "application";
#if NET5_0_OR_GREATER
        var executablePath = AppContext.BaseDirectory;
#else
            var executablePath = assembly.Location;
#endif

        var baseFileName = IsOSWindows
            ? application + ".recent"
            : "." + application.ToLowerInvariant() + ".recent";

        var userFilePath = IsOSWindows
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), company, application, baseFileName)
            : Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "~", baseFileName);

        var localFilePath = Path.Combine(Path.GetDirectoryName(executablePath) ?? "", baseFileName);

        if (IsOSWindows) {

#if NETSTANDARD1_6
                var isPF = executablePath.StartsWith(AddDirectorySuffixIfNeeded(Environment.GetEnvironmentVariable("ProgramFiles")), StringComparison.OrdinalIgnoreCase);
                var isPF32 = executablePath.StartsWith(AddDirectorySuffixIfNeeded(Environment.GetEnvironmentVariable("ProgramFiles(x86)")), StringComparison.OrdinalIgnoreCase);
                var isPF64 = executablePath.StartsWith(AddDirectorySuffixIfNeeded(Environment.GetEnvironmentVariable("ProgramW6432")), StringComparison.OrdinalIgnoreCase);
                var isUserPF = executablePath.StartsWith(AddDirectorySuffixIfNeeded(Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "Programs")), StringComparison.OrdinalIgnoreCase);
#else
            var isPF = executablePath.StartsWith(AddDirectorySuffixIfNeeded(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)), StringComparison.OrdinalIgnoreCase);
            var isPF32 = executablePath.StartsWith(AddDirectorySuffixIfNeeded(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)), StringComparison.OrdinalIgnoreCase);
            var isPF64 = executablePath.StartsWith(AddDirectorySuffixIfNeeded(Environment.GetEnvironmentVariable("ProgramW6432")), StringComparison.OrdinalIgnoreCase);
            var isUserPF = executablePath.StartsWith(AddDirectorySuffixIfNeeded(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs")), StringComparison.OrdinalIgnoreCase);
#endif
            var isInstalled = isPF || isPF32 || isPF64 || isUserPF || _isAssumedInstalled;

            if (isInstalled) { //if in program files, assume user config is first, use local file as override
                _isAssumedInstalled = true;
                _filePath = userFilePath;
            } else { //if outside of program files, assume local file only
                _isAssumedInstalled = false;
                _filePath = localFilePath;
            }

        } else { //Linux

            var isOpt = executablePath.StartsWith(Path.DirectorySeparatorChar + "opt" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
            var isBin = executablePath.StartsWith(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
            var isUsrBin = executablePath.StartsWith(Path.DirectorySeparatorChar + "usr" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
            var isInstalled = isOpt || isBin || isUsrBin || _isAssumedInstalled;

            if (isInstalled) {
                _isAssumedInstalled = true;
                _filePath = userFilePath;
                if (isOpt) { //change override file location to /etc/opt/<app>/<app>.cfg
                    var globalFilePath = Path.DirectorySeparatorChar + "etc" + Path.Combine(Path.GetDirectoryName(executablePath) ?? "", application.ToLowerInvariant() + ".conf");
                }
            } else { //if outside of program files, assume local file only
                _isAssumedInstalled = false;
                _filePath = localFilePath;
            }

        }

        IsInitialized = true;
    }

    private static string AddDirectorySuffixIfNeeded(string? path) {
        if (string.IsNullOrEmpty(path)) { return ""; }
        path = path.Trim();
        if (path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)) { return path; }
        return path + Path.DirectorySeparatorChar;
    }

    #endregion


    private static string EscapeText(string text) {
        var sb = new StringBuilder();
        for (var i = 0; i < text.Length; i++) {
            var ch = text[i];
            switch (ch) {
                case '\\':
                    sb.Append(@"\\");
                    break;
                case '\0':
                    sb.Append(@"\0");
                    break;
                case '\b':
                    sb.Append(@"\b");
                    break;
                case '\t':
                    sb.Append(@"\t");
                    break;
                case '\r':
                    sb.Append(@"\r");
                    break;
                case '\n':
                    sb.Append(@"\n");
                    break;
                default:
                    if (char.IsControl(ch)) {
                        sb.Append(@"\u");
                        sb.Append(((int)ch).ToString("X4", CultureInfo.InvariantCulture));
                    } else {
                        sb.Append(ch);
                    }
                    break;
            }
        }
        return sb.ToString();
    }

    private enum State { Default, Escape, EscapeLong };

    private static string UnescapeText(string text) {
        var sb = new StringBuilder();
        var sbEscapeLong = new StringBuilder();

        var state = State.Default;
        foreach (var ch in text) {
            switch (state) {
                case State.Default:
                    if (ch is '\\') {
                        state = State.Escape;
                    } else {
                        sb.Append(ch);
                    }
                    break;

                case State.Escape:
                    if (ch is 'u') {
                        state = State.EscapeLong;
                    } else {
                        var newCh = ch switch {
                            '0' => '\0',
                            'b' => '\b',
                            't' => '\t',
                            'n' => '\n',
                            'r' => '\r',
                            '_' => ' ',
                            _ => ch,
                        };
                        sb.Append(newCh);
                        state = State.Default;
                    }
                    break;

                case State.EscapeLong:
                    sbEscapeLong.Append(ch);
                    if (sbEscapeLong.Length == 4) {
                        if (int.TryParse(sbEscapeLong.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var chValue)) {
                            sb.Append((char)chValue);
                        }
                        state = State.Default;
                    }
                    break;
            }
        }

        return sb.ToString();
    }
}
