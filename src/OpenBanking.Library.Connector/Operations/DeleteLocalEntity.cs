// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class DeleteLocalEntity<TEntity>
        where TEntity : class, ISupportsFluentDeleteLocal<TEntity>
    {
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly IDbReadWriteEntityMethods<TEntity> _entityRepo;
        private readonly ITimeProvider _timeProvider;

        public DeleteLocalEntity(
            IDbReadWriteEntityMethods<TEntity> entityRepo,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider)
        {
            _entityRepo = entityRepo;
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _timeProvider = timeProvider;
        }

        public async
            Task<IList<IFluentResponseInfoOrWarningMessage>>
            DeleteLocalAsync(
                Guid id,
                string? modifiedBy)
        {
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Fetch entity
            TEntity entity = await _entityRepo.GetAsync(id) ??
                             throw new KeyNotFoundException($"No record found for ID {id}.");

            // Local soft delete
            entity.IsDeleted = new ReadWriteProperty<bool>(
                true,
                _timeProvider,
                modifiedBy);

            await _dbSaveChangesMethod.SaveChangesAsync();

            // Return success response (thrown exceptions produce error response)
            return nonErrorMessages;
        }
    }
}
