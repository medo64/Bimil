using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Collection of header fields.
    /// </summary>
    [DebuggerDisplay("{Count} headers")]
    public class HeaderCollection : IList<Header> {

        /// <summary>
        /// Create a new instance.
        /// </summary>
        internal HeaderCollection(Document owner, ICollection<Header> fields) {
            foreach (var field in fields) {
                if (field.Owner != null) { throw new ArgumentOutOfRangeException(nameof(fields), "Item cannot be in other collection."); }
            }

            this.Owner = owner;
            this.BaseCollection.AddRange(fields);
            foreach (var field in fields) {
                field.Owner = this;
            }

            //ensure first field is always Version
            if (!this.Contains(HeaderType.Version)) {
                this.BaseCollection.Insert(0, new Header(HeaderType.Version, BitConverter.GetBytes(Header.DefaultVersion)));
            } else {
                var versionField = this[HeaderType.Version];
                var versionIndex = this.IndexOf(versionField);
                if (versionIndex > 0) {
                    this.BaseCollection.RemoveAt(versionIndex);
                    this.BaseCollection.Insert(0, versionField);
                }
            }
        }


        internal Document Owner { get; set; }

        internal void MarkAsChanged() {
            this.Owner.MarkAsChanged();
        }


        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly List<Header> BaseCollection = new List<Header>();


        #region ICollection

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <exception cref="System.ArgumentNullException">Item cannot be null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Item cannot be in other collection.</exception>
        /// <exception cref="System.NotSupportedException">Collection is read-only.</exception>
        public void Add(Header item) {
            if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
            if (item.Owner != null) { throw new ArgumentOutOfRangeException(nameof(item), "Item cannot be in other collection."); }
            if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

            this.BaseCollection.Add(item);
            item.Owner = this;
            this.MarkAsChanged();
        }

        /// <summary>
        /// Adds multiple items.
        /// </summary>
        /// <param name="items">Item.</param>
        /// <exception cref="System.ArgumentNullException">Items cannot be null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Item cannot be in other collection.</exception>
        /// <exception cref="System.NotSupportedException">Collection is read-only.</exception>
        public void AddRange(IEnumerable<Header> items) {
            if (items == null) { throw new ArgumentNullException(nameof(items), "Item cannot be null."); }
            foreach (var item in items) {
                if (item.Owner != null) { throw new ArgumentOutOfRangeException(nameof(items), "Item cannot be in other collection."); }
            }
            if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

            this.BaseCollection.AddRange(items);
            foreach (var item in items) {
                item.Owner = this;
            }
            this.MarkAsChanged();
        }

        /// <summary>
        /// Removes all items.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Collection is read-only.</exception>
        public void Clear() {
            if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

            for (int i = this.BaseCollection.Count - 1; i > 0; i--) { //remove all except first field (Version)
                this.BaseCollection[i].Owner = null;
                this.BaseCollection.RemoveAt(i);
            }
            this.MarkAsChanged();
        }

        /// <summary>
        /// Determines whether the collection contains a specific item.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        public bool Contains(Header item) {
            if (item == null) { return false; }
            return this.BaseCollection.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from collection.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Header[] array, int arrayIndex) {
            this.BaseCollection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of items contained in the collection.
        /// </summary>
        public int Count {
            get { return this.BaseCollection.Count; }
        }

        /// <summary>
        /// Searches for the specified item and returns the zero-based index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        public int IndexOf(Header item) {
            return this.BaseCollection.IndexOf(item);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="System.ArgumentNullException">Item cannot be null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than 0. -or- Index is greater than collection count.</exception>
        /// <exception cref="System.NotSupportedException">Collection is read-only.</exception>
        public void Insert(int index, Header item) {
            if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
            if (item.Owner != null) { throw new ArgumentOutOfRangeException(nameof(item), "Item cannot be in other collection."); }
            if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
            if ((index == 0) && (item.HeaderType != HeaderType.Version)) { throw new ArgumentOutOfRangeException(nameof(index), "Version must be the first field."); }

            this.BaseCollection.Insert(index, item);
            item.Owner = this;
            this.MarkAsChanged();
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly {
            get { return this.Owner.IsReadOnly; }
        }

        /// <summary>
        /// Removes the item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <exception cref="System.ArgumentNullException">Item cannot be null.</exception>
        /// <exception cref="System.NotSupportedException">Collection is read-only.</exception>
        public bool Remove(Header item) {
            if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
            if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
            if ((item.HeaderType == HeaderType.Version) && this.BaseCollection.IndexOf(item) == 0) { throw new ArgumentOutOfRangeException(nameof(item), "Cannot remove the first version field."); }

            if (this.BaseCollection.Remove(item)) {
                item.Owner = null;
                this.MarkAsChanged();
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than 0. -or- Index is equal to or greater than collection count.</exception>
        /// <exception cref="System.NotSupportedException">Collection is read-only.</exception>
        public void RemoveAt(int index) {
            if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
            if (index == 0) { throw new ArgumentOutOfRangeException(nameof(index), "Cannot remove the first version field."); }

            var item = this[index];
            this.BaseCollection.Remove(item);
            item.Owner = null;
            this.MarkAsChanged();
        }

        /// <summary>
        /// Exposes the enumerator, which supports a simple iteration over a collection of a specified type.
        /// </summary>
        public IEnumerator<Header> GetEnumerator() {
            var items = new List<Header>(this.BaseCollection); //to avoid exception if access/modification time has to be updated while in foreach
            foreach (var item in items) {
                yield return item;
            }
        }

        /// <summary>
        /// Exposes the enumerator, which supports a simple iteration over a non-generic collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="System.ArgumentNullException">Value cannot be null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than 0. -or- Index is equal to or greater than collection count. -or- Duplicate name in collection.</exception>
        /// <exception cref="System.NotSupportedException">Collection is read-only.</exception>
        public Header this[int index] {
            get { return this.BaseCollection[index]; }
            set {
                if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
                if (value == null) { throw new ArgumentNullException(nameof(value), "Value cannot be null."); }
                if (value.Owner != null) { throw new ArgumentOutOfRangeException(nameof(value), "Item cannot be in other collection."); }
                if (this.Contains(value)) { throw new ArgumentOutOfRangeException(nameof(value), "Duplicate item in collection."); }
                if ((index == 0) && (value.HeaderType != HeaderType.Version)) { throw new ArgumentOutOfRangeException(nameof(value), "Version must be the first field."); }

                var item = this.BaseCollection[index];
                item.Owner = null;
                this.BaseCollection.RemoveAt(index);
                this.BaseCollection.Insert(index, value);
                value.Owner = this;
                this.MarkAsChanged();
            }
        }

        #endregion


        #region ICollection extra

        /// <summary>
        /// Determines whether the collection contains a specific type.
        /// If multiple elements exist with the same type, the first one is returned.
        /// </summary>
        /// <param name="type">The item type to locate.</param>
        public bool Contains(HeaderType type) {
            foreach (var item in this.BaseCollection) {
                if (item.HeaderType == type) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Gets field based on a type.
        /// If multiple elements exist with the same field type, the first one is returned.
        /// If type does not exist, it is created.
        /// 
        /// If value is set to null, field is removed.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <exception cref="ArgumentOutOfRangeException">Only null value is supported.</exception>
        /// <exception cref="NotSupportedException">Collection is read-only.</exception>
        public Header this[HeaderType type] {
            get {
                foreach (var field in this.BaseCollection) {
                    if (field.HeaderType == type) {
                        return field;
                    }
                }

                if (this.IsReadOnly) { return new Header(type); } //return dummy header if collection is read-only 

                var newField = new Header(this, type); //create a new field if one cannot be found

                int i = this.BaseCollection.Count;
                for (i = 0; i < this.BaseCollection.Count; i++) {
                    if (this.BaseCollection[i].HeaderType > type) { break; }
                }
                this.BaseCollection.Insert(i, newField); //insert it in order (does not change order for existing ones)

                return newField;
            }
            set {
                if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
                if (value != null) { throw new ArgumentOutOfRangeException(nameof(value), "Only null value is supported."); }

                Header fieldToRemove = null;
                foreach (var field in this.BaseCollection) {
                    if (field.HeaderType == type) {
                        fieldToRemove = field;
                        break;
                    }
                }
                if (fieldToRemove != null) {
                    this.Remove(fieldToRemove);
                }
            }
        }

        #endregion

    }
}
