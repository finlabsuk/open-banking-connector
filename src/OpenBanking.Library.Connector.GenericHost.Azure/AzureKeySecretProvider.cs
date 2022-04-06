// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure;
using Azure.Security.KeyVault.Secrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;

namespace FinnovationLabs.OpenBanking.Library.Connector.Azure
{
    public class AzureKeySecretProvider : IKeySecretProvider
    {
        public Task<KeySecret?> GetKeySecretAsync(string key) =>
            GetKeySecretAsync(KeySecret.DefaultVaultName, key);

        public async Task<KeySecret?> GetKeySecretAsync(string vaultName, string key)
        {
            string? value = await GetSecretAsync(vaultName, key);

            return value is null ? null : new KeySecret(vaultName, key, value);
        }


        private async Task<string?> GetSecretAsync(string vaultName, string key)
        {
            SecretClient client = SecretClientFactory.CreateSecretClient(vaultName);
            try
            {
                Response<KeyVaultSecret> secret = await client
                    .GetSecretAsync($"{key}");
                return secret.Value.Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }
    }
}
