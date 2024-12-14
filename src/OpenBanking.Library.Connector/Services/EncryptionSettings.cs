// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Services;

public class EncryptionSettings
{
    public Guid? CurrentKeyId { get; private set; }

    public bool DisableEncryption { get; private set; }

    public void SetCurrentKeyId(Guid currentKeyId)
    {
        CurrentKeyId = currentKeyId;
    }

    public void SetDisableEncryption(bool disableEncryption)
    {
        DisableEncryption = disableEncryption;
    }
}
