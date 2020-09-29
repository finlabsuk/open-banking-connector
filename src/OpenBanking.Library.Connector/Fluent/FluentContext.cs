// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    internal class
        FluentContext<TEntity, TPublicRequest, TPublicResponse, TPublicQuery>
        : FluentContextPostOnlyEntity<TEntity, TPublicRequest, TPublicResponse>,
            IFluentContextLocalEntity<TPublicRequest, TPublicResponse, TPublicQuery>
        where TEntity : class, IEntityWithPublicInterface<TEntity, TPublicRequest, TPublicResponse, TPublicQuery>,
        TPublicQuery, new()
        where TPublicRequest : class // required by IEntityWithPublicInterface
        where TPublicResponse : class, TPublicQuery // required by IEntityWithPublicInterface
        where TPublicQuery : class // required by IEntityWithPublicInterface
    {
        internal FluentContext(ISharedContext context) : base(context) { }

        public async Task<FluentResponse<IQueryable<TPublicResponse>>> GetAsync(
            Expression<Func<TPublicQuery, bool>> predicate)
        {
            try
            {
                GetEntityByPredicate<TEntity, TPublicRequest, TPublicResponse, TPublicQuery> i =
                    new GetEntityByPredicate<TEntity, TPublicRequest, TPublicResponse, TPublicQuery>(
                        _context.DbEntityRepositoryFactory.CreateDbEntityRepository<TEntity>());

                IQueryable<TPublicResponse> resp = await i.GetAsync(predicate);
                return new FluentResponse<IQueryable<TPublicResponse>>(resp);
            }
            catch (AggregateException ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentResponse<IQueryable<TPublicResponse>>(messages: ex.CreateErrorMessages(), data: null);
            }
            catch (Exception ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentResponse<IQueryable<TPublicResponse>>(message: ex.CreateErrorMessage(), data: null);
            }
        }

        // Local Entity simplified method.
        public async Task<FluentResponse<TPublicResponse>> GetAsync(string id) => await GetAsync(
            id: id,
            includeBankApiGet: false,
            modifiedBy: null);

        // Local Entity simplified method.
        public async Task<FluentResponse<EmptyClass>> DeleteAsync(
            string id,
            string? modifiedBy,
            bool hardNotSoftDelete) => await DeleteAsync(
            id: id,
            includeBankApiDelete: false,
            modifiedBy: modifiedBy,
            hardNotSoftDelete: hardNotSoftDelete);

        public async Task<FluentResponse<TPublicResponse>> GetAsync(
            string id,
            bool includeBankApiGet,
            string? modifiedBy)
        {
            try
            {
                GetEntityById<TEntity, TPublicRequest, TPublicResponse, TPublicQuery> i =
                    new GetEntityById<TEntity, TPublicRequest, TPublicResponse, TPublicQuery>(
                        timeProvider: _context.TimeProvider,
                        entityRepo: _context.DbEntityRepositoryFactory.CreateDbEntityRepository<TEntity>());

                TPublicResponse resp = await i.GetAsync(
                    id: id,
                    includeBankApiGet: includeBankApiGet,
                    modifiedBy: modifiedBy);
                return new FluentResponse<TPublicResponse>(resp);
            }
            catch (AggregateException ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentResponse<TPublicResponse>(messages: ex.CreateErrorMessages(), data: null);
            }
            catch (Exception ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentResponse<TPublicResponse>(message: ex.CreateErrorMessage(), data: null);
            }
        }

        public async Task<FluentResponse<EmptyClass>> DeleteAsync(
            string id,
            bool includeBankApiDelete,
            string? modifiedBy,
            bool hardNotSoftDelete)
        {
            try
            {
                DeleteEntity<TEntity, TPublicRequest, TPublicResponse, TPublicQuery> i =
                    new DeleteEntity<TEntity, TPublicRequest, TPublicResponse, TPublicQuery>(
                        entityRepo: _context.DbEntityRepositoryFactory.CreateDbEntityRepository<TEntity>(),
                        dbMultiEntityMethods: _context.DbContextService,
                        timeProvider: _context.TimeProvider);

                await i.DeleteAsync(
                    id: id,
                    includeBankApiDelete: includeBankApiDelete,
                    modifiedBy: modifiedBy,
                    hardNotSoftDelete: hardNotSoftDelete);
                return new FluentResponse<EmptyClass>(new EmptyClass());
            }
            catch (AggregateException ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentResponse<EmptyClass>(messages: ex.CreateErrorMessages(), data: null);
            }
            catch (Exception ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentResponse<EmptyClass>(message: ex.CreateErrorMessage(), data: null);
            }
        }
    }
}
