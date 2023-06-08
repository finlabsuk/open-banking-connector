// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Repositories;

public interface IEncryptionKeyInfo
{
    public byte[] GetEncryptionKey(string? keyId);

    string? GetCurrentKeyId();
}

public class EncryptionKeyInfo : IEncryptionKeyInfo
{
    public EncryptionKeyInfo(ISettingsProvider<KeysSettings> keySettingsProvider)
    {
        KeysSettings keysSettings = keySettingsProvider.GetSettings();

        if (keysSettings.DisableEncryption)
        {
            return;
        }

        // Get current key ID and check key present
        CurrentKeyId = keysSettings.CurrentEncryptionKeyId;
        if (!keysSettings.Encryption.TryGetValue(CurrentKeyId, out EncryptionKey? _))
        {
            throw new ArgumentException(
                "Configuration or key secrets error: " +
                $"Encryption key with ID {CurrentKeyId} as specified by CurrentEncryptionKeyId not found.");
        }

        // Copy key values to dictionary
        foreach ((string id, EncryptionKey key) in keysSettings.Encryption)
        {
            byte[] keyValue = Convert.FromBase64String(key.Value);

            if (keyValue.Length is not 32)
            {
                throw new ArgumentException(
                    "Configuration or key secrets error: " +
                    $"Encryption key with ID {id} is {keyValue.Length} bytes (require 32).");
            }

            KeyValues[id] = keyValue;
        }
    }

    public ConcurrentDictionary<string, byte[]> KeyValues { get; } = new();

    public string? CurrentKeyId { get; }

    public byte[] GetEncryptionKey(string? keyId)
    {
        if (keyId is null)
        {
            return Array.Empty<byte>();
        }

        if (!KeyValues.TryGetValue(keyId, out byte[]? key))
        {
            throw new ArgumentException(
                "Configuration or key secrets error: " +
                $"Encryption key with ID {keyId} not found.");
        }

        return key;
    }

    public string? GetCurrentKeyId() => CurrentKeyId;
}
