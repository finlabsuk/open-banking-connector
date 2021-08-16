// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    internal abstract class ObjectContextBase<TEntity> :
        IDeleteLocalContext
        where TEntity : class,
        ISupportsFluentDeleteLocal<TEntity>, new()
    {
        private readonly LocalEntityDelete<TEntity> _localEntityDelete;

        protected ObjectContextBase(ISharedContext sharedContext)
        {
            _localEntityDelete = new LocalEntityDelete<TEntity>(sharedContext);
        }

        public Task<IFluentResponse> DeleteLocalAsync(Guid id, string? modifiedBy) =>
            _localEntityDelete.DeleteAsync(id, modifiedBy, false);
    }
}
