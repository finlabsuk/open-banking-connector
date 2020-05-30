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
    public static class BankClientProfileContextInteractionExtensions
    {
        private static readonly Lens<BankClientProfileContext, BankClientProfile> DataLens =
            Lens.Create(get: (BankClientProfileContext c) => c.Data, set: (c, d) => c.Data = d);

        private static readonly Lens<BankClientProfile, OpenIdConfigurationOverrides> OpenIdLens =
            Lens.Create(
                get: (BankClientProfile c) => c.OpenIdConfigurationOverrides,
                set: (c, d) => c.OpenIdConfigurationOverrides = d);

        public static BankClientProfileContext Data(this BankClientProfileContext context, BankClientProfile value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.Data = value;

            return context;
        }

        public static BankClientProfileContext Id(
            this BankClientProfileContext context,
            string id)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).Id = id;

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

        public static BankClientProfileContext SoftwareStatementProfileId(
            this BankClientProfileContext context,
            string value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).SoftwareStatementProfileId = value;

            return context;
        }


        public static BankClientProfileContext OpenIdOverrides(
            this BankClientProfileContext context,
            OpenIdConfigurationOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).OpenIdConfigurationOverrides = value;

            return context;
        }

        public static BankClientProfileContext OpenIdRegistrationEndpointUrl(
            this BankClientProfileContext context,
            Uri value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens)
                .GetOrCreateDefault(OpenIdLens).RegistrationEndpointUrl = value?.ToString();

            return context;
        }

        public static BankClientProfileContext HttpMtlsOverrides(
            this BankClientProfileContext context,
            HttpMtlsConfigurationOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).HttpMtlsConfigurationOverrides = value;

            return context;
        }

        public static BankClientProfileContext RegistrationClaimsOverrides(
            this BankClientProfileContext context,
            BankClientRegistrationClaimsOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).BankClientRegistrationClaimsOverrides = value;

            return context;
        }

        public static BankClientProfileContext RegistrationResponseOverrides(
            this BankClientProfileContext context,
            BankClientRegistrationDataOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).BankClientRegistrationDataOverrides = value;

            return context;
        }

        public static async Task<BankClientProfileFluentResponse> UpsertAsync(this BankClientProfileContext context)
        {
            context.ArgNotNull(nameof(context));

            IList<FluentResponseMessage> validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new BankClientProfileFluentResponse(messages: validationErrors, data: null);
            }

            try
            {
                BankClientProfileResponse result = await PersistOpenBankingClient(context);

                return new BankClientProfileFluentResponse(result);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new BankClientProfileFluentResponse(messages: ex.CreateErrorMessages(), data: null);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new BankClientProfileFluentResponse(message: ex.CreateErrorMessage(), data: null);
            }
        }

        private static async Task<BankClientProfileResponse> PersistOpenBankingClient(BankClientProfileContext context)
        {
            Persistent.BankClientProfile dto =
                context.Context.EntityMapper.Map<Persistent.BankClientProfile>(context.Data);

            Persistent.BankClientProfile persistedDto = await context.Context.ClientProfileRepository.UpsertAsync(dto);
            await context.Context.DbContextService.SaveChangesAsync();

            return new BankClientProfileResponse(persistedDto);
        }

        public static BankClientProfileContext BankClientRegistrationClaimsOverrides(
            this BankClientProfileContext context,
            BankClientRegistrationClaimsOverrides value)
        {
            context.ArgNotNull(nameof(context));

            context.Data.BankClientRegistrationClaimsOverrides = value;

            return context;
        }

        public static async Task<BankClientProfileFluentResponse> SubmitAsync(this BankClientProfileContext context)
        {
            context.ArgNotNull(nameof(context));

            IList<FluentResponseMessage> validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new BankClientProfileFluentResponse(messages: validationErrors, data: null);
            }

            try
            {
                CreateBankClientProfile i = new CreateBankClientProfile(
                    apiClient: context.Context.ApiClient,
                    mapper: context.Context.EntityMapper,
                    dbMultiEntityMethods: context.Context.DbContextService,
                    bankClientProfileRepo: context.Context.ClientProfileRepository,
                    softwareStatementProfileService: context.Context.SoftwareStatementProfileService);

                BankClientProfileResponse resp = await i.CreateAsync(context.Data);

                return new BankClientProfileFluentResponse(resp);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new BankClientProfileFluentResponse(messages: ex.CreateErrorMessages(), data: null);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new BankClientProfileFluentResponse(message: ex.CreateErrorMessage(), data: null);
            }
        }

        private static IList<FluentResponseMessage> Validate(BankClientProfileContext context)
        {
            return new OpenBankingClientValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
