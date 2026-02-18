// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments;

public interface IDomesticVrpConsentAuthContextsContext :
    ICreateLocalContext<DomesticVrpConsentAuthContextRequest, DomesticVrpConsentAuthContextCreateResponse>,
    IReadLocal2Context<DomesticVrpConsentAuthContextReadResponse> { }

internal class DomesticVrpConsentAuthContextsContext :
    IDomesticVrpConsentAuthContextsContext
{
    private readonly ISharedContext _sharedContext;

    public DomesticVrpConsentAuthContextsContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;
        var domesticVrpConsentCommon = new DomesticVrpConsentCommon(
            _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsent>(),
            _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentAccessToken>(),
            _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentRefreshToken>(),
            _sharedContext.Instrumentation,
            _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
            _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
            _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
            _sharedContext.DbService.GetDbMethods());
        var domesticVrpConsentAuthContextOperations = new DomesticVrpConsentAuthContextOperations(
            _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentAuthContextPersisted>(),
            _sharedContext.DbService.GetDbMethods(),
            _sharedContext.TimeProvider,
            _sharedContext.Instrumentation,
            _sharedContext.BankProfileService,
            _sharedContext.ObSealCertificateMethods,
            domesticVrpConsentCommon);
        CreateLocalObject = domesticVrpConsentAuthContextOperations;
        ReadLocalObject = domesticVrpConsentAuthContextOperations;
    }

    public IObjectCreate<DomesticVrpConsentAuthContextRequest, DomesticVrpConsentAuthContextCreateResponse,
        LocalCreateParams> CreateLocalObject { get; }

    public IObjectRead<DomesticVrpConsentAuthContextReadResponse, LocalReadParams> ReadLocalObject { get; }
}
