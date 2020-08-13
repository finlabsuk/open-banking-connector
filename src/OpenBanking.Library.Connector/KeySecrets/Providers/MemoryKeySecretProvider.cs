// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers
{
    public class MemoryKeySecretProvider : IKeySecretReadOnlyProvider, IKeySecretProvider
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, KeySecret>> _cache;

        public MemoryKeySecretProvider(IEnumerable<KeySecret> secrets)
            : this()
        {
            foreach (KeySecret secret in secrets.ArgNotNull(nameof(secrets)))
            {
                ConcurrentDictionary<string, KeySecret> vault = _cache.GetOrAdd(
                    key: secret.VaultName,
                    value: new ConcurrentDictionary<string, KeySecret>(StringComparer.InvariantCultureIgnoreCase));

                vault.TryAdd(key: secret.Key, value: secret);
            }
        }

        public MemoryKeySecretProvider()
        {
            _cache = new ConcurrentDictionary<string, ConcurrentDictionary<string, KeySecret>>(
                StringComparer
                    .InvariantCultureIgnoreCase);
        }

        public Task SetKeySecretAsync(KeySecret keySecret)
        {
            ConcurrentDictionary<string, KeySecret> vault = _cache.GetOrAdd(
                key: keySecret.VaultName,
                value: new ConcurrentDictionary<string, KeySecret>(StringComparer.InvariantCultureIgnoreCase));

            vault.TryAdd(key: keySecret.Key, value: keySecret);

            return Task.CompletedTask;
        }


        public Task<KeySecret> GetKeySecretAsync(string key)
        {
            key.ArgNotNull(nameof(key));
            return GetKeySecretAsync(vaultName: KeySecret.DefaultVaultName, key: key);
        }

        public Task<KeySecret> GetKeySecretAsync(string vaultName, string key)
        {
            vaultName.ArgNotNull(nameof(vaultName));
            key.ArgNotNull(nameof(key));

            if (_cache.TryGetValue(key: vaultName, value: out ConcurrentDictionary<string, KeySecret> vault) &&
                vault.TryGetValue(key: key, value: out KeySecret secret))
            {
                return secret.ToTaskResult();
            }

            return ((KeySecret) null).ToTaskResult();
        }
    }
}
