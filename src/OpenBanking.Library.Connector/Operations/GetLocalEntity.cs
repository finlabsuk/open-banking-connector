// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class GetLocalEntity<TEntity, TPublicQuery, TPublicLocalResponse>
        where TEntity : class, ISupportsFluentGetLocal<TEntity, TPublicQuery, TPublicLocalResponse>
    {
        private readonly IDbReadOnlyEntityMethods<TEntity> _entityRepo;

        public GetLocalEntity(IDbReadOnlyEntityMethods<TEntity> entityRepo)
        {
            _entityRepo = entityRepo;
        }

        public async Task<(TPublicLocalResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            GetLocalAsync(Guid id)
        {
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Fetch entity
            TEntity entity = await _entityRepo.GetNoTrackingAsync(id) ??
                             throw new KeyNotFoundException($"No record found for ID {id}.");

            // Return success response (thrown exceptions produce error response)
            return (entity.PublicGetLocalResponse, nonErrorMessages);
        }

        public async
            Task<(IQueryable<TPublicLocalResponse> response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages
                )>
            GetLocalAsync(Expression<Func<TPublicQuery, bool>> predicate)
        {
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Convert expression tree type
            ParameterExpression entityInput = Expression.Parameter(typeof(TEntity), "entity");
            InvocationExpression main = Expression.Invoke(predicate, entityInput);
            Expression<Func<TEntity, bool>> predicateWithUpdatedType =
                Expression.Lambda<Func<TEntity, bool>>(main, entityInput);

            // Run query
            IQueryable<TEntity> resultEntity = await _entityRepo.GetNoTrackingAsync(predicateWithUpdatedType);

            // Process results
            IQueryable<TPublicLocalResponse> resultResponse = resultEntity.Select(b => b.PublicGetLocalResponse);

            // Return success response (thrown exceptions produce error response)
            return (resultResponse, nonErrorMessages);
        }
    }
}
