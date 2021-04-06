// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Repositories
{
    /// <summary>
    ///     Interface of read-only repository used to store <see cref="IRepositoryItem" /> items.
    ///     The repository may be something like an in-memory cache.
    /// </summary>
    /// <typeparam name="TRepositoryItem"></typeparam>
    public interface IReadOnlyRepository<TRepositoryItem>
        where TRepositoryItem : class, IRepositoryItem
    {
        Task<TRepositoryItem?> GetAsync(string id);

        Task<IQueryable<TRepositoryItem>> GetAsync(Expression<Func<TRepositoryItem, bool>> predicate);

        Task<IList<string>> GetIdsAsync();
    }
}
