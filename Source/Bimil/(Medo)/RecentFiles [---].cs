/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2017-04-29: Obsoleted.
//2016-03-24: Added IEnumerable interface.
//2012-08-20: Fixed crash when HideFileExt cannot be found in registry.
//2012-05-31: Refactoring.
//2010-10-31: Added option to skip registry writes (NoRegistryWrites).
//2009-07-04: Compatibility with Mono 2.4.
//2009-05-23: New version.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Security;
using Microsoft.Win32;

namespace Medo.Configuration {

    /// <summary>
    /// Enables loading and saving of files list.
    /// It is written in State key at HKEY_CURRENT_USER branch withing defined SubKeyPath.
    /// </summary>
    //[Obsolete("Use RecentlyUsed in combination with Config instead.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "While this class offers IEnumerable interface, it is not a collection as such.")]
    public class RecentFiles : IEnumerable<RecentFile> {

        /// <summary>
        /// Creates new instance with "Default" as group name and maximum of 16 files.
        /// </summary>
        public RecentFiles()
            : this(16, null) {
        }

        /// <summary>
        /// Creates new instance with "Default" as group name.
        /// </summary>
        /// <param name="maximumCount">Maximum number of items to load or save.</param>
        public RecentFiles(int maximumCount)
            : this(maximumCount, null) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="maximumCount">Maximum number of items to load or save.</param>
        /// <param name="groupName">Name of group. If omitted, "Default" is used.</param>
        public RecentFiles(int maximumCount, string groupName) {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null) { assembly = Assembly.GetCallingAssembly(); } //e.g. when running unit tests

            string company = null;
            var companyAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
            if ((companyAttributes != null) && (companyAttributes.Length >= 1)) {
                company = ((AssemblyCompanyAttribute)companyAttributes[companyAttributes.Length - 1]).Company;
            }

            string product;
            var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
            if ((productAttributes != null) && (productAttributes.Length >= 1)) {
                product = ((AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product;
            } else {
                var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
                if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                    product = ((AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
                } else {
                    product = assembly.GetName().Name;
                }
            }

            var basePath = "Software";
            if (!string.IsNullOrEmpty(company)) { basePath += "\\" + company; }
            if (!string.IsNullOrEmpty(product)) { basePath += "\\" + product; }

            SubkeyPath = basePath + "\\RecentFiles";

            MaximumCount = maximumCount;
            if (string.IsNullOrEmpty(groupName)) {
                GroupName = "Default";
            } else {
                GroupName = groupName;
            }

            Load();
        }

        /// <summary>
        /// Gets/sets whether settings should be written to registry.
        /// </summary>
        public static bool NoRegistryWrites { get; set; }

        /// <summary>
        /// Gets maximum number of file names to be saved.
        /// </summary>
        public int MaximumCount { get; private set; }

        /// <summary>
        /// Gets number of file names.
        /// </summary>
        public int Count {
            get { return _items.Count; }
        }

        /// <summary>
        /// Group name.
        /// </summary>
        public string GroupName { get; private set; }


        private readonly List<RecentFile> _items = new List<RecentFile>();

        /// <summary>
        /// Gets file name at given index.
        /// </summary>
        /// <param name="index">Index.</param>
        public RecentFile this[int index] {
            get { return _items[index]; }
        }

        /// <summary>
        /// Returns read-only collection of recent files.
        /// </summary>
        [Obsolete("Use Items property instead.")]
        public ReadOnlyCollection<RecentFile> AsReadOnly() {
            return _items.AsReadOnly();
        }

        /// <summary>
        /// Returns each recent file.
        /// </summary>
        public IEnumerable<RecentFile> Items {
            get {
                foreach (var item in _items) {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Inserts file name on top of list if one does not exist or moves it to top if one does exist.
        /// All changes are immediately saved.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void Push(string fileName) {
            var item = RecentFile.GetRecentFile(fileName);
            if (item != null) {
                _items.Insert(0, item);

                for (var i = _items.Count - 1; i >= 1; --i) { //remove duplicate of it
                    if (_items[i].Equals(fileName)) {
                        _items.RemoveAt(i);
                    }
                }

                Save();
            }
        }

        /// <summary>
        /// Removes all occurrances of given file.
        /// All changes are immediately saved.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void Remove(string fileName) {
            for (var i = _items.Count - 1; i >= 0; --i) {
                if (_items[i].Equals(fileName)) {
                    _items.RemoveAt(i);
                }
            }
            Save();
        }

        /// <summary>
        /// Removes all files from list.
        /// All changes are immediately saved.
        /// </summary>
        public void Clear() {
            _items.Clear();
            Save();
        }


        /// <summary>
        /// Reloads file list from registry.
        /// </summary>
        public void Load() {
            _items.Clear();
            try {
                using (var rk = Registry.CurrentUser.OpenSubKey(SubkeyPath, false)) {
                    if (rk != null) {
                        var valueCU = rk.GetValue(GroupName, null);
                        if (valueCU != null) {
                            var valueKind = RegistryValueKind.MultiString;
                            if (!RecentFiles.IsRunningOnMono) { valueKind = rk.GetValueKind(GroupName); }
                            if (valueKind == RegistryValueKind.MultiString) {
                                if (valueCU is string[] valueArr) {
                                    for (var i = 0; i < valueArr.Length; ++i) {
                                        if (!string.IsNullOrEmpty(valueArr[i])) {
                                            var item = RecentFile.GetRecentFile(valueArr[i]);
                                            if (item != null) {
                                                _items.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (SecurityException) { }
        }

        /// <summary>
        /// Saves current list to registry.
        /// This is automaticaly done on each insert.
        /// </summary>
        public void Save() {
            if (_items.Count > MaximumCount) { _items.RemoveRange(MaximumCount, _items.Count - MaximumCount); }

            var fileNames = new string[_items.Count];
            for (var i = 0; i < _items.Count; ++i) {
                fileNames[i] = _items[i].FileName;
            }

            if (RecentFiles.NoRegistryWrites == false) {
                using (var rk = Registry.CurrentUser.CreateSubKey(SubkeyPath)) {
                    rk.SetValue(GroupName, fileNames, RegistryValueKind.MultiString);
                }
            }
        }


        /// <summary>
        /// Gets/sets subkey used for registry storage.
        /// </summary>
        private string SubkeyPath { get; set; }

        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }


        #region IEnumerable

        /// <summary>
        /// Returns an IEnumerator for the recent files.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<RecentFile> GetEnumerator() {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Items.GetEnumerator();
        }

        #endregion

    }



    /// <summary>
    /// Single recent file
    /// </summary>
    public class RecentFile {

        private RecentFile(string fileName, string title) {
            FileName = fileName;
            Title = title;
        }

        /// <summary>
        /// Gets full file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets title of current file.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj) {
            if (obj is RecentFile other) {
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
        /// Returns a System.String that represents the current object.
        /// </summary>
        public override string ToString() {
            return Title;
        }


        #region Static

        /// <summary>
        /// Gets recent file object or null if such object cannot be found.
        /// </summary>
        /// <param name="fileName">File name.</param>
        internal static RecentFile GetRecentFile(string fileName) {
            try {
                var title = HideExtension ? Path.GetFileNameWithoutExtension(fileName) : Path.GetFileName(fileName);
                return new RecentFile(fileName, title);
            } catch (ArgumentException) {
                return null;
            }
        }


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
                } catch (IOException) { //key does not exist
                }
                return false;
            }
        }

        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }


        #endregion

    }

}
