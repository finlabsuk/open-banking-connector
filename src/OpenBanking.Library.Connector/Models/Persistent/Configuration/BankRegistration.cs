// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

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
            builder.Property(e => e.OAuth2RequestObjectClaimsOverrides)
                .HasConversion(
                    convertToProviderExpression: v => JsonConvert.SerializeObject(v, _formatting),
                    convertFromProviderExpression: v =>
                        JsonConvert.DeserializeObject<OAuth2RequestObjectClaimsOverrides>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.OBClientRegistrationRequest)
                .HasConversion(
                    convertToProviderExpression: v => JsonConvert.SerializeObject(v, _formatting),
                    convertFromProviderExpression: v =>
                        JsonConvert.DeserializeObject<OBClientRegistration1>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.BankId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.OBClientRegistration)
                .HasConversion(
                    convertToProviderExpression: v => JsonConvert.SerializeObject(v, _formatting),
                    convertFromProviderExpression: v =>
                        JsonConvert.DeserializeObject<OBClientRegistration1>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            // Top-level foreign keys
            builder
                .HasOne<Persistent.Bank>()
                .WithMany()
                .HasForeignKey(r => r.BankId);

            // Second-level property info and foreign keys
            builder.OwnsOne(
                navigationExpression: e => e.OpenIdConfiguration,
                buildAction: od =>
                {
                    od.Property(p => p.ResponseTypesSupported)
                        .HasConversion(
                            convertToProviderExpression: v => JsonConvert.SerializeObject(v, _formatting),
                            convertFromProviderExpression: v => JsonConvert.DeserializeObject<string[]>(v));
                    od.Property(p => p.ScopesSupported)
                        .HasConversion(
                            convertToProviderExpression: v => JsonConvert.SerializeObject(v, _formatting),
                            convertFromProviderExpression: v => JsonConvert.DeserializeObject<string[]>(v));
                    od.Property(p => p.ResponseModesSupported)
                        .HasConversion(
                            convertToProviderExpression: v => JsonConvert.SerializeObject(v, _formatting),
                            convertFromProviderExpression: v => JsonConvert.DeserializeObject<string[]>(v));
                });
            builder.Navigation(p => p.OpenIdConfiguration).IsRequired();
        }
    }
}
