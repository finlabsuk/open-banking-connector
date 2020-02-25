// Licensed to Finnovation Labs Limited under one or more agreements.
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
    public class MemoryOpenBankingClientProfileRepository : IOpenBankingClientProfileRepository
    {
        private readonly ConcurrentDictionary<string, OpenBankingClientProfile> _cache =
            new ConcurrentDictionary<string, OpenBankingClientProfile>(StringComparer.InvariantCultureIgnoreCase);

        public Task<OpenBankingClientProfile> GetAsync(string id)
        {
            if (_cache.TryGetValue(id, out var value))
            {
                return value.ToTaskResult();
            }

            return ((OpenBankingClientProfile) null).ToTaskResult();
        }

        public Task<IQueryable<OpenBankingClientProfile>> GetAsync(
            Expression<Func<OpenBankingClientProfile, bool>> predicate)
        {
            var where = predicate.ArgNotNull(nameof(predicate)).Compile();

            var results = _cache.Values.Where(where)
                .ToList() // To maintain non-volatile cache queries
                .AsQueryable();

            return results.ToTaskResult();
        }

        public Task<OpenBankingClientProfile> SetAsync(OpenBankingClientProfile profile)
        {
            profile.ArgNotNull(nameof(profile));
            profile.Id.ArgNotNull(nameof(profile.Id));

            return _cache.AddOrUpdate(profile.Id, _ => profile, (_, __) => profile).ToTaskResult();
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
