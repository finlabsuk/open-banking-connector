// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class PaymentInitiationApiClient(WebAppClient client)
{
    public async Task<DomesticPaymentConsentCreateResponse> DomesticPaymentConsentRead(ConsentReadParams readParams)
    {
        // Read object
        var uriPath = $"/pisp/domestic-payment-consents/{readParams.Id}";
        var response =
            await client.GetAsync<DomesticPaymentConsentCreateResponse>(
                uriPath,
                readParams.ExcludeExternalApiOperation
                    ? [new KeyValuePair<string, IEnumerable<string>>("x-obc-exclude-external-api-operation", ["true"])]
                    : []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<DomesticPaymentConsentFundsConfirmationResponse> DomesticPaymentConsentReadFundsConfirmation(
        ConsentBaseReadParams readParams)
    {
        // Read object
        var uriPath = $"/pisp/domestic-payment-consents/{readParams.Id}/funds-confirmation";
        var response =
            await client.GetAsync<DomesticPaymentConsentFundsConfirmationResponse>(
                uriPath,
                []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<DomesticPaymentConsentCreateResponse> DomesticPaymentConsentCreate(
        DomesticPaymentConsentRequest request)
    {
        // Create object
        var uriPath = "/pisp/domestic-payment-consents";
        DomesticPaymentConsentCreateResponse response =
            await client.CreateAsync<DomesticPaymentConsentCreateResponse, DomesticPaymentConsentRequest>(
                uriPath,
                request);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<BaseResponse> DomesticPaymentConsentDelete(LocalDeleteParams deleteParams)
    {
        // Delete object
        var uriPath = $"/pisp/domestic-payment-consents/{deleteParams.Id}";
        var response =
            await client.DeleteAsync<BaseResponse>(
                uriPath,
                []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<DomesticPaymentConsentAuthContextCreateResponse> DomesticPaymentConsentAuthContextCreate(
        DomesticPaymentConsentAuthContext request)
    {
        // Create object
        var uriPath = "/pisp/domestic-payment-consent-auth-contexts";
        DomesticPaymentConsentAuthContextCreateResponse response =
            await client
                .CreateAsync<DomesticPaymentConsentAuthContextCreateResponse, DomesticPaymentConsentAuthContext>(
                    uriPath,
                    request);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<DomesticPaymentConsentAuthContextReadResponse> DomesticPaymentConsentAuthContextRead(
        LocalReadParams readParams)
    {
        // Read object
        var uriPath = $"/pisp/domestic-payment-consent-auth-contexts/{readParams.Id}";
        var response =
            await client.GetAsync<DomesticPaymentConsentAuthContextReadResponse>(uriPath, []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }
}
