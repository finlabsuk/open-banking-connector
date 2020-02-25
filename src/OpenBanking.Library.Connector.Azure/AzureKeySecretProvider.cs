// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace FinnovationLabs.OpenBanking.Library.Connector.Azure
{
    public class AzureKeySecretProvider : IKeySecretProvider
    {
        public Task<KeySecret> GetKeySecretAsync(string key) => GetKeySecretAsync(KeySecret.DefaultVaultName, key);

        public async Task<KeySecret> GetKeySecretAsync(string vaultName, string key)
        {
            var value = await GetSecretAsync(vaultName, key);

            return new KeySecret(vaultName, key, value);
        }


        private async Task<string> GetSecretAsync(string vaultName, string key)
        {
            var tokenProvider = new AzureServiceTokenProvider();

            using (var keyVaultClient =
                new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback)))
            {
                var secret = await keyVaultClient
                    .GetSecretAsync($"https://{vaultName}.vault.azure.net/secrets/{key}")
                    .ConfigureAwait(false);

                return secret.Value;
            }
        }
    }
}
