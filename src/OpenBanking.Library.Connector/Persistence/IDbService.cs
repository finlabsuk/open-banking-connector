// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    public interface IDbService
    {
        IDbEntityMethods<TEntity> GetDbEntityMethodsClass<TEntity>()
            where TEntity : class, IEntity;

        IDbSaveChangesMethod GetDbSaveChangesMethodClass();
    }
}
