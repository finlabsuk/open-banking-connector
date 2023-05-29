// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
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
    ///     Encryption "nonce".
    /// </summary>
    [Column("nonce")]
    private byte[] _nonce;

    /// <summary>
    ///     Tag
    /// </summary>
    [Column("tag")]
    private byte[] _tag;

    /// <summary>
    ///     Text
    /// </summary>
    [Column("text")]
    private string _text;

    protected EncryptedObject(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        DateTimeOffset modified,
        string? modifiedBy,
        string? keyId,
        string plainText,
        string associatedData,
        byte[] encryptionKey) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy)
    {
        UpdatePlainText(plainText, associatedData, encryptionKey, modified, modifiedBy, keyId);
    }

    public DateTimeOffset Modified { get; private set; }

    public string? ModifiedBy { get; private set; }

    /// <summary>
    ///     Encryption key ID.
    /// </summary>
    public string? KeyId { get; private set; }

    /// <summary>
    ///     Update plain text with new value.
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="associatedData"></param>
    /// <param name="encryptionKey"></param>
    /// <param name="modified"></param>
    /// <param name="modifiedBy"></param>
    /// <param name="keyId"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [MemberNotNull(nameof(_text))]
    [MemberNotNull(nameof(_tag))]
    [MemberNotNull(nameof(_nonce))]
    protected void UpdatePlainText(
        string plainText,
        string associatedData,
        byte[] encryptionKey,
        DateTimeOffset modified,
        string? modifiedBy,
        string? keyId)
    {
        Modified = modified;
        ModifiedBy = modifiedBy;
        KeyId = keyId;

        // Create nonce
        int nonceLengthBytes = AesGcm.NonceByteSizes.MaxSize;
        if (nonceLengthBytes != 12)
        {
            throw new InvalidOperationException();
        }

        _nonce = new byte[nonceLengthBytes];
        RandomNumberGenerator.Fill(_nonce);


        if (KeyId is null)
        {
            _text = plainText;
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
                Encoding.UTF8.GetBytes(plainText),
                tagLengthBytes,
                Encoding.UTF8.GetBytes(associatedData),
                encryptionKey);

            _text = Convert.ToBase64String(cipherText);
            _tag = tag;
        }
    }

    /// <summary>
    ///     Get plain text value.
    /// </summary>
    /// <param name="associatedData"></param>
    /// <param name="encryptionKey"></param>
    /// <returns></returns>
    protected string GetPlainText(string associatedData, byte[] encryptionKey) =>
        KeyId is not null
            ? Encoding.UTF8.GetString(
                AesGcmDecrypt(
                    _nonce,
                    Convert.FromBase64String(_text),
                    _tag,
                    Encoding.UTF8.GetBytes(associatedData),
                    encryptionKey))
            : _text;

    private static (byte[] cipherText, byte[] tag) AesGcmEncrypt(
        byte[] nonce,
        byte[] plainText,
        int tagLengthBytes,
        byte[] associatedData,
        byte[] encryptionKey)
    {
        using var aesGcm = new AesGcm(encryptionKey);

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
        byte[] associatedData,
        byte[] encryptionKey)
    {
        using var aesGcm = new AesGcm(encryptionKey);

        // Decrypt text
        var plainText = new byte[cipherText.Length];
        aesGcm.Decrypt(nonce, cipherText, tag, plainText, associatedData);

        return plainText;
    }
}
