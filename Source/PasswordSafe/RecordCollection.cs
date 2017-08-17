using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Collection of record fields.
    /// </summary>
    [DebuggerDisplay("{Count} records")]
    public class RecordCollection : IList<Record> {

        /// <summary>
        /// Create a new instance.
        /// </summary>
        internal RecordCollection(Entry owner, ICollection<Record> fields) {
            foreach (var field in fields) {
                if (field.Owner != null) { throw new ArgumentOutOfRangeException(nameof(fields), "Item cannot be in other collection."); }
            }

            this.Owner = owner;
            this.BaseCollection.AddRange(fields);
            foreach (var field in fields) {
                field.Owner = this;
            }
        }


        internal Entry Owner { get; set; }

        internal void MarkAsChanged(RecordType type = 0) {
            if ((this.Owner != null) && (this.Owner.Owner != null)) {
                this.Owner.Owner.MarkAsChanged();
            }

            if (this.Count == 0) { return; } //don't track times if there is no other elements

            if (!this.IsReadOnly && (this.Owner != null) && (this?.Owner?.Owner?.Owner?.TrackModify ?? false)) {

                switch (type) {
                    case RecordType.CreationTime:
                    case RecordType.PasswordModificationTime:
                    case RecordType.LastModificationTime:
                    case RecordType.LastAccessTime:
                        break; //ignore changes to fields we auto-fill

                    default:
                        if (!this.Owner.Records.Contains(RecordType.CreationTime)) { //if there is no creation time yet, assume it is just now created.
                            this.Owner.Records[RecordType.CreationTime].Time = DateTime.UtcNow;
                        } else {
                            this.Owner.Records[RecordType.LastModificationTime].Time = DateTime.UtcNow;
                        }

                        if (type == RecordType.Password) {
                            this.Owner.Records[RecordType.PasswordModificationTime].Time = DateTime.UtcNow;
                        }
                        break;
                }
            }
        }

        internal void MarkAsAccessed(RecordType type = 0) {
            if (!this.IsReadOnly && (this.Owner != null) && (this?.Owner?.Owner?.Owner?.TrackAccess ?? false)) {

                switch (type) {
                    case RecordType.Uuid:
                    case RecordType.Group:
                    case RecordType.Title:
                        break; //ignore access to fields we need for normal display

                    case RecordType.CreationTime:
                    case RecordType.PasswordModificationTime:
                    case RecordType.LastModificationTime:
                    case RecordType.LastAccessTime:
                        break; //ignore access to fields we auto-fill

                    default:
                        this.Owner.Records[RecordType.LastAccessTime].Time = DateTime.UtcNow;
                        break;
                }
            }
        }


        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly List<Record> BaseCollection = new List<Record>();


        #region ICollection

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <exception cref="System.ArgumentNullException">Item cannot be null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Item cannot be in other collection.</exception>
        /// <exception cref="System.NotSupportedException">Collection is read-only.</exception>
        public void Add(Record item) {
            if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
            if (item.Owner != null) { throw new ArgumentOutOfRangeException(nameof(item), "Item cannot be in other collection."); }
            if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

            this.BaseCollection.Add(item);
            item.Owner = this;
            this.MarkAsChanged(item.RecordType);
        }

        /// <summary>
        /// Adds multiple items.
        /// </summary>
        /// <param name="items">Item.</param>
        /// <exception cref="System.ArgumentNullException">Items cannot be null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Item cannot be in other collection.</exception>
        /// <exception cref="System.NotSupportedException">Collection is read-only.</exception>
        public void AddRange(IEnumerable<Record> items) {
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

            foreach (var item in this.BaseCollection) {
                item.Owner = null;
            }
            this.BaseCollection.Clear();
            this.MarkAsChanged();
        }

        /// <summary>
        /// Determines whether the collection contains a specific item.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        public bool Contains(Record item) {
            if (item == null) { return false; }
            return this.BaseCollection.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from collection.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Record[] array, int arrayIndex) {
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
        public int IndexOf(Record item) {
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
        public void Insert(int index, Record item) {
            if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
            if (item.Owner != null) { throw new ArgumentOutOfRangeException(nameof(item), "Item cannot be in other collection."); }
            if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

            this.BaseCollection.Insert(index, item);
            item.Owner = this;
            this.MarkAsChanged(item.RecordType);
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly {
            get { return this.Owner?.Owner?.Owner?.IsReadOnly ?? false; }
        }

        /// <summary>
        /// Removes the item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <exception cref="System.ArgumentNullException">Item cannot be null.</exception>
        /// <exception cref="System.NotSupportedException">Collection is read-only.</exception>
        public bool Remove(Record item) {
            if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
            if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

            if (this.BaseCollection.Remove(item)) {
                item.Owner = null;
                this.MarkAsChanged(item.RecordType);
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

            var item = this[index];
            this.BaseCollection.Remove(item);
            item.Owner = null;
            this.MarkAsChanged(item.RecordType);
        }

        /// <summary>
        /// Exposes the enumerator, which supports a simple iteration over a collection of a specified type.
        /// </summary>
        public IEnumerator<Record> GetEnumerator() {
            var items = new List<Record>(this.BaseCollection); //to avoid exception if collection is changed while in foreach
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
        public Record this[int index] {
            get { return this.BaseCollection[index]; }
            set {
                if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
                if (value == null) { throw new ArgumentNullException(nameof(value), "Value cannot be null."); }
                if (value.Owner != null) { throw new ArgumentOutOfRangeException(nameof(value), "Item cannot be in other collection."); }
                if (this.Contains(value)) { throw new ArgumentOutOfRangeException(nameof(value), "Duplicate item in collection."); }

                var item = this.BaseCollection[index];
                item.Owner = null;
                this.BaseCollection.RemoveAt(index);
                this.BaseCollection.Insert(index, value);
                value.Owner = this;
                this.MarkAsChanged(value.RecordType);
            }
        }

        #endregion


        #region ICollection extra

        /// <summary>
        /// Determines whether the collection contains a specific type.
        /// </summary>
        /// <param name="type">The item type to locate.</param>
        public bool Contains(RecordType type) {
            foreach (var item in this.BaseCollection) {
                if (item.RecordType == type) { return true; }
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
        public Record this[RecordType type] {
            get {
                foreach (var field in this.BaseCollection) {
                    if (field.RecordType == type) {
                        return field;
                    }
                }

                if (this.IsReadOnly) { return new Record(type); } //return dummy record if collection is read-only 

                var newField = new Record(this, type); //create a new field if one cannot be found

                int i = this.BaseCollection.Count;
                for (i = 0; i < this.BaseCollection.Count; i++) {
                    if (this.BaseCollection[i].RecordType > type) { break; }
                }
                this.BaseCollection.Insert(i, newField); //insert it in order (does not change order for existing ones)

                return newField;
            }
            set {
                if (this.IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
                if (value != null) { throw new ArgumentOutOfRangeException(nameof(value), "Only null value is supported."); }

                Record fieldToRemove = null;
                foreach (var field in this.BaseCollection) {
                    if (field.RecordType == type) {
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
