// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Repositories
{
    /// <summary>
    ///     Base class for repository implemented as in-memory cache. Can be sub-classed with custom constructor.
    /// </summary>
    /// <typeparam name="TRepositoryItem"></typeparam>
    public abstract class MemoryRepository<TRepositoryItem> : IRepository<TRepositoryItem>
        where TRepositoryItem : class, IRepositoryItem

    {
        protected readonly ConcurrentDictionary<string, TRepositoryItem> _cache =
            new ConcurrentDictionary<string, TRepositoryItem>(StringComparer.InvariantCultureIgnoreCase);

        public Task<TRepositoryItem?> GetAsync(string id)
        {
            id.ArgNotNull(nameof(id));

            if (_cache.TryGetValue(id, out TRepositoryItem value))
            {
                return ((TRepositoryItem?) value).ToTaskResult();
            }

            return ((TRepositoryItem?) null).ToTaskResult();
        }

        public Task<IQueryable<TRepositoryItem>> GetAsync(Expression<Func<TRepositoryItem, bool>> predicate)
        {
            Func<TRepositoryItem, bool> where = predicate.ArgNotNull(nameof(predicate)).Compile();

            IQueryable<TRepositoryItem> results = _cache.Values.Where(where)
                .ToList() // To maintain non-volatile cache queries
                .AsQueryable();

            return results.ToTaskResult();
        }

        public Task<TRepositoryItem> SetAsync(TRepositoryItem value)
        {
            value.ArgNotNull(nameof(value));
            value.Id.ArgNotNull(nameof(value.Id));


            TRepositoryItem result = _cache.AddOrUpdate(value.Id, _ => value, (_, __) => value);

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
