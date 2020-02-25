// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Validation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public static class ClientContextInteractionExtensions
    {
        private static readonly Lens<ClientContext, OpenBankingClient> DataLens =
            Lens.Create((ClientContext c) => c.Data, (c, d) => c.Data = d);

        private static readonly Lens<OpenBankingClient, OpenIdConfigurationOverrides> OpenIdLens =
            Lens.Create((OpenBankingClient c) => c.OpenIdOverrides, (c, d) => c.OpenIdOverrides = d);

        public static ClientContext Data(this ClientContext context, OpenBankingClient value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.Data = value;

            return context;
        }

        public static ClientContext IssuerUrl(this ClientContext context, Uri value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).IssuerUrl = value?.ToString();

            return context;
        }

        public static ClientContext XFapiFinancialId(this ClientContext context, string value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).XFapiFinancialId = value;

            return context;
        }

        public static ClientContext SoftwareStatementProfileId(this ClientContext context, string value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).SoftwareStatementProfileId = value;

            return context;
        }


        public static ClientContext OpenIdOverrides(this ClientContext context, OpenIdConfigurationOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).OpenIdOverrides = value;

            return context;
        }

        public static ClientContext OpenIdRegistrationEndpointUrl(this ClientContext context, Uri value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens)
                .GetOrCreateDefault(OpenIdLens).RegistrationEndpointUrl = value?.ToString();

            return context;
        }

        public static ClientContext HttpMtlsOverrides(this ClientContext context,
            HttpClientMtlsConfigurationOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).HttpMtlsOverrides = value;

            return context;
        }

        public static ClientContext RegistrationClaimsOverrides(this ClientContext context,
            OpenBankingClientRegistrationClaimsOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).RegistrationClaimsOverrides = value;

            return context;
        }

        public static ClientContext RegistrationResponseOverrides(this ClientContext context,
            ClientRegistrationResponseOverrides value)
        {
            context.ArgNotNull(nameof(context))
                .GetOrCreateDefault(DataLens).RegistrationResponseOverrides = value;

            return context;
        }

        public static async Task<ClientResponse> UpsertAsync(this ClientContext context)
        {
            context.ArgNotNull(nameof(context));

            var validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new ClientResponse(validationErrors, null);
            }

            try
            {
                var result = await PersistOpenBankingClient(context);

                return new ClientResponse(result);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new ClientResponse(ex.CreateErrorMessages(), null);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new ClientResponse(ex.CreateErrorMessage(), null);
            }
        }

        private static async Task<OpenBankingClient> PersistOpenBankingClient(ClientContext context)
        {
            var dto = context.Context.EntityMapper.Map<Model.Persistent.OpenBankingClient>(context.Data);

            var persistedDto = await context.Context.ClientRepository.SetAsync(dto);

            return context.Context.EntityMapper.Map<OpenBankingClient>(persistedDto);
        }

        private static IList<OpenBankingResponseMessage> Validate(ClientContext context)
        {
            return new OpenBankingClientValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
