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

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration;

internal class BankPost : LocalEntityCreate<Bank, Models.Public.BankConfiguration.Request.Bank, BankResponse>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly IOpenIdConfigurationRead _configurationRead;

    public BankPost(
        IDbReadWriteEntityMethods<Bank> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService,
        IOpenIdConfigurationRead configurationRead) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        softwareStatementProfileRepo,
        instrumentationClient)
    {
        _bankProfileService = bankProfileService;
        _configurationRead = configurationRead;
    }

    protected override async Task<BankResponse> AddEntity(
        Models.Public.BankConfiguration.Request.Bank request,
        ITimeProvider timeProvider)
    {
        // Create non-error list
        IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        BankProfile? bankProfile = request.BankProfile is { } bankProfileEnum
            ? _bankProfileService.GetBankProfile(bankProfileEnum)
            : null;

        CustomBehaviourClass? customBehaviour =
            request.CustomBehaviour ??
            bankProfile?.CustomBehaviour;

        string issuerUrl =
            request.IssuerUrl ??
            bankProfile?.IssuerUrl ??
            throw new InvalidOperationException(
                "IssuerUrl specified as null and cannot be obtained using specified BankProfile.");

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

        DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion =
            request.DynamicClientRegistrationApiVersion ??
            bankProfile?.DynamicClientRegistrationApiVersion ??
            throw new InvalidOperationException(
                "DynamicClientRegistrationApiVersion specified as null and cannot be obtained using specified BankProfile.");

        // Get OpenID Provider Configuration if available
        (OpenIdConfiguration? openIdConfiguration,
                IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages1) =
            await _configurationRead.GetOpenIdConfigurationAsync(
                issuerUrl,
                customBehaviour?.OpenIdConfigurationGet);
        nonErrorMessages.AddRange(newNonErrorMessages1);

        string registrationEndpoint =
            request.RegistrationEndpoint ??
            openIdConfiguration?.RegistrationEndpoint ??
            throw new ArgumentNullException(
                nameof(request.RegistrationEndpoint),
                $"{nameof(request.RegistrationEndpoint)} specified as null and not obtainable from OpenID Configuration for specified IssuerUrl. ");

        string tokenEndpoint =
            request.TokenEndpoint ??
            openIdConfiguration?.TokenEndpoint ??
            throw new ArgumentNullException(
                nameof(request.TokenEndpoint),
                $"{nameof(request.TokenEndpoint)} specified as null and not obtainable from OpenID Configuration for specified IssuerUrl. ");

        string authorizationEndpoint =
            request.AuthorizationEndpoint ??
            openIdConfiguration?.AuthorizationEndpoint ??
            throw new ArgumentNullException(
                nameof(request.AuthorizationEndpoint),
                $"{nameof(request.AuthorizationEndpoint)} specified as null and not obtainable from OpenID Configuration for specified IssuerUrl. ");

        string jwksUri =
            request.JwksUri ??
            openIdConfiguration?.JwksUri ??
            throw new ArgumentNullException(
                nameof(request.JwksUri),
                $"{nameof(request.JwksUri)} specified as null and not obtainable from OpenID Configuration for specified IssuerUrl. ");
        
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
            jwksUri,
            supportsSca,
            issuerUrl,
            financialId,
            registrationEndpoint,
            tokenEndpoint,
            authorizationEndpoint,
            dynamicClientRegistrationApiVersion,
            customBehaviour);

        // Add entity
        await _entityMethods.AddAsync(entity);

        // Create response
        return entity.PublicGetLocalResponse;
    }
}
