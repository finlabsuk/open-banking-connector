// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class GetEntityByPredicate<TEntity, TPublicRequest, TPublicResponse, TPublicQuery>
        where TEntity : class, IEntityWithPublicInterface<TEntity, TPublicRequest, TPublicResponse, TPublicQuery>,
        TPublicQuery, new()
        where TPublicRequest : class // required by IEntityWithPublicInterface
        where TPublicResponse : class, TPublicQuery // required by IEntityWithPublicInterface
        where TPublicQuery : class // required by IEntityWithPublicInterface
    {
        private readonly IDbReadOnlyEntityRepository<TEntity> _entityRepo;

        public GetEntityByPredicate(IDbReadOnlyEntityRepository<TEntity> entityRepo)
        {
            _entityRepo = entityRepo;
        }

        public async Task<IQueryable<TPublicResponse>> GetAsync(Expression<Func<TPublicQuery, bool>> predicate)
        {
            // Convert expression tree type
            ParameterExpression entityInput = Expression.Parameter(type: typeof(TEntity), name: "entity");
            InvocationExpression main = Expression.Invoke(expression: predicate, entityInput);
            Expression<Func<TEntity, bool>> predicateWithUpdatedType =
                Expression.Lambda<Func<TEntity, bool>>(body: main, entityInput);

            // Run query
            IQueryable<TEntity> resultEntity = await _entityRepo.GetAsync(predicateWithUpdatedType);

            // Process and return results
            IQueryable<TPublicResponse> resultResponse = resultEntity.Select(b => b.PublicResponse);
            return resultResponse;
        }
    }
}
