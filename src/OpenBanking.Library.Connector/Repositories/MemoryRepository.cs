// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Linq.Expressions;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Repositories
{
    /// <summary>
    ///     Repository implemented as in-memory cache.
    /// </summary>
    /// <typeparam name="TRepositoryItem"></typeparam>
    public class MemoryRepository<TRepositoryItem> : IRepository<TRepositoryItem>
        where TRepositoryItem : class, IRepositoryItem

    {
        protected readonly ConcurrentDictionary<string, TRepositoryItem> _cache =
            new(StringComparer.InvariantCultureIgnoreCase);

        public Task<TRepositoryItem?> GetAsync(string id)
        {
            id.ArgNotNull(nameof(id));

            if (_cache.TryGetValue(id, out TRepositoryItem? value))
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
