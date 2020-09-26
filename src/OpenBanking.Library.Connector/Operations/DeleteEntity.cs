// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class DeleteEntity<TEntity, TPublicRequest, TPublicResponse, TPublicQuery>
        where TEntity : class, IEntityWithPublicInterface<TEntity, TPublicRequest, TPublicResponse, TPublicQuery>,
        TPublicQuery, new()
        where TPublicRequest : class // required by IEntityWithPublicInterface
        where TPublicResponse : class, TPublicQuery // required by IEntityWithPublicInterface
        where TPublicQuery : class // required by IEntityWithPublicInterface
    {
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly IDbEntityRepository<TEntity> _entityRepo;
        private readonly ITimeProvider _timeProvider;

        public DeleteEntity(
            IDbEntityRepository<TEntity> entityRepo,
            IDbMultiEntityMethods dbMultiEntityMethods,
            ITimeProvider timeProvider)
        {
            _entityRepo = entityRepo;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _timeProvider = timeProvider;
        }

        public async Task DeleteAsync(string id, bool includeBankApiDelete, string? modifiedBy, bool hardNotSoftDelete)
        {
            // Fetch entity
            TEntity entity = await _entityRepo.GetAsync(id) ??
                             throw new KeyNotFoundException($"No record found for ID {id}.");

            // Bank API delete
            if (includeBankApiDelete)
            {
                await entity.BankApiDeleteAsync();
            }

            // Local delete
            if (hardNotSoftDelete)
            {
                await _entityRepo.RemoveAsync(entity);
            }
            else
            {
                entity.IsDeleted = new ReadWriteProperty<bool>(
                    data: true,
                    timeProvider: _timeProvider,
                    modifiedBy: modifiedBy);
            }

            await _dbMultiEntityMethods.SaveChangesAsync();
        }
    }
}
