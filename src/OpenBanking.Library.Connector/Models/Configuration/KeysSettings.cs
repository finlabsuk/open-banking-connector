// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

/// <summary>
///     Encryption key used for storage of sensitive data in database.
/// </summary>
public class EncryptionKey
{
    /// <summary>
    ///     Encryption key (256-bit) used for symmetric encryption (AES-256-GCM) of sensitive data in database such as tokens.
    ///     Specified as a base64-encoded string.
    ///     For example, the Kubernetes
    ///     docs (https://kubernetes.io/docs/tasks/administer-cluster/encrypt-data/#encrypting-your-data) suggest such
    ///     a string could be generated on Linux using the command `head -c 32 /dev/urandom | base64`.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}

public class KeysSettings : ISettings<KeysSettings>
{
    public Dictionary<string, EncryptionKey> Encryption { get; set; } = new();

    /// <summary>
    ///     Encryption key to use for encrypting new objects. Specified by key ID.
    /// </summary>
    public string CurrentEncryptionKeyId { get; set; } = string.Empty;

    /// <summary>
    ///     Disable encryption of new objects (not recommended).
    /// </summary>
    public bool DisableEncryption { get; set; }

    public string SettingsGroupName => "OpenBankingConnector:Keys";

    public KeysSettings Validate()
    {
        if (DisableEncryption)
        {
            return this;
        }

        if (!Encryption.Any())
        {
            throw new ArgumentException(
                "Configuration or key secrets error: " +
                "No encryption keys provided.");
        }

        foreach ((string key, EncryptionKey value) in Encryption)
        {
            if (string.IsNullOrEmpty(value.Value))
            {
                throw new ArgumentException(
                    "Configuration or key secrets error: " +
                    $"No non-empty Value provided for encryption key {key}.");
            }
        }

        if (string.IsNullOrEmpty(CurrentEncryptionKeyId))
        {
            throw new ArgumentException(
                "Configuration or key secrets error: " +
                "No non-empty value provided for CurrentEncryptionKeyId.");
        }

        if (!Encryption.TryGetValue(CurrentEncryptionKeyId, out EncryptionKey? _))
        {
            throw new ArgumentException(
                "Configuration or key secrets error: " +
                $"Encryption key with ID {CurrentEncryptionKeyId} (specified by CurrentEncryptionKeyId) not found.");
        }

        return this;
    }
}
