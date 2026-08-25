namespace Bimil;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;

/// <summary>
/// Collection of records.
/// </summary>
[DebuggerDisplay("{Count} records")]
public class KeyBlockCollection : IReadOnlyCollection<KeyBlock> {

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="keyBlocks">Key block collection.</param>
    internal KeyBlockCollection(Document document, ICollection<KeyBlock> keyBlocks) {
        Document = document;
        if ((keyBlocks != null) && (keyBlocks.Count > 0)) { BaseCollection.AddRange(keyBlocks); }
    }


    private readonly Document Document;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private readonly List<KeyBlock> BaseCollection = [];


    #region ICollection

    /// <summary>
    /// Determines whether the collection contains a specific item.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    public bool Contains(KeyBlock item) {
        if (item == null) { return false; }
        return BaseCollection.Contains(item);
    }

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from collection.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(KeyBlock[] array, int arrayIndex) {
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
    public int IndexOf(KeyBlock item) {
        return BaseCollection.IndexOf(item);
    }

    /// <summary>
    /// Exposes the enumerator, which supports a simple iteration over a collection of a specified type.
    /// </summary>
    public IEnumerator<KeyBlock> GetEnumerator() {
        var items = new List<KeyBlock>(BaseCollection); //to avoid exception if collection is changed while in foreach
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
    public KeyBlock this[int index] {
        get { return BaseCollection[index]; }
    }

    #endregion


    /// <summary>
    /// Clear all entries except for the active block.
    /// </summary>
    public void Clear() {
        for (var i = BaseCollection.Count - 1; i >= 0; i--) {
            var keyBlock = BaseCollection[i];
            if (!keyBlock.Equals(Document.ActiveKeyBlock)) { BaseCollection.RemoveAt(i); }
        }
    }

    /// <summary>
    /// Removes a keyblock
    /// </summary>
    /// <param name="keyBlock">Key block to remove.</param>
    public void Remove(KeyBlock keyBlock) {
        ArgumentNullException.ThrowIfNull(keyBlock);
        if (keyBlock.Equals(Document.ActiveKeyBlock)) { throw new InvalidOperationException("Cannot remove active key block."); }
        BaseCollection.Remove(keyBlock);
    }

    /// <summary>
    /// Appends a new key.
    /// </summary>
    /// <param name="passphrase">Passphrase.</param>
    public void AppendNew(byte[] passphrase) {
        AppendNew(passphrase, zeroBytes: false);
    }

    /// <summary>
    /// Appends a new key.
    /// </summary>
    /// <param name="passphrase">Passphrase.</param>
    /// <param name="zeroBytes">If true, passed bytes will be zeroed out.</param>
    public void AppendNew(byte[] passphrase, bool zeroBytes) {
        if (Document.DatabaseVersion == DatabaseVersion.V3) { throw new NotSupportedException("Multiple keys are not supported for database V3"); }

        var salt = new byte[32]; RandomNumberGenerator.Fill(salt);
        var keyK = Document.ActiveKeyBlock.EncryptionKey.GetBytes();
        var keyL = Document.ActiveKeyBlock.AuthenticationKey.GetBytes();
        try {
            var keyBlock = KeyBlock.Create(salt, KeyBlock.DefaultIterationCount, keyK, keyL, passphrase, zeroBytes);
            BaseCollection.Add(keyBlock);
        } finally {
            CryptographicOperations.ZeroMemory(salt);
            CryptographicOperations.ZeroMemory(keyK);
            CryptographicOperations.ZeroMemory(keyL);
        }
    }

}
