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
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration
{
    internal class BankRegistration : Base<Persistent.BankRegistration>
    {
        public BankRegistration(bool supportsGlobalQueryFilter, Formatting jsonFormatting) :
            base(
                supportsGlobalQueryFilter,
                jsonFormatting) { }

        public override void Configure(EntityTypeBuilder<Persistent.BankRegistration> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.SoftwareStatementProfileId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.SoftwareStatementAndCertificateProfileOverrideCase)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.RegistrationScope)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<RegistrationScope>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            // TODO: check how this conversion handles invalid data
            builder.Property(e => e.ClientRegistrationApi)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<ClientRegistrationApiVersion>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.OAuth2RequestObjectClaimsOverrides)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _jsonFormatting),
                    v =>
                        JsonConvert.DeserializeObject<OAuth2RequestObjectClaimsOverrides>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.BankId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.BankApiRequest)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _jsonFormatting),
                    v =>
                        JsonConvert.DeserializeObject<ClientRegistrationModelsPublic.OBClientRegistration1>(v)!)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.Name)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.OpenIdConfigurationResponse)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _jsonFormatting),
                    v =>
                        JsonConvert.DeserializeObject<OpenIdConfiguration>(v)!)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.Issuer)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.TokenEndpoint)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.AuthorizationEndpoint)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.RegistrationEndpoint)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.TokenEndpointAuthMethod)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<TokenEndpointAuthMethodEnum>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.RedirectUris)
                .HasConversion(
                    v => string.Join(' ', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries),
                    new ValueComparer<IList<string>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.ExternalApiSecret)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.RegistrationAccessToken)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            // Second-level property info
            builder.OwnsOne(
                p => p.BankApiResponse,
                od =>
                {
                    od.Property(e => e.Data)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v, _jsonFormatting),
                            v =>
                                JsonConvert
                                    .DeserializeObject<ClientRegistrationModelsPublic.OBClientRegistration1Response>(
                                        v)!)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.Modified)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.ModifiedBy)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                });
            builder.Navigation(p => p.BankApiResponse).IsRequired();
        }
    }
}
