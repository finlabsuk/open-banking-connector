// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using DomesticPaymentConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.
    DomesticPaymentConsentAuthContext;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation;

public interface IDomesticPaymentConsentAuthContextsContext :
    ICreateLocalContext<DomesticPaymentConsentAuthContextRequest, DomesticPaymentConsentAuthContextCreateResponse>,
    IReadLocal2Context<DomesticPaymentConsentAuthContextReadResponse> { }

internal class DomesticPaymentConsentAuthContextsContext :
    IDomesticPaymentConsentAuthContextsContext
{
    private readonly ISharedContext _sharedContext;

    public DomesticPaymentConsentAuthContextsContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;

        var domesticPaymentConsentCommon = new DomesticPaymentConsentCommon(
            _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsent>(),
            _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
            _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
            _sharedContext.Instrumentation,
            _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
            _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
            _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
            _sharedContext.DbService.GetDbMethods());
        var domesticPaymentConsentAuthContextOperations = new DomesticPaymentConsentAuthContextOperations(
            _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAuthContextPersisted>(),
            _sharedContext.DbService.GetDbMethods(),
            _sharedContext.TimeProvider,
            _sharedContext.Instrumentation,
            _sharedContext.BankProfileService,
            _sharedContext.ObSealCertificateMethods,
            domesticPaymentConsentCommon);
        CreateLocalObject = domesticPaymentConsentAuthContextOperations;
        ReadLocalObject = domesticPaymentConsentAuthContextOperations;
    }

    public IObjectCreate<DomesticPaymentConsentAuthContextRequest, DomesticPaymentConsentAuthContextCreateResponse,
        LocalCreateParams> CreateLocalObject { get; }

    public IObjectRead<DomesticPaymentConsentAuthContextReadResponse, LocalReadParams> ReadLocalObject { get; }
}
