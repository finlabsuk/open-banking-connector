// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class GetEntityById<TEntity, TPublicRequest, TPublicResponse, TPublicQuery>
        where TEntity : class, IEntityWithPublicInterface<TEntity, TPublicRequest, TPublicResponse, TPublicQuery>,
        TPublicQuery, new()
        where TPublicRequest : class // required by IEntityWithPublicInterface
        where TPublicResponse : class, TPublicQuery // required by IEntityWithPublicInterface
        where TPublicQuery : class // required by IEntityWithPublicInterface
    {
        private readonly IDbReadOnlyEntityRepository<TEntity> _entityRepo;
        private readonly ITimeProvider _timeProvider;

        public GetEntityById(ITimeProvider timeProvider, IDbReadOnlyEntityRepository<TEntity> entityRepo)
        {
            _timeProvider = timeProvider;
            _entityRepo = entityRepo;
        }

        public async Task<TPublicResponse> GetAsync(
            Guid id,
            bool includeBankApiGet,
            string? modifiedBy)
        {
            // Fetch entity
            TEntity entity = await _entityRepo.GetAsync(id) ??
                             throw new KeyNotFoundException($"No record found for ID {id}.");

            // Bank API get and determine response
            TPublicResponse response;
            if (includeBankApiGet)
            {
                response = await entity.BankApiGetAsync(timeProvider: _timeProvider, modifiedBy: modifiedBy);
            }
            else
            {
                response = entity.PublicResponse;
            }

            return response;
        }
    }
}
