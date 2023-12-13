// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration;
using Bank = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request.Bank;
using BankPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.Bank;
using ObSealCertificate =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request.ObSealCertificate;
using ObWacCertificate =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request.ObWacCertificate;
using SoftwareStatement =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request.SoftwareStatement;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.BankConfiguration;

public interface IManagementContext
{
    /// <summary>
    ///     API for Bank objects.
    ///     A Bank is the base object for a bank in Open Banking Connector and is parent to BankRegistration and BankApiSet
    ///     objects
    /// </summary>
    ILocalEntityContext<Bank, IBankPublicQuery, BankResponse, BankResponse> Banks { get; }

    /// <summary>
    ///     API for BankRegistration objects.
    ///     A BankRegistration corresponds to an OAuth2 client registration with a Bank. Multiple BankRegistrations may be
    ///     created for the same bank.
    /// </summary>
    IBankRegistrationsContext BankRegistrations { get; }

    ILocalEntityContext<ObWacCertificate, IObWacCertificatePublicQuery,
            ObWacCertificateResponse, ObWacCertificateResponse>
        ObWacCertificates { get; }

    ILocalEntityContext<ObSealCertificate, IObSealCertificatePublicQuery,
            ObSealCertificateResponse, ObSealCertificateResponse>
        ObSealCertificates { get; }

    ILocalEntityContext<SoftwareStatement, ISoftwareStatementPublicQuery,
            SoftwareStatementResponse, SoftwareStatementResponse>
        SoftwareStatements { get; }

    /// <summary>
    ///     API for AccountAndTransactionApi objects.
    ///     An AccountAndTransactionApi specifies a functional Account and Transaction API supported by a Bank. Multiple
    ///     AccountAndTransactionApis may be created for the same bank.
    /// </summary>
    ILocalEntityContext<AccountAndTransactionApiRequest, IAccountAndTransactionApiQuery,
            AccountAndTransactionApiResponse, AccountAndTransactionApiResponse>
        AccountAndTransactionApis { get; }

    /// <summary>
    ///     API for PaymentInitiationApi objects.
    ///     A PaymentInitiationApi specifies a functional Payment Initiation API supported by a Bank. Multiple
    ///     PaymentInitiationApis may be created for the same bank.
    /// </summary>
    ILocalEntityContext<PaymentInitiationApiRequest, IPaymentInitiationApiQuery,
            PaymentInitiationApiResponse, PaymentInitiationApiResponse>
        PaymentInitiationApis { get; }

    /// <summary>
    ///     API for VariableRecurringPaymentsApi objects.
    ///     A VariableRecurringPaymentsApi specifies a functional Variable Recurring Payments API supported by a Bank. Multiple
    ///     VariableRecurringPaymentsApis may be created for the same bank.
    /// </summary>
    ILocalEntityContext<VariableRecurringPaymentsApiRequest, IVariableRecurringPaymentsApiQuery,
            VariableRecurringPaymentsApiResponse, VariableRecurringPaymentsApiResponse>
        VariableRecurringPaymentsApis { get; }
}

internal class ManagementContext : IManagementContext
{
    private readonly ISharedContext _sharedContext;

    public ManagementContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;
    }

    public ILocalEntityContext<Bank, IBankPublicQuery, BankResponse, BankResponse> Banks =>
        new LocalEntityContextInternal<BankPersisted, Bank, IBankPublicQuery,
            BankResponse, BankResponse>(
            _sharedContext,
            new BankPost(
                _sharedContext.DbService.GetDbEntityMethodsClass<BankPersisted>(),
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                _sharedContext.SoftwareStatementProfileCachedRepo,
                _sharedContext.Instrumentation,
                _sharedContext.BankProfileService,
                new OpenIdConfigurationRead(_sharedContext.ApiClient)));

    public IBankRegistrationsContext
        BankRegistrations => new BankRegistrationsContextInternal(_sharedContext);

    public ILocalEntityContext<ObWacCertificate, IObWacCertificatePublicQuery, ObWacCertificateResponse,
        ObWacCertificateResponse> ObWacCertificates =>
        new LocalEntityContextInternal<ObWacCertificateEntity, ObWacCertificate, IObWacCertificatePublicQuery,
            ObWacCertificateResponse, ObWacCertificateResponse>(
            _sharedContext,
            new ObWacCertificatePost(
                _sharedContext.DbService.GetDbEntityMethodsClass<ObWacCertificateEntity>(),
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                _sharedContext.SoftwareStatementProfileCachedRepo,
                _sharedContext.Instrumentation));

    public ILocalEntityContext<ObSealCertificate, IObSealCertificatePublicQuery, ObSealCertificateResponse,
        ObSealCertificateResponse> ObSealCertificates =>
        new LocalEntityContextInternal<ObSealCertificateEntity, ObSealCertificate, IObSealCertificatePublicQuery,
            ObSealCertificateResponse, ObSealCertificateResponse>(
            _sharedContext,
            new ObSealCertificatePost(
                _sharedContext.DbService.GetDbEntityMethodsClass<ObSealCertificateEntity>(),
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                _sharedContext.SoftwareStatementProfileCachedRepo,
                _sharedContext.Instrumentation));

    public ILocalEntityContext<SoftwareStatement, ISoftwareStatementPublicQuery, SoftwareStatementResponse,
        SoftwareStatementResponse> SoftwareStatements =>
        new LocalEntityContextInternal<SoftwareStatementEntity, SoftwareStatement, ISoftwareStatementPublicQuery,
            SoftwareStatementResponse, SoftwareStatementResponse>(
            _sharedContext,
            new SoftwareStatementPost(
                _sharedContext.DbService.GetDbEntityMethodsClass<SoftwareStatementEntity>(),
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                _sharedContext.SoftwareStatementProfileCachedRepo,
                _sharedContext.Instrumentation));

    public ILocalEntityContext<AccountAndTransactionApiRequest, IAccountAndTransactionApiQuery,
        AccountAndTransactionApiResponse, AccountAndTransactionApiResponse> AccountAndTransactionApis =>
        new LocalEntityContextInternal<AccountAndTransactionApiEntity, AccountAndTransactionApiRequest,
            IAccountAndTransactionApiQuery,
            AccountAndTransactionApiResponse, AccountAndTransactionApiResponse>(
            _sharedContext,
            new AccountAndTransactionApiPost(
                _sharedContext.DbService.GetDbEntityMethodsClass<AccountAndTransactionApiEntity>(),
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                _sharedContext.SoftwareStatementProfileCachedRepo,
                _sharedContext.Instrumentation,
                _sharedContext.BankProfileService));

    public ILocalEntityContext<PaymentInitiationApiRequest, IPaymentInitiationApiQuery, PaymentInitiationApiResponse
        , PaymentInitiationApiResponse> PaymentInitiationApis =>
        new LocalEntityContextInternal<PaymentInitiationApiEntity, PaymentInitiationApiRequest,
            IPaymentInitiationApiQuery,
            PaymentInitiationApiResponse, PaymentInitiationApiResponse>(
            _sharedContext,
            new PaymentInitiationApiPost(
                _sharedContext.DbService.GetDbEntityMethodsClass<PaymentInitiationApiEntity>(),
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                _sharedContext.SoftwareStatementProfileCachedRepo,
                _sharedContext.Instrumentation,
                _sharedContext.BankProfileService));

    public ILocalEntityContext<VariableRecurringPaymentsApiRequest, IVariableRecurringPaymentsApiQuery,
        VariableRecurringPaymentsApiResponse, VariableRecurringPaymentsApiResponse> VariableRecurringPaymentsApis
        =>
            new LocalEntityContextInternal<VariableRecurringPaymentsApiEntity, VariableRecurringPaymentsApiRequest,
                IVariableRecurringPaymentsApiQuery,
                VariableRecurringPaymentsApiResponse, VariableRecurringPaymentsApiResponse>(
                _sharedContext,
                new VariableRecurringPaymentsApiPost(
                    _sharedContext.DbService.GetDbEntityMethodsClass<VariableRecurringPaymentsApiEntity>(),
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.Instrumentation,
                    _sharedContext.BankProfileService));
}
