// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration;

internal class AccountAndTransactionApiPost : LocalEntityCreate<AccountAndTransactionApiEntity,
    AccountAndTransactionApiRequest, AccountAndTransactionApiResponse>
{
    private readonly IBankProfileService _bankProfileService;

    public AccountAndTransactionApiPost(
        IDbReadWriteEntityMethods<AccountAndTransactionApiEntity> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        softwareStatementProfileRepo,
        instrumentationClient)
    {
        _bankProfileService = bankProfileService;
    }

    protected override async Task<AccountAndTransactionApiResponse> AddEntity(
        AccountAndTransactionApiRequest request,
        ITimeProvider timeProvider)
    {
        // Get bank profile if available
        BankProfile? bankProfile = null;
        if (request.BankProfile is not null)
        {
            bankProfile = _bankProfileService.GetBankProfile(request.BankProfile.Value);
        }

        // Get API version
        AccountAndTransactionApiVersion apiVersion =
            request.ApiVersion ??
            bankProfile?.GetRequiredAccountAndTransactionApi().ApiVersion ??
            throw new InvalidOperationException(
                "ApiVersion specified as null and cannot be obtained from specified BankProfile.");

        // Get base URL
        string baseUrl =
            request.BaseUrl ??
            bankProfile?.GetRequiredAccountAndTransactionApi().BaseUrl ??
            throw new InvalidOperationException(
                "BaseUrl specified as null and cannot be obtained from specified BankProfile.");

        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var entity = new AccountAndTransactionApiEntity(
            request.Reference,
            Guid.NewGuid(),
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            request.BankId,
            apiVersion,
            baseUrl.TrimEnd('/'));

        // Add entity
        await _entityMethods.AddAsync(entity);

        // Create response
        return entity.PublicGetLocalResponse;
    }
}
