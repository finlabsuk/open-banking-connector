// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests;

using OpenBankingConsent = (ConsentType consentType, Guid consentId);

/// <summary>
///     Class that provides redirects to bank tests.
/// </summary>
public class RedirectObservable
{
    // Lazy singleton
    private static readonly Lazy<RedirectObservable> Singleton = new(
        () =>
        {
            var redirectObservable = new RedirectObservable();
            TestingMethods.Instance.RegisterCreateAccountAccessConsentAuthContext(
                redirectObservable.CreateAccountAccessConsentAuthContext);
            TestingMethods.Instance.RegisterCreateDomesticPaymentConsentAuthContext(
                redirectObservable.CreateDomesticPaymentConsentAuthContext);
            TestingMethods.Instance.RegisterCreateDomesticVrpConsentAuthContext(
                redirectObservable.CreateDomesticVrpConsentAuthContext);
            TestingMethods.Instance.RegisterProcessRedirect(redirectObservable.ProcessRedirect);
            return redirectObservable;
        });

    // Observers
    private readonly ConcurrentDictionary<OpenBankingConsent, RedirectObserver> _observers = new();

    // Public reference to singleton
    public static RedirectObservable Instance => Singleton.Value;

    public IDisposable Subscribe(RedirectObserver observer)
    {
        OpenBankingConsent consent = (observer.ConsentType, observer.ConsentId);

        if (!_observers.TryAdd(consent, observer))
        {
            throw new ArgumentException("The observer is already registered.");
        }

        return new Unsubscriber(_observers, consent);
    }

    private async Task<AccountAccessConsentAuthContextCreateResponse> CreateAccountAccessConsentAuthContext(
        Guid consentId)
    {
        if (!_observers.TryGetValue((ConsentType.AccountAccessConsent, consentId), out RedirectObserver? observer))
        {
            throw new KeyNotFoundException(
                $"The redirect observer could not be found for consent of type {ConsentType.AccountAccessConsent} and ID {consentId}.");
        }

        AccountAccessConsentAuthContextCreateResponse response =
            await observer.CreateAccountAccessConsentAuthContext();

        return response;
    }

    private async Task<DomesticPaymentConsentAuthContextCreateResponse> CreateDomesticPaymentConsentAuthContext(
        Guid consentId)
    {
        if (!_observers.TryGetValue((ConsentType.DomesticPaymentConsent, consentId), out RedirectObserver? observer))
        {
            throw new KeyNotFoundException(
                $"The redirect observer could not be found for consent of type {ConsentType.DomesticPaymentConsent} and ID {consentId}.");
        }

        DomesticPaymentConsentAuthContextCreateResponse response =
            await observer.CreateDomesticPaymentConsentAuthContext();

        return response;
    }

    private async Task<DomesticVrpConsentAuthContextCreateResponse> CreateDomesticVrpConsentAuthContext(
        Guid consentId)
    {
        if (!_observers.TryGetValue((ConsentType.DomesticVrpConsent, consentId), out RedirectObserver? observer))
        {
            throw new KeyNotFoundException(
                $"The redirect observer could not be found for consent of type {ConsentType.DomesticVrpConsent} and ID {consentId}.");
        }

        DomesticVrpConsentAuthContextCreateResponse response =
            await observer.CreateDomesticVrpConsentAuthContext();

        return response;
    }


    private async Task<AuthContextUpdateAuthResultResponse> ProcessRedirect(TestingAuthResult authResult)
    {
        RedirectObserver observer =
            _observers.Values.SingleOrDefault(x => x.AssociatedStates.Contains(authResult.State)) ??
            throw new KeyNotFoundException("The redirect observer could not be found.");

        return await observer.ProcessRedirect(authResult);
    }

    private class Unsubscriber(
        ConcurrentDictionary<OpenBankingConsent, RedirectObserver> observers,
        OpenBankingConsent consent)
        : IDisposable
    {
        public void Dispose() => observers.TryRemove(consent, out _);
    }
}
