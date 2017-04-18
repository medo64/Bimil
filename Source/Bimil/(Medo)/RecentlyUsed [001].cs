//Copyright 2017 by Josip Medved <jmedved@jmedved.com> (www.medo64.com) MIT License

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
            this.MaximumCount = maximumCount;
            if (fileNames != null) {
                foreach (var fileName in fileNames) {
                    this.BaseItems.Add(new RecentlyUsedFile(fileName));
                    if (this.BaseItems.Count >= this.MaximumCount) { break; }
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
            get { return this.BaseItems.Count; }
        }


        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private List<RecentlyUsedFile> BaseItems = new List<RecentlyUsedFile>();

        /// <summary>
        /// Gets file at given index.
        /// </summary>
        /// <param name="index">Index.</param>
        public RecentlyUsedFile this[int index] {
            get { return this.BaseItems[index]; }
        }

        /// <summary>
        /// Returns each recently used file.
        /// </summary>
        public IEnumerable<RecentlyUsedFile> Files {
            get {
                foreach (var item in this.BaseItems) {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Returns file path enumeration.
        /// </summary>
        public IEnumerable<string> FileNames {
            get {
                foreach (var item in this.Files) {
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
            this.BaseItems.Insert(0, file);
            for (var i = this.BaseItems.Count - 1; i > 0; i--) { //remove same file if somewhere on the list
                if (this.BaseItems[i].Equals(file)) { this.BaseItems.RemoveAt(i); }
            }
            if (this.BaseItems.Count > this.MaximumCount) { //limit to maximum count
                this.BaseItems.RemoveRange(this.MaximumCount, this.BaseItems.Count - this.MaximumCount);
            }
            this.OnChanged(EventArgs.Empty);
            return true;
        }


        /// <summary>
        /// Removes all occurrances of given file.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void Remove(string fileName) {
            var changed = false;
            for (var i = this.BaseItems.Count - 1; i >= 0; i--) {
                if (this.BaseItems[i].Equals(fileName)) {
                    this.BaseItems.RemoveAt(i);
                    changed = true;
                }
            }
            if (changed) { this.OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        /// Removes all occurrances of given file.
        /// </summary>
        /// <param name="file">Recently used file.</param>
        public void Remove(RecentlyUsedFile file) {
            var changed = false;
            for (var i = this.BaseItems.Count - 1; i >= 0; i--) {
                if (this.BaseItems[i].Equals(file)) {
                    this.BaseItems.RemoveAt(i);
                    changed = true;
                }
            }
            if (changed) { this.OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        /// Removes all files from list.
        /// </summary>
        public void Clear() {
            if (this.BaseItems.Count > 0) {
                this.BaseItems.Clear();
                this.OnChanged(EventArgs.Empty);
            }
        }


        /// <summary>
        /// Raises Changed event.
        /// </summary>
        public event EventHandler Changed;

        private void OnChanged(EventArgs e) {
            this.Changed?.Invoke(this, e);
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
            this.FileName = new FileInfo(fileName).FullName; //expand local names
            this.Title = HideExtension ? Path.GetFileNameWithoutExtension(this.FileName) : Path.GetFileName(this.FileName);
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
                    return File.Exists(this.FileName);
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
                return string.Equals(this.FileName, other.FileName, StringComparison.OrdinalIgnoreCase);
            }
            if (obj is string otherString) {
                return string.Equals(this.FileName, otherString, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode() {
            return this.FileName.GetHashCode();
        }

        /// <summary>
        /// Returns a file title.
        /// </summary>
        public override string ToString() {
            return this.Title;
        }


#if NETSTANDARD1_6
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
