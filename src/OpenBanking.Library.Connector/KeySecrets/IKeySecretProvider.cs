// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;

/// <summary>
///     Basic abstraction for provider of key secrets when IConfiguration not used to provide
///     key secrets.
/// </summary>
public interface IKeySecretProvider
{
    Task<KeySecret?> GetKeySecretAsync(string vaultName, string key);

    Task<KeySecret?> GetKeySecretAsync(string key);
}
