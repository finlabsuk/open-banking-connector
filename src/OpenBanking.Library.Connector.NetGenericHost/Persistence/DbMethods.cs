// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace FinnovationLabs.OpenBanking.Library.Connector.NetGenericHost.Persistence
{
    public static class DbMethods
    {
        public static void CheckDbExists(this IServiceProvider serviceProvider)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            IServiceProvider services = scope.ServiceProvider;
            using BaseDbContext context = services.GetRequiredService<BaseDbContext>();

            // Delete/Create DB as required (should normally be commented out)
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Check DB exists
            IRelationalDatabaseCreator creator = context.Database.GetService<IRelationalDatabaseCreator>();
            if (!creator.Exists())
            {
                throw new ApplicationException(
                    "No database found. Run 'dotnet ef database update' in OpenBanking.WebApp.Connector.Sample root folder to create test DB.");
            }
        }
    }
}
