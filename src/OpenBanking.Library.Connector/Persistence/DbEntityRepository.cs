// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    public class DbEntityRepository<TEntity> : IDbEntityRepository<TEntity> where TEntity: class, IEntity
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

            return _db.Set<TEntity>()
                .FindAsync(id);
        }

        public async Task<IQueryable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> predicate)
        {
            var where = predicate.ArgNotNull(nameof(predicate));

            var results = await _db.Set<TEntity>()
                    .Where(where)
                    .ToListAsync(); // To maintain non-volatile cache queries
                
            return results.AsQueryable();
        }

        // NB: This is an UPSERT method.
        public async Task<TEntity> SetAsync(TEntity instance)
        {
            instance.ArgNotNull(nameof(instance));
            
            // Input should be detached (untracked)
            if (_db.Entry(instance).State != EntityState.Detached)
            {
                throw new InvalidOperationException("Entity is tracked, no need to use set (upsert).");
            }

            var existingValue = await _db.Set<TEntity>()
                .FindAsync(instance.Id);
            if (existingValue is null)
            {
                await _db.AddAsync(instance);
                return instance;
            }
            else
            {
                _db.Entry(existingValue).CurrentValues.SetValues(instance);
                return existingValue;
            }
        }
        
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public Task DeleteAsync(TEntity instance)
        {
            instance.ArgNotNull(nameof(instance));

            if (
                _db.Entry(instance).State == EntityState.Added ||
                _db.Entry(instance).State == EntityState.Modified
            )
            {
                throw new InvalidOperationException("Entity is in invalid tracking state for this operation.");
            }

            _db.Set<TEntity>()
                .Remove(instance);
            return ((TEntity) null).ToTaskResult();
        }

        public Task<List<string>> GetIdsAsync()
        {
            var keys = _db.Set<TEntity>()
                .Select(p => p.Id)
                .ToListAsync();
            
            return keys;
        }
    }
}
