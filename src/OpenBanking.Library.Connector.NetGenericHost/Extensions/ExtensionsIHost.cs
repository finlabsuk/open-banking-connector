// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinnovationLabs.OpenBanking.Library.Connector.NetGenericHost.Extensions
{
    public static class ExtensionsIHost
    {
        public static IHost StartupTasks(
            this IHost host,
            bool deleteAndRecreateDb)
        {
            using IServiceScope scope = host.Services.CreateScope();
            using BaseDbContext context = scope.ServiceProvider.GetRequiredService<BaseDbContext>();

            // Delete/Create DB as required
            if (deleteAndRecreateDb)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            else
            {
                // Check DB exists
                IRelationalDatabaseCreator creator = context.Database.GetService<IRelationalDatabaseCreator>();
                if (!creator.Exists())
                {
                    throw new ApplicationException(
                        "No database found. Run 'dotnet ef database update' in OpenBanking.WebApp.Connector.Sample root folder to create test DB.");
                }
            }

            return host;
        }
    }
}
