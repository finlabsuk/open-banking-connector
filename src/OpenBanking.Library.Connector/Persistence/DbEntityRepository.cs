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
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    public interface IDbEntityRepositoryFactory
    {
        IDbEntityRepository<TEntity> CreateDbEntityRepository<TEntity>() where TEntity : class, IEntity;
    }

    public class DbEntityRepositoryFactory : IDbEntityRepositoryFactory
    {
        private readonly BaseDbContext _db;

        public DbEntityRepositoryFactory(BaseDbContext db)
        {
            _db = db;
        }

        public IDbEntityRepository<TEntity> CreateDbEntityRepository<TEntity>() where TEntity : class, IEntity
        {
            return new DbEntityRepository<TEntity>(_db);
        }
    }

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

        public async ValueTask<TEntity?> GetAsync(Guid id, bool detachFirst = false)
        {
            if (detachFirst)
            {
                IEnumerable<EntityEntry<TEntity>> changeTrackerEntity = _db.ChangeTracker
                    .Entries<TEntity>()
                    .Where(x => x.Entity.Id == id);

                EntityEntry<TEntity>? value = changeTrackerEntity.SingleOrDefault();
                if (!(value is null))
                {
                    value.State = EntityState.Detached;
                }
            }

            TEntity? result = await _dbSet
                .FindAsync(id);

            return result;
        }

        public async Task<IQueryable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = false)
        {
            Expression<Func<TEntity, bool>> where = predicate.ArgNotNull(nameof(predicate));

            IQueryable<TEntity> baseQuery = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;

            List<TEntity> results = await baseQuery
                .Where(where)
                .ToListAsync(); // To maintain non-volatile cache queries

            return results.AsQueryable();
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
            return ((TEntity?) null).ToTaskResult();
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

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
