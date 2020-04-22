// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    public interface IDbEntityRepository<TEntity> where TEntity : class, IEntity
    {
        ValueTask<TEntity> GetAsync(string id);

        Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Emulates UPSERT until such time as EF Core supports UPSERT. Return value
        /// is that tracked by EF Core change tracker (input parameter in
        /// async method cannot be ref).
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Task<TEntity> UpsertAsync(TEntity instance);

        Task RemoveAsync(TEntity instance);

        Task<IQueryable<TEntity>> GetAllAsync();
    }
}
