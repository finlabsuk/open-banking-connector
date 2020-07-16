// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    /// <summary>
    ///     Entity- (type-) specific DB methods
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DbEntityRepository<TEntity> : IDbEntityRepository<TEntity>, IDisposable where TEntity : class, IEntity
    {
        private readonly BaseDbContext _db;
        private readonly DbSet<TEntity> _dbSet;

        public DbEntityRepository(BaseDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<TEntity>();
        }

        public ValueTask<TEntity> GetAsync(string id)
        {
            id.ArgNotNull(nameof(id));

            return _dbSet
                .FindAsync(id);
        }

        public async Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Expression<Func<TEntity, bool>> where = predicate.ArgNotNull(nameof(predicate));

            List<TEntity> results = await _dbSet
                .Where(where)
                .ToListAsync(); // To maintain non-volatile cache queries

            return results.AsQueryable();
        }

        public async Task<TEntity> UpsertAsync(TEntity instance)
        {
            instance.ArgNotNull(nameof(instance));

            // Input should be detached (untracked)
            if (_db.Entry(instance).State != EntityState.Detached)
            {
                throw new InvalidOperationException("Entity is tracked, no need to use set (upsert).");
            }

            TEntity existingValue = await _dbSet
                .FindAsync(instance.Id);
            if (existingValue is null)
            {
                await _db.AddAsync(instance);
                return instance;
            }

            _db.Entry(existingValue).CurrentValues.SetValues(instance);
            return existingValue;
        }

        public Task RemoveAsync(TEntity instance)
        {
            instance.ArgNotNull(nameof(instance));

            if (
                _db.Entry(instance).State == EntityState.Added ||
                _db.Entry(instance).State == EntityState.Modified
            )
            {
                throw new InvalidOperationException("Entity is in invalid tracking state for this operation.");
            }

            _dbSet
                .Remove(instance);
            return ((TEntity) null).ToTaskResult();
        }

        public async Task<IQueryable<TEntity>> GetAllAsync()
        {
            List<TEntity> instances = await _dbSet
                .ToListAsync();

            return instances.AsQueryable();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task AddAsync(TEntity instance)
        {
            instance.ArgNotNull(nameof(instance));

            // Input should be detached (untracked)
            if (_db.Entry(instance).State != EntityState.Detached)
            {
                throw new InvalidOperationException("Instance is tracked, no need to use add.");
            }

            await _dbSet.AddAsync(instance);
        }
    }
}
