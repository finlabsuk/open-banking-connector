// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration;

internal class EncryptedObjectConfig<TEntity> : BaseConfig<TEntity>
    where TEntity : EncryptedObject
{
    public EncryptedObjectConfig(DbProvider dbProvider, bool supportsGlobalQueryFilter, Formatting jsonFormatting) :
        base(dbProvider, supportsGlobalQueryFilter, jsonFormatting) { }

    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        // Top-level property info: fields, read-only, JSON conversion, etc
        builder.Property("_nonce");
        builder.Property("_text");
        builder.Property("_tag");
        builder.Property("_text2");
    }
}
