// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web;

public class TestingMethods
{
    private static readonly Lazy<TestingMethods> Singleton = new(() => new TestingMethods());

    public static TestingMethods Instance => Singleton.Value;

    public Func<TestingAuthResult, Task<AuthContextUpdateAuthResultResponse>>? ProcessRedirect { get; private set; }

    public Func<Guid, Task<AccountAccessConsentAuthContextCreateResponse>>? CreateAccountAccessConsentAuthContext
    {
        get;
        private set;
    }

    public Func<Guid, Task<DomesticPaymentConsentAuthContextCreateResponse>>? CreateDomesticPaymentConsentAuthContext
    {
        get;
        private set;
    }

    public Func<Guid, Task<DomesticVrpConsentAuthContextCreateResponse>>? CreateDomesticVrpConsentAuthContext
    {
        get;
        private set;
    }

    public void RegisterCreateAccountAccessConsentAuthContext(
        Func<Guid, Task<AccountAccessConsentAuthContextCreateResponse>> createAccountAccessConsentAuthContext)
    {
        CreateAccountAccessConsentAuthContext = createAccountAccessConsentAuthContext;
    }

    public void RegisterCreateDomesticPaymentConsentAuthContext(
        Func<Guid, Task<DomesticPaymentConsentAuthContextCreateResponse>> createDomesticPaymentConsentAuthContext)
    {
        CreateDomesticPaymentConsentAuthContext = createDomesticPaymentConsentAuthContext;
    }

    public void RegisterCreateDomesticVrpConsentAuthContext(
        Func<Guid, Task<DomesticVrpConsentAuthContextCreateResponse>> createDomesticVrpConsentAuthContext)
    {
        CreateDomesticVrpConsentAuthContext = createDomesticVrpConsentAuthContext;
    }


    public void RegisterProcessRedirect(
        Func<TestingAuthResult, Task<AuthContextUpdateAuthResultResponse>> processRedirect)
    {
        ProcessRedirect = processRedirect;
    }
}
