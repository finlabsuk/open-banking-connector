// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public class DbEntityRepository<TEntity> : IDbEntityRepository<TEntity> where TEntity: class, IEntity
    {
        private readonly BaseDbContext _db;

        public DbEntityRepository(BaseDbContext db)
        {
            _db = db;
        }

        public async Task<TEntity> GetAsync(string id)
        {
            var value = await _db.Set<TEntity>()
                .FindAsync(id);
            if (value is null)
            {
                throw new KeyNotFoundException("Cannot find value with specified ID.");
            }

            return value;
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
        public async Task<TEntity> SetAsync(TEntity value)
        {
            // Input should be detached (untracked)
            if (_db.Entry(value).State != EntityState.Detached)
            {
                throw new InvalidOperationException("Entity is tracked, no need to use set (upsert).");
            }

            var existingValue = await _db.Set<TEntity>()
                .FindAsync(value.Id);
            if (existingValue is null)
            {
                await _db.AddAsync(value);
                return value;
            }
            else
            {
                _db.Entry(existingValue).CurrentValues.SetValues(value);
                return existingValue;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetIdsAsync()
        {
            IList<string> keys = _db.Set<TEntity>()
                .Select(p => p.Id)
                .ToList();

            return keys.ToTaskResult();
        }
    }
}
