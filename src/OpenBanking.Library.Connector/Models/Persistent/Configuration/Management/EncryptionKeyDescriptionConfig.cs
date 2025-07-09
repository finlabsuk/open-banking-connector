// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.Management;

internal class EncryptionKeyDescriptionConfig(
    bool supportsGlobalQueryFilter,
    DbProvider dbProvider,
    bool isRelationalDatabase,
    Formatting jsonFormatting)
    : BaseConfig<EncryptionKeyDescriptionEntity>(
        supportsGlobalQueryFilter,
        dbProvider,
        isRelationalDatabase,
        jsonFormatting)
{
    public override void Configure(EntityTypeBuilder<EncryptionKeyDescriptionEntity> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.Key)
            .HasConversion(
                v =>
                    JsonConvert.SerializeObject(
                        v,
                        _jsonFormatting,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<SecretDescription>(v)!)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
    }
}
