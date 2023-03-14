// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.EntityFrameworkCore;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class
    ConsentCommon<TEntity, TPublicRequest, TPublicResponse, TApiRequest, TApiResponse>
    where TEntity : class, IEntity
    where TPublicRequest : ConsentRequestBase
    where TApiRequest : class, ISupportsValidation
    where TApiResponse : class, ISupportsValidation
{
    private readonly IDbReadOnlyEntityMethods<BankRegistrationPersisted> _bankRegistrationMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;

    public ConsentCommon(
        IDbReadOnlyEntityMethods<BankRegistrationPersisted> bankRegistrationMethods,
        IInstrumentationClient instrumentationClient,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo)
    {
        _bankRegistrationMethods = bankRegistrationMethods;
        _instrumentationClient = instrumentationClient;
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
    }

    public async
        Task<(BankRegistrationPersisted bankRegistration, string bankFinancialId, string tokenEndpoint,
            CustomBehaviourClass? customBehaviour, ProcessedSoftwareStatementProfile
            processedSoftwareStatementProfile)> GetBankRegistration(Guid bankRegistrationId)
    {
        // Load BankRegistration
        BankRegistrationPersisted bankRegistration =
            await _bankRegistrationMethods
                .DbSetNoTracking
                .Include(o => o.BankNavigation)
                .SingleOrDefaultAsync(x => x.Id == bankRegistrationId) ??
            throw new KeyNotFoundException(
                $"No record found for BankRegistrationId {bankRegistrationId} specified by request.");
        string bankFinancialId = bankRegistration.BankNavigation.FinancialId;
        string tokenEndpoint = bankRegistration.BankNavigation.TokenEndpoint;
        CustomBehaviourClass? customBehaviour = bankRegistration.BankNavigation.CustomBehaviour;

        // Get software statement profile
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await _softwareStatementProfileRepo.GetAsync(
                bankRegistration.SoftwareStatementProfileId,
                bankRegistration.SoftwareStatementProfileOverride);

        return (bankRegistration, bankFinancialId, tokenEndpoint, customBehaviour,
            processedSoftwareStatementProfile);
    }
}
