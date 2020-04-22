// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
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

        public static SoftwareStatementProfileContext Id(
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

        public static SoftwareStatementProfileContext SigningKeyInfo(this SoftwareStatementProfileContext context,
            string keyId,
            string keySecretName,
            string certificate
            )
        {
            SoftwareStatementProfile data = context.ArgNotNull(nameof(context)).GetOrCreateDefault(DataLens);

            data.SigningKeyId = keyId;
            data.SigningKeySecretName = keySecretName;
            data.SigningCertificate = certificate;

            return context;
        }

        public static SoftwareStatementProfileContext TransportKeyInfo(this SoftwareStatementProfileContext context,
            string keySecretName,
            string certificate
        )
        {
            SoftwareStatementProfile data = context.ArgNotNull(nameof(context)).GetOrCreateDefault(DataLens);

            data.TransportKeySecretName = keySecretName;
            data.TransportCertificate = certificate;

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

            IList<FluentResponseMessage> validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new OpenBankingSoftwareStatementResponse(validationErrors, null);
            }

            CreateSoftwareStatementProfile creator = new CreateSoftwareStatementProfile(
                context.Context.EntityMapper,
                context.Context.DbContextService,
                context.Context.SoftwareStatementRepository
                );

            List<FluentResponseMessage> messages = new List<FluentResponseMessage>();

            try
            {
                SoftwareStatementProfileResponse response = await creator.CreateAsync(context.Data);

                return new OpenBankingSoftwareStatementResponse(messages, response);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new OpenBankingSoftwareStatementResponse(ex.CreateErrorMessage(), null);
            }
        }

        private static IList<FluentResponseMessage> Validate(SoftwareStatementProfileContext context)
        {
            return new SoftwareStatementProfileValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
