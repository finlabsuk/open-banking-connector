// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.AccountTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using SoftwareStatementProfile =
    FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public static class ClientProfileInteractionExtensions
    {
        private static readonly Lens<ClientProfileContext, OpenBankingClientProfile> DataLens =
            Lens.Create((ClientProfileContext c) => c.Data, (c, d) => c.Data = d);

        private static readonly Lens<OpenBankingClientProfile, OpenBankingClient> ObClientLens =
            Lens.Create((OpenBankingClientProfile c) => c.OpenBankingClient, (c, d) => c.OpenBankingClient = d);

        public static ClientProfileContext Data(this ClientProfileContext context, OpenBankingClientProfile value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.Data = value;

            return context;
        }

        public static ClientProfileContext SoftwareStatementProfileId(
            this ClientProfileContext context, string id)
        {
            context.ArgNotNull(nameof(context));

            context.GetOrCreateDefault(DataLens)
                .GetOrCreateDefault(ObClientLens).SoftwareStatementProfileId = id;

            return context;
        }

        public static ClientProfileContext IssuerUrl(this ClientProfileContext context, string url)
        {
            context.ArgNotNull(nameof(context));

            context.GetOrCreateDefault(DataLens)
                .GetOrCreateDefault(ObClientLens).IssuerUrl = url;

            return context;
        }

        public static ClientProfileContext RedirectUrl(this ClientProfileContext context, string url)
        {
            context.ArgNotNull(nameof(context));

            context.GetOrCreateDefault(DataLens)
                .RedirectUrl = url;

            return context;
        }

        public static ClientProfileContext XFapiFinancialId(this ClientProfileContext context,
            string value)
        {
            context.ArgNotNull(nameof(context));

            context.GetOrCreateDefault(DataLens)
                .GetOrCreateDefault(ObClientLens).XFapiFinancialId = value;

            return context;
        }

        public static ClientProfileContext AccountTransactionApi(
            this ClientProfileContext context,
            AccountApiVersion version, string url)
        {
            context.ArgNotNull(nameof(context));

            var data = context.GetOrCreateDefault(DataLens);

            data.AccountTransactionApiVersion = version;
            data.AccountTransactionApiBaseUrl = url;

            return context;
        }


        public static ClientProfileContext PaymentInitiationApi(this ClientProfileContext context,
            ApiVersion version, string url)
        {
            context.ArgNotNull(nameof(context));

            var data = context.GetOrCreateDefault(DataLens);

            data.PaymentInitiationApiVersion = version;
            data.PaymentInitiationApiBaseUrl = url;

            return context;
        }

        public static ClientProfileContext OpenBankingClientRegistrationClaimsOverrides(
            this ClientProfileContext context,
            OpenBankingClientRegistrationClaimsOverrides value)
        {
            context.ArgNotNull(nameof(context));

            context.GetOrCreateDefault(DataLens)
                .GetOrCreateDefault(ObClientLens).RegistrationClaimsOverrides = value;

            return context;
        }

        public static async Task<ClientProfileResponse> SubmitAsync(this ClientProfileContext context)
        {
            context.ArgNotNull(nameof(context));

            var validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new ClientProfileResponse(validationErrors, null);
            }

            try
            {
                var i = new CreateClientProfile(
                    context.Context.EntityMapper,
                    context.Context.ApiClient,
                    context.Context.SoftwareStatementRepository,
                    context.Context.ClientProfileRepository,
                    context.Context.ClientRepository
                );

                var resp = await i.CreateAsync(context.Data);

                return new ClientProfileResponse(resp);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new ClientProfileResponse(ex.CreateErrorMessages(), null);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new ClientProfileResponse(ex.CreateErrorMessage(), null);
            }
        }

        private static async Task<SoftwareStatementProfile> GetSoftwareStatement(ClientProfileContext context)
        {
            var repo = context.Context.SoftwareStatementRepository;

            return await repo.GetAsync(context.Data.OpenBankingClient.SoftwareStatementProfileId);
        }

        private static IList<OpenBankingResponseMessage> Validate(ClientProfileContext context)
        {
            return new OpenBankingClientProfileValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
