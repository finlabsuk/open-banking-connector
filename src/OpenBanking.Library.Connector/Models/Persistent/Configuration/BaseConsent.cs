// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration;

internal class BaseConsentConfig<TEntity> : BaseConfig<TEntity>
    where TEntity : BaseConsent
{
    public BaseConsentConfig(DbProvider dbProvider, bool supportsGlobalQueryFilter, Formatting jsonFormatting) :
        base(dbProvider, supportsGlobalQueryFilter, jsonFormatting) { }

    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.Id)
            .HasColumnOrder(0);
        builder.Property(e => e.BankRegistrationId)
            .HasColumnOrder(1)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

        builder.Property(e => e.ExternalApiId)
            .HasColumnOrder(100)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.AuthContextState)
            .HasColumnOrder(101);
        builder.Property(e => e.AuthContextNonce)
            .HasColumnOrder(102);
        builder.Property(e => e.AuthContextModified)
            .HasColumnOrder(103);
        builder.Property(e => e.AuthContextModifiedBy)
            .HasColumnOrder(104);
        builder.Property("_accessTokenAccessToken")
            .HasColumnOrder(105);
        builder.Property("_accessTokenExpiresIn")
            .HasColumnOrder(106);
        builder.Property("_accessTokenRefreshToken")
            .HasColumnOrder(107);
        builder.Property("_accessTokenModified")
            .HasColumnOrder(108);
        builder.Property("_accessTokenModifiedBy")
            .HasColumnOrder(109);
        builder.Property(e => e.ExternalApiUserId);
        builder.Property(e => e.ExternalApiUserIdModified);
        builder.Property(e => e.ExternalApiUserIdModifiedBy);

        // Note: we specify column order above and in parent classes to solve two problems:
        // (1) Auto-ordering with two base classes seems to put columns from "middle" class at end of table.
        // (2) Field-sourced columns jump to start of table with auto-ordering and their ordering w.r.t. one
        // another seems fixed as alphabetical.
        // We group columns above into two blocks:
        // 0-99: for keys (primary and foreign)
        // 100+: for "middle" class columns including those sourced from fields. 
    }
}
