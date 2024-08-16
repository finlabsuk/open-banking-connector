// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class VariableRecurringPaymentsApiClient(WebAppClient client)
{
    public async Task<DomesticVrpConsentCreateResponse> DomesticVrpConsentRead(ConsentReadParams readParams)
    {
        // Read object
        var uriPath = $"/vrp/domestic-vrp-consents/{readParams.Id}";
        var response =
            await client.GetAsync<DomesticVrpConsentCreateResponse>(
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

    public async Task<DomesticVrpConsentCreateResponse> DomesticVrpConsentCreate(
        DomesticVrpConsentRequest request)
    {
        // Create object
        var uriPath = "/vrp/domestic-vrp-consents";
        DomesticVrpConsentCreateResponse response =
            await client.CreateAsync<DomesticVrpConsentCreateResponse, DomesticVrpConsentRequest>(
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

    public async Task<DomesticVrpConsentFundsConfirmationResponse> DomesticVrpConsentCreateFundsConfirmation(
        VrpConsentFundsConfirmationCreateParams createParams)
    {
        // Create object
        var uriPath = $"/vrp/domestic-vrp-consents/{createParams.ConsentId}/funds-confirmation";
        DomesticVrpConsentFundsConfirmationResponse response =
            await client
                .CreateAsync<DomesticVrpConsentFundsConfirmationResponse, DomesticVrpConsentFundsConfirmationRequest>(
                    uriPath,
                    createParams.Request);

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    public async Task<BaseResponse> DomesticVrpConsentDelete(ConsentDeleteParams deleteParams)
    {
        // Delete object
        var uriPath = $"/vrp/domestic-vrp-consents/{deleteParams.Id}";
        var response =
            await client.DeleteAsync<BaseResponse>(
                uriPath,
                deleteParams.ExcludeExternalApiOperation
                    ? [new KeyValuePair<string, IEnumerable<string>>("x-obc-exclude-external-api-operation", ["true"])]
                    : []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<DomesticVrpConsentAuthContextCreateResponse> DomesticVrpConsentAuthContextCreate(
        DomesticVrpConsentAuthContext request)
    {
        // Create object
        var uriPath = "/vrp/domestic-vrp-consent-auth-contexts";
        DomesticVrpConsentAuthContextCreateResponse response =
            await client
                .CreateAsync<DomesticVrpConsentAuthContextCreateResponse, DomesticVrpConsentAuthContext>(
                    uriPath,
                    request);

        // Checks
        response.Warnings.Should().BeNull();
        response.AuthUrl.Should().NotBeNull();

        return response;
    }

    public async Task<DomesticVrpConsentAuthContextReadResponse> DomesticVrpConsentAuthContextRead(
        LocalReadParams readParams)
    {
        // Read object
        var uriPath = $"/vrp/domestic-vrp-consent-auth-contexts/{readParams.Id}";
        var response =
            await client.GetAsync<DomesticVrpConsentAuthContextReadResponse>(uriPath, []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<DomesticVrpResponse> DomesticVrpRead(ConsentExternalEntityReadParams readParams)
    {
        // Read object
        var uriPath = $"/vrp/domestic-vrps/{readParams.ExternalApiId}";
        var response =
            await client.GetAsync<DomesticVrpResponse>(
                uriPath,
                [
                    new KeyValuePair<string, IEnumerable<string>>(
                        "x-obc-domestic-vrp-consent-id",
                        [$"{readParams.ConsentId}"])
                ]);

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    public async Task<DomesticVrpPaymentDetailsResponse> DomesticVrpReadPaymentDetails(
        ConsentExternalEntityReadParams readParams)
    {
        // Read object
        var uriPath = $"/vrp/domestic-vrps/{readParams.ExternalApiId}/payment-details";
        var response =
            await client.GetAsync<DomesticVrpPaymentDetailsResponse>(
                uriPath,
                [
                    new KeyValuePair<string, IEnumerable<string>>(
                        "x-obc-domestic-vrp-consent-id",
                        [$"{readParams.ConsentId}"])
                ]);

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    public async Task<DomesticVrpResponse> DomesticVrpCreate(
        DomesticVrpRequest request,
        ConsentExternalCreateParams createParams)
    {
        // Create object
        var uriPath = "/vrp/domestic-vrps";
        DomesticVrpResponse response =
            await client.CreateAsync<DomesticVrpResponse, DomesticVrpRequest>(
                uriPath,
                request);

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }
}
