// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;

public class OBSealKey
{
    public OBSealKey(string keyId, string key)
    {
        KeyId = keyId;
        Key = key;
    }

    // Open Banking Signing Key ID as string, e.g. "ABC"
    public string KeyId { get; }

    // Open Banking Signing Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
    public string Key { get; }
}
