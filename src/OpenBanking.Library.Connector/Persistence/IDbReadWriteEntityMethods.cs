﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    /// <summary>
    ///     Methods allowing a DB Entity record to be changed. To delete a record, please use <see cref="IEntity.IsDeleted" />
    ///     (soft-delete).
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDbReadWriteEntityMethods<TEntity> where TEntity : class, IEntity
    {
        ValueTask<TEntity?> GetAsync(Guid id);
        Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
    }
}
