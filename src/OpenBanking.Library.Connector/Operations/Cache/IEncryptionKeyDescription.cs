// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;

public interface IEncryptionKeyDescription
{
    public Task<byte[]> GetEncryptionKey(Guid? keyId);

    Guid? GetCurrentKeyId();

    void Set(Guid entityId, EncryptionKeyDescription encryptionKeyDescription);
}
