namespace Bimil;

using System;
using System.Text;

/// <summary>
/// Key block.
/// </summary>
public sealed class KeyBlock {

    private KeyBlock(byte[] salt, uint iterationCount, byte[] encryptionKey, byte[] authenticationKey, byte[]? passphrase, bool zeroBytes) {
        Salt = new ProtectedBytes(salt, zeroBytes) { IsReadOnly = true };
        IterationCount = iterationCount;
        KeyK = new ProtectedBytes(encryptionKey, zeroBytes) { IsReadOnly = true };
        KeyL = new ProtectedBytes(authenticationKey, zeroBytes) { IsReadOnly = true };
        Passphrase = (passphrase != null)
                   ? new ProtectedBytes(passphrase, zeroBytes) { IsReadOnly = true }
                   : new ProtectedBytes() { IsReadOnly = true };
    }


    /// <summary>
    /// Gets salt.
    /// </summary>
    public ProtectedBytes Salt { get; }

    /// <summary>
    /// Gets iteration count.
    /// </summary>
    public uint IterationCount { get; }

    /// <summary>
    /// Gets encryption key.
    /// </summary>
    public ProtectedBytes KeyK { get; }

    /// <summary>
    /// Gets authentication key.
    /// </summary>
    public ProtectedBytes KeyL { get; }

    /// <summary>
    /// Gets if block has key information present.
    /// For example, key L might not be present if no passphrase was used for opening.
    /// </summary>
    public bool HasKeys {
        get { return (Salt.Length > 0) && (KeyK.Length > 0) && (KeyL.Length > 0); }
    }


    /// <summary>
    /// Gets passphrase.
    /// </summary>
    public ProtectedBytes Passphrase { get; }

    /// <summary>
    /// Gets if passphrase exists.
    /// </summary>
    public bool HasPassphrase { get { return Passphrase.Length > 0; } }

    /// <summary>
    /// Change passphrase for the block.
    /// </summary>
    /// <param name="passphrase">Passphrase.</param>
    public void ChangePassphrase(string passphrase) {
        ChangePassphrase(Encoding.UTF8.GetBytes(passphrase), zeroBytes: true);
    }

    /// <summary>
    /// Change passphrase for the block.
    /// </summary>
    /// <param name="passphraseBytes">Passphrase bytes.</param>
    public void ChangePassphrase(byte[] passphraseBytes) {
        ChangePassphrase(passphraseBytes, zeroBytes: false);
    }

    /// <summary>
    /// Change passphrase for the block.
    /// </summary>
    /// <param name="passphraseBytes">Passphrase bytes.</param>
    /// <param name="zeroBytes">If true, bytes will be zeroed after password is set.</param>
    public void ChangePassphrase(byte[] passphraseBytes, bool zeroBytes) {
        ArgumentNullException.ThrowIfNull(passphraseBytes);
        if (passphraseBytes.Length == 0) { throw new ArgumentOutOfRangeException(nameof(passphraseBytes), "Passphrase cannot be empty."); }
        Passphrase.SetBytes(passphraseBytes, zeroBytes);
    }


    /// <summary>
    /// Gets minimum iteration count.
    /// </summary>
    internal static readonly uint MinimumIterationCount = 262144;

    /// <summary>
    /// Gets default iteration count.
    /// </summary>
    internal static readonly uint DefaultIterationCount = 2 * MinimumIterationCount;


    /// <summary>
    /// Create a new key block.
    /// </summary>
    /// <param name="salt">Salt.</param>
    /// <param name="iterationCount">Iteration count.</param>
    /// <param name="encryptionKey">Encryption key.</param>
    /// <param name="authenticationKey">Authentication key.</param>
    /// <param name="passphrase">Passphrase.</param>
    /// <param name="zeroBytes">If true, bytes will be zeroed.</param>
    public static KeyBlock Create(byte[] salt, uint iterationCount, byte[] encryptionKey, byte[] authenticationKey, byte[]? passphrase, bool zeroBytes) {
        ArgumentNullException.ThrowIfNull(salt);
        ArgumentNullException.ThrowIfNull(encryptionKey);
        ArgumentNullException.ThrowIfNull(authenticationKey);

        if (salt.Length != 32) { throw new ArgumentOutOfRangeException(nameof(salt), "Salt must be 32 bytes."); }
        if (iterationCount < 1) { throw new ArgumentOutOfRangeException(nameof(iterationCount), "Iteration count must be a positive number."); }
        if (encryptionKey.Length != 32) { throw new ArgumentOutOfRangeException(nameof(encryptionKey), "Encryption key must be 32 bytes."); }
        if (authenticationKey.Length != 32) { throw new ArgumentOutOfRangeException(nameof(authenticationKey), "Authentication key must be 32 bytes."); }

        return new KeyBlock(salt, iterationCount, encryptionKey, authenticationKey, passphrase, zeroBytes);
    }

}
