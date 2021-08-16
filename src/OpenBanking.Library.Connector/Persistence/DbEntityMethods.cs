// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    /// <summary>
    ///     Entity- (type-) specific DB methods
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DbEntityMethods<TEntity> : IDbEntityMethods<TEntity>
        where TEntity : class, IEntity
    {
        private readonly BaseDbContext _db;

        public DbEntityMethods(BaseDbContext db)
        {
            _db = db;
            DbSet = _db.Set<TEntity>();
        }

        public DbSet<TEntity> DbSet { get; }

        public IQueryable<TEntity> DbSetNoTracking => DbSet.AsNoTracking();
        public ValueTask<TEntity?> GetAsync(Guid id) => GetInnerAsync(id);

        public Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
            => GetInnerAsync(predicate);

        public ValueTask<TEntity?> GetNoTrackingAsync(Guid id) =>
            GetInnerAsync(id, true);

        public Task<IQueryable<TEntity>> GetNoTrackingAsync(Expression<Func<TEntity, bool>> predicate)
            => GetInnerAsync(predicate, true);

        public async Task AddAsync(TEntity entity)
        {
            entity.ArgNotNull(nameof(entity));

            // Check entity state
            if (_db.Entry(entity).State != EntityState.Detached)
            {
                throw new InvalidOperationException($"Entity is not in state {EntityState.Detached} so will not add");
            }

            // Check whether deleted
            if (entity.IsDeleted.Data)
            {
                throw new InvalidOperationException("Entity marked as deleted so will not add");
            }


            await DbSet.AddAsync(entity);
        }

        private async ValueTask<TEntity?> GetInnerAsync(Guid id, bool asNoTracking = false)
        {
            TEntity? result;

            if (asNoTracking)
            {
                result = await DbSet
                    .AsNoTracking()
                    .SingleOrDefaultAsync(s => s.Id == id);
            }
            else
            {
                result = await DbSet
                    .FindAsync(id);
            }

            return result;
        }

        private async Task<IQueryable<TEntity>> GetInnerAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = false)
        {
            Expression<Func<TEntity, bool>> where = predicate.ArgNotNull(nameof(predicate));

            IQueryable<TEntity> baseQuery = asNoTracking ? DbSet.AsNoTracking() : DbSet;

            List<TEntity> results = await baseQuery
                .Where(where)
                .ToListAsync(); // To maintain non-volatile cache queries

            return results.AsQueryable();
        }
    }
}
