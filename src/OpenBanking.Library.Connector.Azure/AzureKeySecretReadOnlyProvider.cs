// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace FinnovationLabs.OpenBanking.Library.Connector.Azure
{
    public class AzureKeySecretReadOnlyProvider : IKeySecretReadOnlyProvider
    {
        public Task<KeySecret> GetKeySecretAsync(string key) =>
            GetKeySecretAsync(vaultName: KeySecret.DefaultVaultName, key: key);

        public async Task<KeySecret> GetKeySecretAsync(string vaultName, string key)
        {
            var value = await GetSecretAsync(vaultName: vaultName, key: key);

            return new KeySecret(vaultName: vaultName, key: key, value: value);
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
