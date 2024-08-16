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

/// <summary>
///     Class used by bank tests to consume redirects
/// </summary>
public class RedirectObserver
{
    private readonly TaskCompletionSource<AuthContextUpdateAuthResultResponse> _taskCompletionSource =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public required Func<TestingAuthResult, Task<AuthContextUpdateAuthResultResponse>> ProcessRedirectFcn { get; init; }

    public Func<Task<AccountAccessConsentAuthContextCreateResponse>>? AccountAccessConsentAuthContextCreateFcn
    {
        get;
        init;
    }

    public Func<Task<DomesticPaymentConsentAuthContextCreateResponse>>? DomesticPaymentConsentAuthContextCreateFcn
    {
        get;
        init;
    }

    public Func<Task<DomesticVrpConsentAuthContextCreateResponse>>? DomesticVrpConsentAuthContextCreateFcn
    {
        get;
        init;
    }

    public required Guid ConsentId { get; init; }

    public required ConsentType ConsentType { get; init; }

    public ConcurrentBag<string> AssociatedStates { get; } = new();

    public async Task<AuthContextUpdateAuthResultResponse> ProcessRedirect(TestingAuthResult authResult)
    {
        AuthContextUpdateAuthResultResponse response = await ProcessRedirectFcn(authResult);
        _taskCompletionSource.TrySetResult(response);
        
        return response;
    }

    public async Task<AccountAccessConsentAuthContextCreateResponse> CreateAccountAccessConsentAuthContext()
    {
        if (AccountAccessConsentAuthContextCreateFcn is null)
        {
            throw new InvalidOperationException("Cannot create AccountAccessConsent.");
        }
        AccountAccessConsentAuthContextCreateResponse response = await AccountAccessConsentAuthContextCreateFcn();

        // Register state parameter
        AssociatedStates.Add(response.State);

        return response;
    }

    public async Task<DomesticPaymentConsentAuthContextCreateResponse> CreateDomesticPaymentConsentAuthContext()
    {
        if (DomesticPaymentConsentAuthContextCreateFcn is null)
        {
            throw new InvalidOperationException("Cannot create DomesticPaymentConsent.");
        }
        DomesticPaymentConsentAuthContextCreateResponse response = await DomesticPaymentConsentAuthContextCreateFcn();

        // Register state parameter
        AssociatedStates.Add(response.State);

        return response;
    }

    public async Task<DomesticVrpConsentAuthContextCreateResponse> CreateDomesticVrpConsentAuthContext()
    {
        if (DomesticVrpConsentAuthContextCreateFcn is null)
        {
            throw new InvalidOperationException("Cannot create DomesticVrpConsent.");
        }
        DomesticVrpConsentAuthContextCreateResponse response = await DomesticVrpConsentAuthContextCreateFcn();

        // Register state parameter
        AssociatedStates.Add(response.State);

        return response;
    }

    public async Task<AuthContextUpdateAuthResultResponse> WaitForRedirect(TimeSpan timeout)
    {
        var cts = new CancellationTokenSource(timeout);
        cts.Token.Register(() => _taskCompletionSource.TrySetCanceled(), false);

        try
        {
            return await _taskCompletionSource.Task.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException($"The redirect observer timed out after {timeout.TotalSeconds} seconds.");
        }
    }
}
