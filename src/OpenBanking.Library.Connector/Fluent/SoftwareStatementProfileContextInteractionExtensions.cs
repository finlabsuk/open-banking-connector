// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public static class SoftwareStatementProfileContextInteractionExtensions
    {
        private static readonly Lens<SoftwareStatementProfileContext, SoftwareStatementProfile> DataLens =
            Lens.Create((SoftwareStatementProfileContext c) => c.Data, (c, d) => c.Data = d);

        public static SoftwareStatementProfileContext Data(this SoftwareStatementProfileContext context,
            SoftwareStatementProfile value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.Data = value;

            return context;
        }

        public static SoftwareStatementProfileContext SoftwareStatementProfileId(
            this SoftwareStatementProfileContext context,
            string id)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).Id = id;

            return context;
        }

        public static SoftwareStatementProfileContext SoftwareStatement(this SoftwareStatementProfileContext context,
            string statement)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).SoftwareStatement = statement;

            return context;
        }

        public static SoftwareStatementProfileContext SigningKeyId(this SoftwareStatementProfileContext context,
            string value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).ObSigningKid = value;

            return context;
        }

        public static SoftwareStatementProfileContext SigningCertificate(this SoftwareStatementProfileContext context,
            string key, string certificate)
        {
            var data = context.ArgNotNull(nameof(context)).GetOrCreateDefault(DataLens);

            data.ObSigningKey = key;
            data.ObSigningPem = certificate;

            return context;
        }

        public static SoftwareStatementProfileContext TransportCertificate(this SoftwareStatementProfileContext context,
            string key, string certificate)
        {
            var data = context.ArgNotNull(nameof(context)).GetOrCreateDefault(DataLens);

            data.ObTransportKey = key;
            data.ObTransportPem = certificate;

            return context;
        }

        public static SoftwareStatementProfileContext DefaultFragmentRedirectUrl(
            this SoftwareStatementProfileContext context, string url)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).DefaultFragmentRedirectUrl = url;

            return context;
        }

        public static async Task<OpenBankingSoftwareStatementResponse> SubmitAsync(
            this SoftwareStatementProfileContext context)
        {
            context.ArgNotNull(nameof(context));

            var validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new OpenBankingSoftwareStatementResponse(validationErrors, null);
            }

            var creator = new CreateSoftwareStatementProfile(context.Context.SoftwareStatementRepository,
                context.Context.EntityMapper);

            var messages = new List<OpenBankingResponseMessage>();

            try
            {
                var response = await creator.CreateAsync(context.Data);

                return new OpenBankingSoftwareStatementResponse(messages, response);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new OpenBankingSoftwareStatementResponse(ex.CreateErrorMessage(), null);
            }
        }

        private static IList<OpenBankingResponseMessage> Validate(SoftwareStatementProfileContext context)
        {
            return new SoftwareStatementProfileValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
