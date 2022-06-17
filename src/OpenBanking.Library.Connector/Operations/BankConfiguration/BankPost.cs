// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration
{
    internal class BankPost : LocalEntityCreate<Bank, Models.Public.BankConfiguration.Request.Bank, BankResponse>
    {
        private readonly IBankProfileDefinitions _bankProfileDefinitions;
        private readonly IOpenIdConfigurationRead _configurationRead;

        public BankPost(
            IDbReadWriteEntityMethods<Bank> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IBankProfileDefinitions bankProfileDefinitions,
            IOpenIdConfigurationRead configurationRead) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient)
        {
            _bankProfileDefinitions = bankProfileDefinitions;
            _configurationRead = configurationRead;
        }

        protected override async Task<BankResponse> AddEntity(
            Models.Public.BankConfiguration.Request.Bank request,
            ITimeProvider timeProvider)
        {
            // Create non-error list
            IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            BankProfile? bankProfile = null;
            if (request.BankProfile is not null)
            {
                bankProfile = _bankProfileDefinitions.GetBankProfile(request.BankProfile.Value);
            }

            CustomBehaviourClass? customBehaviour =
                request.CustomBehaviour ??
                bankProfile?.CustomBehaviour;

            string? issuerUrl =
                request.IssuerUrl ??
                bankProfile?.IssuerUrl;

            Uri? openIdConfigurationUrl = null;
            if (customBehaviour?.OpenIdConfigurationGet?.EndpointUnavailable is not true)
            {
                openIdConfigurationUrl = new Uri(
                    customBehaviour?.OpenIdConfigurationGet?.Url
                    ?? string.Join("/", issuerUrl.TrimEnd('/'), ".well-known/openid-configuration"));
            }

            string financialId =
                request.FinancialId ??
                bankProfile?.FinancialId ??
                throw new InvalidOperationException(
                    "FinancialId specified as null and cannot be obtained using specified BankProfile.");

            bool supportsSca =
                request.SupportsSca ??
                bankProfile?.SupportsSca ??
                throw new InvalidOperationException(
                    "SupportsSca specified as null and cannot be obtained using specified BankProfile.");

            OAuth2ResponseMode defaultResponseMode =
                request.DefaultResponseMode ??
                bankProfile?.DefaultResponseMode ??
                throw new InvalidOperationException(
                    "DefaultResponseMode specified as null and cannot be obtained using specified BankProfile.");

            DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion =
                request.DynamicClientRegistrationApiVersion ??
                bankProfile?.DynamicClientRegistrationApiVersion ??
                throw new InvalidOperationException(
                    "DynamicClientRegistrationApiVersion specified as null and cannot be obtained using specified BankProfile.");

            // Get OpenID Provider Configuration if issuer URL available and determine endpoints appropriately
            string registrationEndpoint;
            string tokenEndpoint;
            string authorizationEndpoint;
            string jwksUri;
            if (openIdConfigurationUrl is null)
            {
                // Determine endpoints
                registrationEndpoint =
                    request.RegistrationEndpoint ??
                    throw new ArgumentNullException(
                        null,
                        "RegistrationEndpoint specified as null and cannot be obtained using IssuerUrl (also null).");
                tokenEndpoint =
                    request.TokenEndpoint ??
                    throw new ArgumentNullException(
                        null,
                        "TokenEndpoint specified as null and cannot be obtained using IssuerUrl (also null).");
                authorizationEndpoint =
                    request.AuthorizationEndpoint ??
                    throw new ArgumentNullException(
                        null,
                        "AuthorizationEndpoint specified as null and cannot be obtained using IssuerUrl (also null).");
                jwksUri =
                    request.JwksUri ??
                    throw new ArgumentNullException(
                        null,
                        "JwksUri specified as null and cannot be obtained using IssuerUrl (also null).");
            }
            else
            {
                (OpenIdConfiguration openIdConfiguration,
                        IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                    await _configurationRead.GetOpenIdConfigurationAsync(
                        openIdConfigurationUrl,
                        customBehaviour?.OpenIdConfigurationGet);
                nonErrorMessages.AddRange(newNonErrorMessages);

                // Determine endpoints
                registrationEndpoint =
                    request.RegistrationEndpoint ?? openIdConfiguration.RegistrationEndpoint;
                tokenEndpoint =
                    request.TokenEndpoint ?? openIdConfiguration.TokenEndpoint;
                authorizationEndpoint =
                    request.AuthorizationEndpoint ?? openIdConfiguration.AuthorizationEndpoint;
                jwksUri =
                    request.JwksUri ?? openIdConfiguration.JwksUri;
            }

            // Create persisted entity
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var entity = new Bank(
                Guid.NewGuid(),
                request.Reference,
                false,
                utcNow,
                request.CreatedBy,
                utcNow,
                request.CreatedBy,
                issuerUrl,
                financialId,
                registrationEndpoint,
                tokenEndpoint,
                authorizationEndpoint,
                jwksUri,
                defaultResponseMode,
                dynamicClientRegistrationApiVersion,
                customBehaviour,
                supportsSca);

            // Add entity
            await _entityMethods.AddAsync(entity);

            // Create response
            return entity.PublicGetLocalResponse;
        }
    }
}
