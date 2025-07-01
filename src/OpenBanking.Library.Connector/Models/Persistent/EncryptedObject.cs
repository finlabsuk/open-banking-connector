// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
[Index(nameof(_nonce), IsUnique = true)]
internal class EncryptedObject : BaseEntity
{
    /// <summary>
    ///     Encryption "nonce". Protected rather than private for EF Core visibility.
    /// </summary>
    [Column("nonce")]
    protected byte[] _nonce = null!;

    /// <summary>
    ///     Tag. Protected rather than private for EF Core visibility.
    /// </summary>
    [Column("tag")]
    protected byte[] _tag = null!;

    /// <summary>
    ///     Text. Protected rather than private for EF Core visibility.
    /// </summary>
    [Column("text")]
    protected byte[] _text = null!;

    /// <summary>
    ///     Text (string). Protected rather than private for EF Core visibility.
    /// </summary>
    [Column("text2")]
    protected string? _text2;

    /// <summary>
    ///     Constructor. Ideally would set all fields (full state) of class but unfortunately having parameters which don't
    ///     directly map to properties causes an issue for EF Core. Thus this constructor should be followed by a call
    ///     to <see cref="UpdatePlainText" />.
    /// </summary>
    public EncryptedObject(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy) { }

    public DateTimeOffset Modified { get; private set; }

    public string? ModifiedBy { get; private set; }

    [ForeignKey(nameof(EncryptionKeyDescriptionId))]
    public EncryptionKeyDescriptionEntity? EncryptionKeyDescriptionNavigation { get; private set; }

    public Guid? EncryptionKeyDescriptionId { get; set; }

    /// <summary>
    ///     Update plain text with new value.
    /// </summary>
    protected void UpdatePlainText(
        string plainText,
        string associatedData,
        byte[] encryptionKey,
        DateTimeOffset modified,
        string? modifiedBy,
        Guid? keyId)
    {
        Modified = modified;
        ModifiedBy = modifiedBy;
        EncryptionKeyDescriptionId = keyId;

        // Create nonce
        int nonceLengthBytes = AesGcm.NonceByteSizes.MaxSize;
        if (nonceLengthBytes != 12)
        {
            throw new InvalidOperationException();
        }

        _nonce = new byte[nonceLengthBytes];
        RandomNumberGenerator.Fill(_nonce);

        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        if (EncryptionKeyDescriptionId is null)
        {
            _text = plainTextBytes;
            _text2 = plainText;
            _tag = Array.Empty<byte>();
        }
        else
        {
            int tagLengthBytes = AesGcm.TagByteSizes.MaxSize;
            if (tagLengthBytes != 16)
            {
                throw new InvalidOperationException();
            }

            (byte[] cipherText, byte[] tag) = AesGcmEncrypt(
                _nonce,
                plainTextBytes,
                tagLengthBytes,
                Encoding.UTF8.GetBytes(associatedData),
                encryptionKey);

            _text = cipherText;
            _text2 = null;
            _tag = tag;
        }
    }

    /// <summary>
    ///     Get plain text value.
    /// </summary>
    protected string GetPlainText(string associatedData, byte[] encryptionKey)
    {
        int tagLengthBytes = AesGcm.TagByteSizes.MaxSize;
        if (tagLengthBytes != 16)
        {
            throw new InvalidOperationException();
        }
        return Encoding.UTF8.GetString(
            EncryptionKeyDescriptionId is not null
                ? AesGcmDecrypt(
                    _nonce,
                    _text,
                    _tag,
                    tagLengthBytes,
                    Encoding.UTF8.GetBytes(associatedData),
                    encryptionKey)
                : _text);
    }

    private static (byte[] cipherText, byte[] tag) AesGcmEncrypt(
        byte[] nonce,
        byte[] plainText,
        int tagLengthBytes,
        byte[] associatedData,
        byte[] encryptionKey)
    {
        using var aesGcm = new AesGcm(encryptionKey, tagLengthBytes);

        // Encrypt text
        var cipherText = new byte[plainText.Length];
        var tag = new byte[tagLengthBytes];
        aesGcm.Encrypt(nonce, plainText, cipherText, tag, associatedData);

        return (cipherText, tag);
    }

    private static byte[] AesGcmDecrypt(
        byte[] nonce,
        byte[] cipherText,
        byte[] tag,
        int tagLengthBytes,
        byte[] associatedData,
        byte[] encryptionKey)
    {
        using var aesGcm = new AesGcm(encryptionKey, tagLengthBytes);

        // Decrypt text
        var plainText = new byte[cipherText.Length];
        aesGcm.Decrypt(nonce, cipherText, tag, plainText, associatedData);

        return plainText;
    }
}
