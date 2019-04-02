/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2017-04-17: New version.


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using Microsoft.Win32;

namespace Medo.Configuration {

    /// <summary>
    /// Handling of recently used file list.
    /// </summary>
    [DebuggerDisplay("{Count} files")]
    public class RecentlyUsed {

        /// <summary>
        /// Creates a new instance and maximum of 16 files.
        /// </summary>
        public RecentlyUsed()
            : this(null, 16) {
        }

        /// <summary>
        /// Creates a new instance and maximum of 16 files.
        /// </summary>
        /// <param name="fileNames">File path enumeration.</param>
        public RecentlyUsed(IEnumerable<string> fileNames)
            : this(fileNames, 16) {
        }

        /// <summary>
        /// Creates new instance with "Default" as group name.
        /// </summary>
        /// <param name="fileNames">File path enumeration.</param>
        /// <param name="maximumCount">Maximum number of items to load or save.</param>
        /// <exception cref="ArgumentOutOfRangeException">Maximum count must be between 1 and 99.</exception>
        public RecentlyUsed(IEnumerable<string> fileNames, int maximumCount) {
            if ((maximumCount < 1) || (maximumCount > 99)) { throw new ArgumentOutOfRangeException(nameof(maximumCount), "Maximum count must be between 1 and 99."); }
            MaximumCount = maximumCount;
            if (fileNames != null) {
                foreach (var fileName in fileNames) {
                    BaseItems.Add(new RecentlyUsedFile(fileName));
                    if (BaseItems.Count >= MaximumCount) { break; }
                }
            }
        }


        /// <summary>
        /// Gets maximum number of files.
        /// </summary>
        public int MaximumCount { get; }

        /// <summary>
        /// Gets number of file names.
        /// </summary>
        public int Count {
            get { return BaseItems.Count; }
        }


        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly List<RecentlyUsedFile> BaseItems = new List<RecentlyUsedFile>();

        /// <summary>
        /// Gets file at given index.
        /// </summary>
        /// <param name="index">Index.</param>
        public RecentlyUsedFile this[int index] {
            get { return BaseItems[index]; }
        }

        /// <summary>
        /// Returns each recently used file.
        /// </summary>
        public IEnumerable<RecentlyUsedFile> Files {
            get {
                foreach (var item in BaseItems) {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Returns file path enumeration.
        /// </summary>
        public IEnumerable<string> FileNames {
            get {
                foreach (var item in Files) {
                    yield return item.FileName;
                }
            }
        }

        /// <summary>
        /// Inserts file name on top of list if one does not exist or moves it to top if one does exist.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public bool Push(string fileName) {
            RecentlyUsedFile file;
            try {
                file = new RecentlyUsedFile(fileName);
            } catch (ArgumentException) { return false; }
            BaseItems.Insert(0, file);
            for (var i = BaseItems.Count - 1; i > 0; i--) { //remove same file if somewhere on the list
                if (BaseItems[i].Equals(file)) { BaseItems.RemoveAt(i); }
            }
            if (BaseItems.Count > MaximumCount) { //limit to maximum count
                BaseItems.RemoveRange(MaximumCount, BaseItems.Count - MaximumCount);
            }
            OnChanged(EventArgs.Empty);
            return true;
        }


        /// <summary>
        /// Removes all occurrances of given file.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void Remove(string fileName) {
            var changed = false;
            for (var i = BaseItems.Count - 1; i >= 0; i--) {
                if (BaseItems[i].Equals(fileName)) {
                    BaseItems.RemoveAt(i);
                    changed = true;
                }
            }
            if (changed) { OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        /// Removes all occurrances of given file.
        /// </summary>
        /// <param name="file">Recently used file.</param>
        public void Remove(RecentlyUsedFile file) {
            var changed = false;
            for (var i = BaseItems.Count - 1; i >= 0; i--) {
                if (BaseItems[i].Equals(file)) {
                    BaseItems.RemoveAt(i);
                    changed = true;
                }
            }
            if (changed) { OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        /// Removes all files from list.
        /// </summary>
        public void Clear() {
            if (BaseItems.Count > 0) {
                BaseItems.Clear();
                OnChanged(EventArgs.Empty);
            }
        }


        /// <summary>
        /// Raises Changed event.
        /// </summary>
        public event EventHandler Changed;

        private void OnChanged(EventArgs e) {
            Changed?.Invoke(this, e);
        }
    }



    /// <summary>
    /// Single recent file.
    /// </summary>
    public class RecentlyUsedFile {

        /// <summary>
        /// Creates a new recently used file.
        /// </summary>
        /// <param name="fileName">File name.</param>
        internal RecentlyUsedFile(string fileName) {
            FileName = new FileInfo(fileName).FullName; //expand local names
            Title = HideExtension ? Path.GetFileNameWithoutExtension(FileName) : Path.GetFileName(FileName);
        }


        /// <summary>
        /// Gets full file name.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets title of current file.
        /// </summary>
        public string Title { get; }


        /// <summary>
        /// Gets if file exists.
        /// </summary>
        public bool FileExists {
            get {
                try {
                    return File.Exists(FileName);
                } catch (IOException) {
                } catch (SecurityException) { }
                return false;
            }
        }


        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj) {
            if (obj is RecentlyUsedFile other) {
                return string.Equals(FileName, other.FileName, StringComparison.OrdinalIgnoreCase);
            }
            if (obj is string otherString) {
                return string.Equals(FileName, otherString, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode() {
            return FileName.GetHashCode();
        }

        /// <summary>
        /// Returns a file title.
        /// </summary>
        public override string ToString() {
            return Title;
        }


#if NETSTANDARD1_6 || NETSTANDARD2_0
        private static bool HideExtension {
            get { return false; }
        }
#else
        private static bool HideExtension {
            get {
                try {
                    using (var rk = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", false)) {
                        if (rk != null) {
                            var valueKind = IsRunningOnMono ? RegistryValueKind.DWord : rk.GetValueKind("HideFileExt");
                            if (valueKind == RegistryValueKind.DWord) {
                                var hideFileExt = (int)(rk.GetValue("HideFileExt", 1));
                                return (hideFileExt != 0);
                            }
                        }
                    }
                } catch (SecurityException) {
                } catch (IOException) { } //key does not exist
                return false;
            }
        }

        private static bool IsRunningOnMono {
            get { return (Type.GetType("Mono.Runtime") != null); }
        }

#endif

    }
}
