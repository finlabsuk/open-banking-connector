// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.AccountAndTransaction;

internal class AccountAccessConsentConfig(
    bool supportsGlobalQueryFilter,
    DbProvider dbProvider,
    bool isRelationalDatabase,
    Formatting jsonFormatting)
    : BaseConsentConfig<AccountAccessConsent>(
        supportsGlobalQueryFilter,
        dbProvider,
        isRelationalDatabase,
        jsonFormatting)
{
    public override void Configure(EntityTypeBuilder<AccountAccessConsent> builder)
    {
        base.Configure(builder);

        // Use camel case for MongoDB
        if (_dbProvider is DbProvider.MongoDb)
        {
            builder.ToCollection("accountAccessConsent");
        }
    }
}
