// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    internal abstract class ObjectContextBase<TEntity> :
        IDeleteLocalContextInternal
        where TEntity : class,
        IEntity
    {
        protected ObjectContextBase(ISharedContext sharedContext)
        {
            Context = sharedContext;
            DeleteLocalObject = new LocalEntityDelete<TEntity, LocalDeleteParams>(
                sharedContext.DbService.GetDbEntityMethodsClass<TEntity>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation);
        }

        public ISharedContext Context { get; }
        public IObjectDelete<LocalDeleteParams> DeleteLocalObject { get; }
    }
}
