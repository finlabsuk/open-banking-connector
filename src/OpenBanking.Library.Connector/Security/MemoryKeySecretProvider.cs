﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public class MemoryKeySecretProvider : IKeySecretProvider
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, KeySecret>> _cache;

        public MemoryKeySecretProvider(IEnumerable<KeySecret> secrets)
            : this()
        {
            foreach (var secret in secrets.ArgNotNull(nameof(secrets)))
            {
                var vault = _cache.GetOrAdd(secret.VaultName,
                    new ConcurrentDictionary<string, KeySecret>(StringComparer.InvariantCultureIgnoreCase));

                vault.TryAdd(secret.Key, secret);
            }
        }

        public MemoryKeySecretProvider()
        {
            _cache = new ConcurrentDictionary<string, ConcurrentDictionary<string, KeySecret>>(StringComparer
                .InvariantCultureIgnoreCase);
        }


        public Task<KeySecret> GetKeySecretAsync(string key)
        {
            key.ArgNotNull(nameof(key));
            return GetKeySecretAsync(KeySecret.DefaultVaultName, key);
        }

        public Task<KeySecret> GetKeySecretAsync(string vaultName, string key)
        {
            vaultName.ArgNotNull(nameof(vaultName));
            key.ArgNotNull(nameof(key));

            if (_cache.TryGetValue(vaultName, out var vault) &&
                vault.TryGetValue(key, out var secret))
            {
                return secret.ToTaskResult();
            }

            return ((KeySecret) null).ToTaskResult();
        }
    }
}
