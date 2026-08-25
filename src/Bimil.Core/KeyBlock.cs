namespace Bimil;

using System;
using System.Security.Cryptography;

/// <summary>
/// Key block.
/// </summary>
public sealed class KeyBlock {

    private KeyBlock(byte[] salt, uint iterationCount, byte[] encryptionKey, byte[] authenticationKey, byte[]? passphrase, bool zeroBytes) {
        Salt = new ProtectedBytes(salt, zeroBytes);
        IterationCount = iterationCount;
        EncryptionKey = new ProtectedBytes(encryptionKey, zeroBytes);
        AuthenticationKey = new ProtectedBytes(authenticationKey, zeroBytes);
        Passphrase = (passphrase != null) ? new ProtectedBytes(passphrase, zeroBytes) : new ProtectedBytes();
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
    public ProtectedBytes EncryptionKey { get; }

    /// <summary>
    /// Gets authentication key.
    /// </summary>
    public ProtectedBytes AuthenticationKey { get; }

    /// <summary>
    /// Gets passphrase.
    /// </summary>
    public ProtectedBytes Passphrase { get; }

    /// <summary>
    /// Gets if passphrase exists.
    /// </summary>
    public bool HasPassphrase { get { return Passphrase.Length > 0; } }


    /// <summary>
    /// Gets minimum iteration count.
    /// </summary>
    public static readonly uint MinimumIterationCount = 262144;

    /// <summary>
    /// Gets default iteration count.
    /// </summary>
    public static readonly uint DefaultIterationCount = 2 * MinimumIterationCount;


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
