namespace Bimil;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Collection of record fields.
/// Does not contain EndOfEntry field.
/// </summary>
[DebuggerDisplay("{Count} record fields")]
public class FieldCollection : IList<Field> {

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="fields">Field collection.</param>
    /// <param name="isReadOnly">If true, collection is readonly.</param>
    internal FieldCollection(ICollection<Field> fields)
        : this(fields, isReadOnly: false) {
    }

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="fields">Field collection.</param>
    /// <param name="isReadOnly">If true, collection is readonly.</param>
    internal FieldCollection(ICollection<Field> fields, bool isReadOnly) {
        if ((fields != null) && (fields.Count > 0)) { BaseCollection.AddRange(fields); }
        _IsReadOnly = isReadOnly;
    }


    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private readonly List<Field> BaseCollection = [];


    #region ICollection

    /// <summary>
    /// Adds an item.
    /// </summary>
    /// <param name="item">Item.</param>
    /// <exception cref="ArgumentNullException">Item cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Item cannot be in other collection.</exception>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public void Add(Field item) {
        if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
        if (item.Type is FieldType.EndOfEntry) { throw new NotSupportedException("Cannot add EndOfEntry field."); }

        BaseCollection.Add(item);
    }

    /// <summary>
    /// Adds multiple items.
    /// </summary>
    /// <param name="items">Item.</param>
    /// <exception cref="ArgumentNullException">Items cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Item cannot be in other collection.</exception>
    /// <exception cref="NotSupportedException">Collection is read-only.</exception>
    public void AddRange(IEnumerable<Field> items) {
        if (items == null) { throw new ArgumentNullException(nameof(items), "Item cannot be null."); }
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }

        foreach (var item in items) {
            if (item.Type is FieldType.EndOfEntry) { throw new NotSupportedException("Cannot add EndOfEntry field."); }
        }

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
    public bool Contains(Field item) {
        if (item == null) { return false; }
        return BaseCollection.Contains(item);
    }

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from collection.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(Field[] array, int arrayIndex) {
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
    public int IndexOf(Field item) {
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
    public void Insert(int index, Field item) {
        if (item == null) { throw new ArgumentNullException(nameof(item), "Item cannot be null."); }
        if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
        if (item.Type is FieldType.EndOfEntry) { throw new NotSupportedException("Cannot add EndOfEntry field."); }

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
    public bool Remove(Field item) {
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
    public IEnumerator<Field> GetEnumerator() {
        var items = new List<Field>(BaseCollection); //to avoid exception if collection is changed while in foreach
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
    public Field this[int index] {
        get { return BaseCollection[index]; }
        set {
            if (value == null) { throw new ArgumentNullException(nameof(value), "Value cannot be null."); }
            if (IsReadOnly) { throw new NotSupportedException("Collection is read-only."); }
            if (Contains(value)) { throw new ArgumentOutOfRangeException(nameof(value), "Duplicate item in collection."); }
            if (value.Type is FieldType.EndOfEntry) { throw new NotSupportedException("Cannot add EndOfEntry field."); }

            var item = BaseCollection[index];
            BaseCollection.RemoveAt(index);
            BaseCollection.Insert(index, value);
        }
    }

    #endregion


    #region ICollection extra

    /// <summary>
    /// Gets field based on a type.
    /// If multiple elements exist with the same record field type, the first one is returned.
    /// If type does not exist, it is created.
    /// </summary>
    /// <param name="type">Field type.</param>
    public Field? this[FieldType type] {
        get {
            foreach (var field in BaseCollection) {
                if (field.Type == type) {
                    return field;
                }
            }
            return null;
        }
    }

    #endregion

}
