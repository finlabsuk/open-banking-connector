// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.KeySecrets;
using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;

public static class ConfigurationBuilderExtensions
{
    /// <summary>
    ///     Add Key Vault to .NET Generic Host app configuration.
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="keySecretProvider"></param>
    /// <param name="vaultName"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddKeyVault(
        this IConfigurationBuilder configurationBuilder,
        IGenericHostKeySecretProvider keySecretProvider,
        string vaultName) => keySecretProvider.AddKeyVaultToConfigurationInner(configurationBuilder, vaultName);
}
