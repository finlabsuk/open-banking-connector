// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    public interface IDbReadOnlyEntityRepository<TEntity> where TEntity : class, IEntity
    {
        ValueTask<TEntity?> GetAsync(Guid id, bool detachFirst = false);
        Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);
    }
}
