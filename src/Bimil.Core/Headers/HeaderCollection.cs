namespace Bimil;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Collection of header fields.
/// Does not contain EndOfEntry field.
/// </summary>
[DebuggerDisplay("{Count} header fields")]
public class HeaderCollection : IList<Header> {

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="headers">Header field collection.</param>
    internal HeaderCollection(ICollection<Header> headers)
        : this(headers, isReadOnly: false) {
    }

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="headers">Header field collection.</param>
    /// <param name="isReadOnly">If true, collection is readonly.</param>
    internal HeaderCollection(ICollection<Header> headers, bool isReadOnly) {
        if ((headers != null) && (headers.Count > 0)) { BaseCollection.AddRange(headers); }
        _IsReadOnly = isReadOnly;

        // ensure first field is always Version
        if (this[HeaderType.Version] is VersionHeader versionField) {
            var versionIndex = IndexOf(versionField);
            if (versionIndex > 0) {
                BaseCollection.RemoveAt(versionIndex);
                BaseCollection.Insert(0, versionField);
            }
        } else {
            throw new ArgumentOutOfRangeException(nameof(headers), "Version field is missing.");
        }
    }


    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private readonly List<Header> BaseCollection = [];


    #region ICollection

    /// <summary>
    /// Adds an item.
    /// </summary>
    /// <param name="item">Item.</param>
    /// <exception cref="ArgumentNullException">Item cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Item cannot be in other collection.</exception>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public void Add(Header item) {
        if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
        if (item.Type is HeaderType.EndOfEntry) { throw new NotSupportedException("Cannot add EndOfEntry header field."); }
        if (item.Type is HeaderType.Version) { throw new NotSupportedException("Cannot have multiple Version header fields."); }

        BaseCollection.Add(item);
    }

    /// <summary>
    /// Adds multiple items.
    /// </summary>
    /// <param name="items">Item.</param>
    /// <exception cref="ArgumentNullException">Items cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Item cannot be in other collection.</exception>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public void AddRange(IEnumerable<Header> items) {
        if (items == null) { throw new ArgumentNullException(nameof(items), "Item cannot be null."); }
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

        foreach (var item in items) {
            if (item.Type is HeaderType.EndOfEntry) { throw new NotSupportedException("Cannot add EndOfEntry header field."); }
        }

        BaseCollection.AddRange(items);
    }

    /// <summary>
    /// Removes all items except for Version field.
    /// </summary>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public void Clear() {
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

        for (var i = BaseCollection.Count - 1; i > 0; i--) {  // remove all except the first field (Version)
            BaseCollection.RemoveAt(i);
        }
    }

    /// <summary>
    /// Determines whether the collection contains a specific item.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    public bool Contains(Header item) {
        if (item == null) { return false; }
        return BaseCollection.Contains(item);
    }

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from collection.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(Header[] array, int arrayIndex) {
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
    public int IndexOf(Header item) {
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
    public void Insert(int index, Header item) {
        if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
        if (item.Type is HeaderType.EndOfEntry) { throw new NotSupportedException("Cannot add EndOfEntry header field."); }
        if ((index == 0) && (item.Type != HeaderType.Version)) { throw new ArgumentOutOfRangeException(nameof(index), "Version must be the first header field."); }

        BaseCollection.Insert(index, item);
    }

    private readonly bool _IsReadOnly;
    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    public bool IsReadOnly {
        get { return _IsReadOnly; }
    }

    /// <summary>
    /// Removes the item from the collection.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <exception cref="ArgumentNullException">Item cannot be null.</exception>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public bool Remove(Header item) {
        if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
        if ((item.Type == HeaderType.Version) && BaseCollection.IndexOf(item) == 0) { throw new ArgumentOutOfRangeException(nameof(item), "Cannot remove the first version header field."); }

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
        if (index == 0) { throw new ArgumentOutOfRangeException(nameof(index), "Cannot remove the first version header field."); }

        var item = this[index];
        BaseCollection.Remove(item);
    }

    /// <summary>
    /// Exposes the enumerator, which supports a simple iteration over a collection of a specified type.
    /// </summary>
    public IEnumerator<Header> GetEnumerator() {
        var items = new List<Header>(BaseCollection);  // to avoid exception if collection is changed while in foreach
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
    public Header this[int index] {
        get { return BaseCollection[index]; }
        set {
            if (value == null) { throw new ArgumentNullException(nameof(value), "Value cannot be null."); }
            if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
            if (Contains(value)) { throw new ArgumentOutOfRangeException(nameof(value), "Duplicate item in collection."); }
            if (value.Type is HeaderType.EndOfEntry) { throw new NotSupportedException("Cannot add EndOfEntry header field."); }
            if ((index == 0) && (value.Type != HeaderType.Version)) { throw new ArgumentOutOfRangeException(nameof(value), "Version must be the first header field."); }

            var item = BaseCollection[index];
            BaseCollection.RemoveAt(index);
            BaseCollection.Insert(index, value);
        }
    }

    #endregion


    #region ICollection extra

    /// <summary>
    /// Determines whether the collection contains a specific type.
    /// </summary>
    /// <param name="type">The item type to locate.</param>
    public bool Contains(HeaderType type) {
        foreach (var item in BaseCollection) {
            if (item.Type == type) { return true; }
        }
        return false;
    }

    /// <summary>
    /// Gets field based on a type.
    /// If multiple elements exist with the same header field type, the first one is returned.
    /// If type does not exist, it is created.
    /// </summary>
    /// <param name="type">Header field type.</param>
    public Header? this[HeaderType type] {
        get {
            foreach (var field in BaseCollection) {
                if (field.Type == type) {
                    return field;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Removes the item from the collection.
    /// </summary>
    /// <param name="type">Header field type.</param>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public bool Remove(HeaderType type) {
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

        Header? fieldToRemove = null;
        foreach (var field in BaseCollection) {
            if (field.Type == type) {
                fieldToRemove = field;
                break;
            }
        }
        if (fieldToRemove != null) {
            return Remove(fieldToRemove);
        }
        return false;  // not found
    }

    #endregion

}
