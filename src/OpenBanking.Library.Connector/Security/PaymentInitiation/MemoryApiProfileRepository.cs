// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security.PaymentInitiation
{
    public class MemoryApiProfileRepository : IApiProfileRepository
    {
        private readonly ConcurrentDictionary<string, ApiProfile> _cache =
            new ConcurrentDictionary<string, ApiProfile>(StringComparer.InvariantCultureIgnoreCase);

        public Task<ApiProfile> GetAsync(string id)
        {
            if (_cache.TryGetValue(id, out var value))
            {
                return value.ToTaskResult();
            }

            return ((ApiProfile) null).ToTaskResult();
        }

        public Task<IQueryable<ApiProfile>> GetAsync(Expression<Func<ApiProfile, bool>> predicate)
        {
            var where = predicate.ArgNotNull(nameof(predicate)).Compile();

            var results = _cache.Values.Where(where)
                .ToList() // To maintain non-volatile cache queries
                .AsQueryable();

            return results.ToTaskResult();
        }

        public async Task<ApiProfile> SetAsync(ApiProfile value)
        {
            value.ArgNotNull(nameof(value));

            var existingDto = (await GetAsync(c => c.Id == value.Id)).SingleOrDefault();
            if (existingDto != null)
            {
                throw new InvalidOperationException("Object already exists");
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
