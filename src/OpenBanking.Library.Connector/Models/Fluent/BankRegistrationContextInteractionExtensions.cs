// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public static class BankRegistrationContextInteractionExtensions
    {
        private static readonly Lens<BankRegistrationContext, BankRegistration> DataLens =
            Lens.Create(get: (BankRegistrationContext c) => c.Data, set: (c, d) => c.Data = d);

        private static readonly Lens<BankRegistration, OpenIdConfigurationOverrides> OpenIdLens =
            Lens.Create(
                get: (BankRegistration c) => c.OpenIdConfigurationOverrides,
                set: (c, d) => c.OpenIdConfigurationOverrides = d);

        public static BankRegistrationContext Data(this BankRegistrationContext context, BankRegistration value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.Data = value;

            return context;
        }

        public static BankRegistrationContext SoftwareStatementProfileId(
            this BankRegistrationContext context,
            string value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).SoftwareStatementProfileId = value;

            return context;
        }


        public static BankRegistrationContext OpenIdOverrides(
            this BankRegistrationContext context,
            OpenIdConfigurationOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).OpenIdConfigurationOverrides = value;

            return context;
        }

        public static BankRegistrationContext OpenIdRegistrationEndpointUrl(
            this BankRegistrationContext context,
            Uri value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens)
                .GetOrCreateDefault(OpenIdLens).RegistrationEndpointUrl = value?.ToString();

            return context;
        }

        public static BankRegistrationContext HttpMtlsOverrides(
            this BankRegistrationContext context,
            HttpMtlsConfigurationOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).HttpMtlsConfigurationOverrides = value;

            return context;
        }

        public static BankRegistrationContext RegistrationClaimsOverrides(
            this BankRegistrationContext context,
            BankClientRegistrationClaimsOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).BankClientRegistrationClaimsOverrides = value;

            return context;
        }

        public static BankRegistrationContext RegistrationResponseOverrides(
            this BankRegistrationContext context,
            RegistrationResponseJsonOptions value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).RegistrationResponseJsonOptions = value;

            return context;
        }

        public static async Task<FluentResponse<BankRegistrationResponse>> UpsertAsync(
            this BankRegistrationContext context)
        {
            context.ArgNotNull(nameof(context));

            IList<FluentResponseMessage> validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new FluentResponse<BankRegistrationResponse>(messages: validationErrors);
            }

            try
            {
                BankRegistrationResponse result = await PersistOpenBankingClient(context);

                return new FluentResponse<BankRegistrationResponse>(result);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new FluentResponse<BankRegistrationResponse>(messages: ex.CreateErrorMessages());
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new FluentResponse<BankRegistrationResponse>(message: ex.CreateErrorMessage());
            }
        }

        private static async Task<BankRegistrationResponse> PersistOpenBankingClient(BankRegistrationContext context)
        {
            Persistent.BankRegistration dto =
                context.Context.EntityMapper.Map<Persistent.BankRegistration>(context.Data);

            Persistent.BankRegistration
                persistedDto = await context.Context.BankRegistrationRepository.UpsertAsync(dto);
            await context.Context.DbContextService.SaveChangesAsync();

            return new BankRegistrationResponse(persistedDto);
        }

        public static BankRegistrationContext BankClientRegistrationClaimsOverrides(
            this BankRegistrationContext context,
            BankClientRegistrationClaimsOverrides value)
        {
            context.ArgNotNull(nameof(context));

            context.Data.BankClientRegistrationClaimsOverrides = value;

            return context;
        }

        public static async Task<FluentResponse<BankRegistrationResponse>> SubmitAsync(
            this BankRegistrationContext context)
        {
            context.ArgNotNull(nameof(context));

            IList<FluentResponseMessage> validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new FluentResponse<BankRegistrationResponse>(messages: validationErrors);
            }

            try
            {
                CreateBankRegistration i = new CreateBankRegistration(
                    apiClient: context.Context.ApiClient,
                    bankRegistrationRepo: context.Context.BankRegistrationRepository,
                    bankRepo: context.Context.BankRepository,
                    dbMultiEntityMethods: context.Context.DbContextService,
                    softwareStatementProfileService: context.Context.SoftwareStatementProfileService);

                BankRegistrationResponse resp = await i.CreateAsync(context.Data);

                return new FluentResponse<BankRegistrationResponse>(resp);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new FluentResponse<BankRegistrationResponse>(messages: ex.CreateErrorMessages());
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new FluentResponse<BankRegistrationResponse>(message: ex.CreateErrorMessage());
            }
        }

        private static IList<FluentResponseMessage> Validate(BankRegistrationContext context)
        {
            return new OpenBankingClientValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
