// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
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
        if (readParams.ExcludeExternalApiOperation)
        {
            response.ExternalApiResponse.Should().BeNull();
        }
        else
        {
            response.ExternalApiResponse.Should().NotBeNull();
        }

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
        response.ExternalApiResponse.Should().NotBeNull();

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
        if (request.ExternalApiObject is not null)
        {
            response.ExternalApiResponse.Should().BeNull();
        }
        else
        {
            response.ExternalApiResponse.Should().NotBeNull();
        }

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
        response.AuthUrl.Should().NotBeNull();

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

    public async Task<DomesticPaymentResponse> DomesticPaymentRead(ConsentExternalEntityReadParams readParams)
    {
        // Read object
        var uriPath = $"/pisp/domestic-payments/{readParams.ExternalApiId}";
        var response =
            await client.GetAsync<DomesticPaymentResponse>(
                uriPath,
                [
                    new KeyValuePair<string, IEnumerable<string>>(
                        "x-obc-domestic-payment-consent-id",
                        [$"{readParams.ConsentId}"])
                ]);

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    public async Task<DomesticPaymentPaymentDetailsResponse> DomesticPaymentReadPaymentDetails(
        ConsentExternalEntityReadParams readParams)
    {
        // Read object
        var uriPath = $"/pisp/domestic-payments/{readParams.ExternalApiId}/payment-details";
        var response =
            await client.GetAsync<DomesticPaymentPaymentDetailsResponse>(
                uriPath,
                [
                    new KeyValuePair<string, IEnumerable<string>>(
                        "x-obc-domestic-payment-consent-id",
                        [$"{readParams.ConsentId}"])
                ]);

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    public async Task<DomesticPaymentResponse> DomesticPaymentCreate(
        DomesticPaymentRequest request,
        ConsentExternalCreateParams createParams)
    {
        // Create object
        var uriPath = "/pisp/domestic-payments";
        DomesticPaymentResponse response =
            await client.CreateAsync<DomesticPaymentResponse, DomesticPaymentRequest>(
                uriPath,
                request);

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }
}
