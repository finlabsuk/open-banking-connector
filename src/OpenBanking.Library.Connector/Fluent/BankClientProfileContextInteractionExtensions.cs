// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public static class BankClientProfileContextInteractionExtensions
    {
        private static readonly Lens<BankClientProfileContext, BankClientProfile> DataLens =
            Lens.Create((BankClientProfileContext c) => c.Data, (c, d) => c.Data = d);

        private static readonly Lens<BankClientProfile, OpenIdConfigurationOverrides> OpenIdLens =
            Lens.Create((BankClientProfile c) => c.OpenIdConfigurationOverrides, (c, d) => c.OpenIdConfigurationOverrides = d);
        
        public static BankClientProfileContext Data(this BankClientProfileContext context, BankClientProfile value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.Data = value;

            return context;
        }

        public static BankClientProfileContext IssuerUrl(this BankClientProfileContext context, Uri value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).IssuerUrl = value?.ToString();

            return context;
        }

        public static BankClientProfileContext XFapiFinancialId(this BankClientProfileContext context, string value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).XFapiFinancialId = value;

            return context;
        }

        public static BankClientProfileContext SoftwareStatementProfileId(this BankClientProfileContext context, string value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).SoftwareStatementProfileId = value;

            return context;
        }


        public static BankClientProfileContext OpenIdOverrides(this BankClientProfileContext context, OpenIdConfigurationOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).OpenIdConfigurationOverrides = value;

            return context;
        }

        public static BankClientProfileContext OpenIdRegistrationEndpointUrl(this BankClientProfileContext context, Uri value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens)
                .GetOrCreateDefault(OpenIdLens).RegistrationEndpointUrl = value?.ToString();

            return context;
        }

        public static BankClientProfileContext HttpMtlsOverrides(this BankClientProfileContext context,
            HttpClientMtlsConfigurationOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).HttpMtlsOverrides = value;

            return context;
        }

        public static BankClientProfileContext RegistrationClaimsOverrides(this BankClientProfileContext context,
            OpenBankingClientRegistrationClaimsOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).ClientRegistrationClaimsOverrides = value;

            return context;
        }

        public static BankClientProfileContext RegistrationResponseOverrides(this BankClientProfileContext context,
            ClientRegistrationResponseOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).ClientRegistrationDataOverrides = value;

            return context;
        }

        public static async Task<BankClientProfileResponse> UpsertAsync(this BankClientProfileContext context)
        {
            context.ArgNotNull(nameof(context));

            var validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new BankClientProfileResponse(validationErrors, null);
            }

            try
            {
                var result = await PersistOpenBankingClient(context);

                return new BankClientProfileResponse(result);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new BankClientProfileResponse(ex.CreateErrorMessages(), null);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new BankClientProfileResponse(ex.CreateErrorMessage(), null);
            }
        }

        private static async Task<Model.Public.Response.BankClientProfile> PersistOpenBankingClient(BankClientProfileContext context)
        {
            var dto = context.Context.EntityMapper.Map<Model.Persistent.BankClientProfile>(context.Data);

            var persistedDto = await context.Context.ClientProfileRepository.SetAsync(dto);

            return new Model.Public.Response.BankClientProfile(persistedDto);
        }

        public static BankClientProfileContext OpenBankingClientRegistrationClaimsOverrides(
            this BankClientProfileContext context,
            OpenBankingClientRegistrationClaimsOverrides value)
        {
            context.ArgNotNull(nameof(context));

            context.Data.ClientRegistrationClaimsOverrides = value;

            return context;
        }

        public static async Task<BankClientProfileResponse> SubmitAsync(this BankClientProfileContext context)
        {
            context.ArgNotNull(nameof(context));

            var validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new BankClientProfileResponse(validationErrors, null);
            }

            try
            {
                var i = new CreateBankClientProfile(
                    context.Context.ApiClient,
                    context.Context.EntityMapper,
                    context.Context.SoftwareStatementRepository,
                    context.Context.ClientProfileRepository
                );

                var resp = await i.CreateAsync(context.Data);

                return new BankClientProfileResponse(resp);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new BankClientProfileResponse(ex.CreateErrorMessages(), null);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new BankClientProfileResponse(ex.CreateErrorMessage(), null);
            }
        }

        private static IList<OpenBankingResponseMessage> Validate(BankClientProfileContext context)
        {
            return new OpenBankingClientValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
