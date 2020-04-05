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
    public class MemorySoftwareStatementProfileRepository : ISoftwareStatementProfileRepository
    {
        private readonly ConcurrentDictionary<string, SoftwareStatementProfile> _cache =
            new ConcurrentDictionary<string, SoftwareStatementProfile>(StringComparer.InvariantCultureIgnoreCase);


        public Task<SoftwareStatementProfile> GetAsync(string id)
        {
            if (_cache.TryGetValue(id, out var value))
            {
                return value.ToTaskResult();
            }

            return ((SoftwareStatementProfile) null).ToTaskResult();
        }

        public Task<IQueryable<SoftwareStatementProfile>> GetAsync(
            Expression<Func<SoftwareStatementProfile, bool>> predicate)
        {
            var where = predicate.ArgNotNull(nameof(predicate)).Compile();

            var results = _cache.Values.Where(where)
                .ToList() // To maintain non-volatile cache queries
                .AsQueryable();

            return results.ToTaskResult();
        }


        public Task<SoftwareStatementProfile> SetAsync(SoftwareStatementProfile profile)
        {
            profile.ArgNotNull(nameof(profile));
            profile.Id.ArgNotNull(nameof(profile.Id));

            var result = _cache.AddOrUpdate(profile.Id, _ => profile, (_, __) => profile);

            return result.ToTaskResult();
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
