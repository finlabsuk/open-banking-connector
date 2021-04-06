// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration
{
    internal class BankRegistration : Base<Persistent.BankRegistration>
    {
        private readonly Formatting _formatting;

        public BankRegistration(Formatting formatting)
        {
            _formatting = formatting;
        }

        public override void Configure(EntityTypeBuilder<Persistent.BankRegistration> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.SoftwareStatementProfileId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.RegistrationScope)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            // TODO: check how this conversion handles invalid data
            builder.Property(e => e.ClientRegistrationApi)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<ClientRegistrationApiVersion>(v));
            builder.Property(e => e.OAuth2RequestObjectClaimsOverrides)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _formatting),
                    v =>
                        JsonConvert.DeserializeObject<OAuth2RequestObjectClaimsOverrides>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.OBClientRegistrationRequest)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _formatting),
                    v =>
                        JsonConvert.DeserializeObject<ClientRegistrationModelsPublic.OBClientRegistration1>(v)!)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.BankId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.OBClientRegistration)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _formatting),
                    v =>
                        JsonConvert.DeserializeObject<ClientRegistrationModelsPublic.OBClientRegistration1>(v)!)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.Name)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            // Top-level foreign keys
            builder
                .HasOne<Persistent.Bank>()
                .WithMany()
                .HasForeignKey(r => r.BankId);

            // Second-level property info and foreign keys
            builder.OwnsOne(
                e => e.OpenIdConfiguration,
                od =>
                {
                    od.Property(p => p.ResponseTypesSupported)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v, _formatting),
                            v => JsonConvert.DeserializeObject<IList<string>>(v)!,
                            new ValueComparer<IList<string>>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c));
                    od.Property(p => p.ScopesSupported)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v, _formatting),
                            v => JsonConvert.DeserializeObject<IList<string>>(v)!,
                            new ValueComparer<IList<string>>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c));
                    od.Property(p => p.ResponseModesSupported)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v, _formatting),
                            v => JsonConvert.DeserializeObject<IList<string>>(v)!,
                            new ValueComparer<IList<string>>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c));
                });
            builder.Navigation(p => p.OpenIdConfiguration).IsRequired();
        }
    }
}
