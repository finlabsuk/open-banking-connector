﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence;

public enum DbProvider
{
    Sqlite,
    PostgreSql,
    MongoDb
}

public static class DbProviderHelper
{
    static DbProviderHelper()
    {
        AllDbProviders = Enum.GetValues(typeof(DbProvider))
            .Cast<DbProvider>();
    }

    public static IEnumerable<DbProvider> AllDbProviders { get; }
}
