// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Access
{
    public class ReadOnlyKeySecretItemCache<TItem> : IReadOnlyKeySecretItemRepository<TItem>
        where TItem : class, IKeySecretItem
    {
        private readonly IDictionary<string, TItem> _cache;

        public ReadOnlyKeySecretItemCache(IDictionary<string, TItem> cache)
        {
            _cache = cache;
        }

        public Task<TItem> GetAsync(string id)
        {
            if (_cache.TryGetValue(key: id, value: out TItem value))
            {
                return Task.FromResult(value);
            }

            throw new ArgumentOutOfRangeException($"No entry matching ID {id}");
        }
    }
}
