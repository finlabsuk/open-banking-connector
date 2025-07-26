// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Services;

public class SettingsService : ISettingsService
{
    private readonly Lock _currentEncryptionKeyIdLock = new();
    private readonly Lock _disableEncryptionLock = new();

    private Guid? _currentEncryptionKeyId;
    private bool _disableEncryption;

    public Guid? CurrentEncryptionKeyId
    {
        get
        {
            lock (_currentEncryptionKeyIdLock)
            {
                return _currentEncryptionKeyId;
            }
        }
        set
        {
            lock (_currentEncryptionKeyIdLock)
            {
                _currentEncryptionKeyId = value;
            }
        }
    }

    public bool DisableEncryption
    {
        get
        {
            lock (_disableEncryptionLock)
            {
                return _disableEncryption;
            }
        }
        set
        {
            lock (_disableEncryptionLock)
            {
                _disableEncryption = value;
            }
        }
    }
}
