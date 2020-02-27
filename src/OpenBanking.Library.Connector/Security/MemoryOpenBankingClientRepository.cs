﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public class MemoryOpenBankingClientRepository : IOpenBankingClientRepository
    {
        private readonly ConcurrentDictionary<string, BankClient> _cache =
            new ConcurrentDictionary<string, BankClient>(StringComparer.InvariantCultureIgnoreCase);

        public Task<BankClient> GetAsync(string id)
        {
            if (_cache.TryGetValue(id, out var value))
            {
                return value.ToTaskResult();
            }

            return ((BankClient) null).ToTaskResult();
        }

        public Task<IQueryable<BankClient>> GetAsync(Expression<Func<BankClient, bool>> predicate)
        {
            var where = predicate.ArgNotNull(nameof(predicate)).Compile();

            var results = _cache.Values.Where(where)
                .ToList() // To maintain non-volatile cache queries
                .AsQueryable();

            return results.ToTaskResult();
        }

        public async Task<BankClient> SetAsync(BankClient value)
        {
            value.ArgNotNull(nameof(value));

            if (value.Id == null)
            {
                var existingDto = (await GetAsync(c => c.IssuerUrl == value.IssuerUrl)).SingleOrDefault();
                if (existingDto != null)
                {
                    value.Id = existingDto.Id;
                }
                else
                {
                    value.Id = Guid.NewGuid().ToString();
                }
            }

            return _cache.AddOrUpdate(value.Id, _ => value, (_, __) => value);
        }

        public Task<bool> DeleteAsync(string id)
        {
            return _cache.TryRemove(id, out _).ToTaskResult();
        }

        public Task<IList<string>> GetIdsAsync()
        {
            IList<string> keys = _cache.Keys.ToList();

            return keys.ToTaskResult();
        }
    }
}
