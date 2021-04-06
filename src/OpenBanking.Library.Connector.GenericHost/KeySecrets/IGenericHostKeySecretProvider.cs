// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.KeySecrets
{
    /// <summary>
    ///     Abstraction for provider of key secrets in .NET Generic Core app.
    /// </summary>
    public interface IGenericHostKeySecretProvider
    {
        /// <summary>
        ///     Add Key Vault to .NET Generic Host app configuration.
        ///     Inner method intended to be wrapped as extension to IConfigurationBuilder.
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <param name="vaultName"></param>
        /// <returns></returns>
        IConfigurationBuilder AddKeyVaultToConfigurationInner(
            IConfigurationBuilder configurationBuilder,
            string vaultName);
    }
}
