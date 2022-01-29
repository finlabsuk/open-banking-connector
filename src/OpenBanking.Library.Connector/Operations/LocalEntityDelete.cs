// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class LocalEntityDelete<TEntity> :
        IObjectDelete
        where TEntity : class, ISupportsFluentDeleteLocal<TEntity>
    {
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        protected readonly IDbReadWriteEntityMethods<TEntity> _entityMethods;
        protected readonly IInstrumentationClient _instrumentationClient;
        protected readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public LocalEntityDelete(
            IDbReadWriteEntityMethods<TEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient)
        {
            _entityMethods = entityMethods;
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _timeProvider = timeProvider;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _instrumentationClient = instrumentationClient;
        }

        public async
            Task<IList<IFluentResponseInfoOrWarningMessage>> DeleteAsync(
                Guid id,
                string? modifiedBy,
                bool useRegistrationAccessToken)
        {
            var requestInfo = new DeleteRequestInfo(id, modifiedBy, useRegistrationAccessToken);

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // DELETE at bank API
            (TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await ApiDelete(requestInfo);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Local soft delete
            persistedObject.IsDeleted = new ReadWriteProperty<bool>(
                true,
                _timeProvider,
                requestInfo.ModifiedBy);

            await _dbSaveChangesMethod.SaveChangesAsync();

            // Return success response (thrown exceptions produce error response)
            return nonErrorMessages;
        }

        protected virtual async
            Task<(TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiDelete(DeleteRequestInfo requestInfo)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Get persisted entity
            TEntity persistedObject =
                await _entityMethods
                    .DbSet
                    .SingleOrDefaultAsync(x => x.Id == requestInfo.Id) ??
                throw new KeyNotFoundException($"No record found for entity with ID {requestInfo.Id}.");

            return (persistedObject, nonErrorMessages);
        }

        public class DeleteRequestInfo
        {
            public DeleteRequestInfo(Guid id, string? modifiedBy, bool useRegistrationAccessToken)
            {
                Id = id;
                ModifiedBy = modifiedBy;
                UseRegistrationAccessToken = useRegistrationAccessToken;
            }

            public Guid Id { get; }

            public string? ModifiedBy { get; }

            public bool UseRegistrationAccessToken { get; }
        }
    }
}
