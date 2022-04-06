// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace FinnovationLabs.OpenBanking.Library.Connector.Azure
{
    internal static class SecretClientFactory
    {
        public static SecretClient CreateSecretClient(string vaultName)
        {
            return new SecretClient(
                new Uri($"https://{vaultName}.vault.azure.net/"),
                new DefaultAzureCredential());
        }
    }
}
