namespace Bimil;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Collection of records.
/// </summary>
[DebuggerDisplay("{Count} records")]
public class RecordCollection : IList<Record> {

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="records">Record collection.</param>
    internal RecordCollection(ICollection<Record> records)
        : this(records, isReadOnly: false) {
    }

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="records">Record collection.</param>
    /// <param name="isReadOnly">If true, collection is readonly.</param>
    internal RecordCollection(ICollection<Record> records, bool isReadOnly) {
        if ((records != null) && (records.Count > 0)) { BaseCollection.AddRange(records); }
        IsReadOnly = isReadOnly;

    }


    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private readonly List<Record> BaseCollection = [];


    #region ICollection

    /// <summary>
    /// Adds an item.
    /// </summary>
    /// <param name="item">Item.</param>
    /// <exception cref="ArgumentNullException">Item cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Item cannot be in other collection.</exception>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public void Add(Record item) {
        if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

        BaseCollection.Add(item);
    }

    /// <summary>
    /// Adds multiple items.
    /// </summary>
    /// <param name="items">Item.</param>
    /// <exception cref="ArgumentNullException">Items cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Item cannot be in other collection.</exception>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public void AddRange(IEnumerable<Record> items) {
        if (items == null) { throw new ArgumentNullException(nameof(items), "Item cannot be null."); }
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

        BaseCollection.AddRange(items);
    }

    /// <summary>
    /// Removes all items.
    /// </summary>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public void Clear() {
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

        BaseCollection.Clear();
    }

    /// <summary>
    /// Determines whether the collection contains a specific item.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    public bool Contains(Record item) {
        if (item == null) { return false; }
        return BaseCollection.Contains(item);
    }

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from collection.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(Record[] array, int arrayIndex) {
        BaseCollection.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Gets the number of items contained in the collection.
    /// </summary>
    public int Count {
        get { return BaseCollection.Count; }
    }

    /// <summary>
    /// Searches for the specified item and returns the zero-based index of the first occurrence within the entire collection.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    public int IndexOf(Record item) {
        return BaseCollection.IndexOf(item);
    }

    /// <summary>
    /// Inserts an element into the collection at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The item to insert.</param>
    /// <exception cref="ArgumentNullException">Item cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Index is less than 0. -or- Index is greater than collection count.</exception>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public void Insert(int index, Record item) {
        if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

        BaseCollection.Insert(index, item);
    }

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    public bool IsReadOnly {
        get;
        private set;
    }

    /// <summary>
    /// Removes the item from the collection.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <exception cref="ArgumentNullException">Item cannot be null.</exception>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public bool Remove(Record item) {
        if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

        return BaseCollection.Remove(item);
    }

    /// <summary>
    /// Removes the element at the specified index of the collection.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">Index is less than 0. -or- Index is equal to or greater than collection count.</exception>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public void RemoveAt(int index) {
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

        var item = this[index];
        BaseCollection.Remove(item);
    }

    /// <summary>
    /// Exposes the enumerator, which supports a simple iteration over a collection of a specified type.
    /// </summary>
    public IEnumerator<Record> GetEnumerator() {
        var items = new List<Record>(BaseCollection); //to avoid exception if collection is changed while in foreach
        foreach (var item in items) {
            yield return item;
        }
    }

    /// <summary>
    /// Exposes the enumerator, which supports a simple iteration over a non-generic collection.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <exception cref="ArgumentNullException">Value cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Index is less than 0. -or- Index is equal to or greater than collection count. -or- Duplicate name in collection.</exception>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public Record this[int index] {
        get { return BaseCollection[index]; }
        set {
            if (value == null) { throw new ArgumentNullException(nameof(value), "Value cannot be null."); }
            if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
            if (Contains(value)) { throw new ArgumentOutOfRangeException(nameof(value), "Duplicate item in collection."); }

            var item = BaseCollection[index];
            BaseCollection.RemoveAt(index);
            BaseCollection.Insert(index, value);
        }
    }

    #endregion


    /// <summary>
    /// Returns entry record if one is found or null.
    /// </summary>
    /// <param name="uuid">UUID.</param>
    public EntryRecord? FindEntryRecord(Guid uuid) {
        foreach (var record in this) {
            if (record is EntryRecord entryRecord && (entryRecord.Uuid == uuid)) { return entryRecord; }
        }
        return null;
    }

    /// <summary>
    /// Returns alias record if one is found or null.
    /// </summary>
    /// <param name="uuid">UUID.</param>
    public AliasRecord? FindAliasRecord(Guid uuid) {
        foreach (var record in this) {
            if (record is AliasRecord aliasRecord && (aliasRecord.Uuid == uuid)) { return aliasRecord; }
        }
        return null;
    }

    /// <summary>
    /// Returns shortcut record if one is found or null.
    /// </summary>
    /// <param name="uuid">UUID.</param>
    public ShortcutRecord? FindShortcutRecord(Guid uuid) {
        foreach (var record in this) {
            if (record is ShortcutRecord shortcutRecord && (shortcutRecord.Uuid == uuid)) { return shortcutRecord; }
        }
        return null;
    }

    /// <summary>
    /// Returns attachment record if one is found or null.
    /// </summary>
    /// <param name="uuid">UUID.</param>
    public AttachmentRecord? FindAttachemntRecord(Guid uuid) {
        foreach (var record in this) {
            if (record is AttachmentRecord attachmentRecord && (attachmentRecord.Uuid == uuid)) { return attachmentRecord; }
        }
        return null;
    }

}
