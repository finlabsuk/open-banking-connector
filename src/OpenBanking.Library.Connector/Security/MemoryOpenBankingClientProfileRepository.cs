// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public class MemoryOpenBankingClientProfileRepository : IOpenBankingClientProfileRepository
    {
        private readonly ConcurrentDictionary<string, BankClientProfile> _cache =
            new ConcurrentDictionary<string, BankClientProfile>(StringComparer.InvariantCultureIgnoreCase);

        public Task<BankClientProfile> GetAsync(string id)
        {
            if (_cache.TryGetValue(id, out var value))
            {
                return value.ToTaskResult();
            }

            return ((BankClientProfile) null).ToTaskResult();
        }

        public Task<IQueryable<BankClientProfile>> GetAsync(
            Expression<Func<BankClientProfile, bool>> predicate)
        {
            var where = predicate.ArgNotNull(nameof(predicate)).Compile();

            var results = _cache.Values.Where(where)
                .ToList() // To maintain non-volatile cache queries
                .AsQueryable();

            return results.ToTaskResult();
        }

        public async Task<BankClientProfile> SetAsync(BankClientProfile value)
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
            return _cache.TryRemove(id, out var x).ToTaskResult();
        }

        public Task<IList<string>> GetIdsAsync()
        {
            IList<string> keys = _cache.Keys.ToList();

            return keys.ToTaskResult();
        }
    }
}
