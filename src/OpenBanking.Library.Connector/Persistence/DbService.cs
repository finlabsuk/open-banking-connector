// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    /// <summary>
    ///     Base DB service which depends on DB Context and provides method classes
    ///     allowing access to DB entities in a controlled manner.
    /// </summary>
    public class DbService : IDbService
    {
        private readonly BaseDbContext _db;

        public DbService(BaseDbContext db)
        {
            _db = db;
        }

        public IDbEntityMethods<TEntity> GetDbEntityMethodsClass<TEntity>() where TEntity : class, IEntity =>
            new DbEntityMethods<TEntity>(_db);

        public IDbSaveChangesMethod GetDbSaveChangesMethodClass() => new DbSaveChangesMethod(_db);
    }
}
