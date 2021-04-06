// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.KeySecrets;
using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Azure
{
    public class AzureGenericHostKeySecretProvider : IGenericHostKeySecretProvider
    {
        public IConfigurationBuilder AddKeyVaultToConfigurationInner(
            IConfigurationBuilder configurationBuilder,
            string vaultName)
        {
            SecretClient secretClient =
                SecretClientFactory.CreateSecretClient(vaultName);
            configurationBuilder.AddAzureKeyVault(
                secretClient,
                new KeyVaultSecretManager());
            return configurationBuilder;
        }
    }
}
