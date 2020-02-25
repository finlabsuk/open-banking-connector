// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public class MemoryDomesticConsentRepository : IDomesticConsentRepository
    {
        private readonly ConcurrentDictionary<string, DomesticConsent> _cache =
            new ConcurrentDictionary<string, DomesticConsent>(StringComparer.InvariantCultureIgnoreCase);

        public Task<DomesticConsent> GetAsync(string id)
        {
            if (_cache.TryGetValue(id, out var value))
            {
                return value.ToTaskResult();
            }

            return ((DomesticConsent) null).ToTaskResult();
        }

        public Task<IQueryable<DomesticConsent>> GetAsync(Expression<Func<DomesticConsent, bool>> predicate)
        {
            var where = predicate.ArgNotNull(nameof(predicate)).Compile();

            var domesticConsents = _cache.Values.Where(where)
                .ToList() // To maintain non-volatile cache queries
                .AsQueryable();

            return domesticConsents.ToTaskResult();
        }

        public Task<DomesticConsent> SetAsync(DomesticConsent value)
        {
            value.ArgNotNull(nameof(value));
            value.Id.ArgNotNull(nameof(value.Id));

            return _cache.AddOrUpdate(value.Id, _ => value, (_, __) => value).ToTaskResult();
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
