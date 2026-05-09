// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

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
        Assert.IsNull(response.Warnings);
        if (readParams.ExcludeExternalApiOperation)
        {
            Assert.IsNull(response.ExternalApiResponse);
        }
        else
        {
            Assert.IsNotNull(response.ExternalApiResponse);
        }

        return response;
    }

    public async Task<DomesticPaymentConsentFundsConfirmationResponse> DomesticPaymentConsentReadFundsConfirmation(
        ConsentBaseReadParams readParams,
        DomesticPaymentConsentCustomBehaviour? customBehaviour)
    {
        // Read object
        var uriPath = $"/pisp/domestic-payment-consents/{readParams.Id}/funds-confirmation";
        var response =
            await client.GetAsync<DomesticPaymentConsentFundsConfirmationResponse>(
                uriPath,
                []);

        // Checks
        Assert.IsNull(response.Warnings);
        Assert.IsNotNull(response.ExternalApiResponse);
        Assert.IsNotNull(response.ExternalApiResponse.Data.FundsAvailableResult);

        // Check funds available
        bool responseDataFundsAvailableResultFundsAvailableMayBeWrong =
            customBehaviour?.ResponseDataFundsAvailableResultFundsAvailableMayBeWrong ?? false;
        if (!responseDataFundsAvailableResultFundsAvailableMayBeWrong)
        {
            Assert.IsTrue(response.ExternalApiResponse.Data.FundsAvailableResult!.FundsAvailable);
        }

        return response;
    }

    public async Task<DomesticPaymentConsentCreateResponse> DomesticPaymentConsentCreate(
        DomesticPaymentConsentRequest request,
        DomesticPaymentConsentCustomBehaviour? customBehaviour)
    {
        // Create object
        var uriPath = "/pisp/domestic-payment-consents";
        DomesticPaymentConsentCreateResponse response =
            await client.CreateAsync<DomesticPaymentConsentCreateResponse, DomesticPaymentConsentRequest>(
                uriPath,
                request);

        // Checks
        Assert.IsNull(response.Warnings);
        if (request.ExternalApiObject is not null)
        {
            Assert.IsNull(response.ExternalApiResponse);
        }
        else
        {
            Assert.IsNotNull(response.ExternalApiResponse);
            Assert.AreEqual(
                request.ExternalApiRequest!.Risk.PaymentContextCode,
                response.ExternalApiResponse!.Risk.PaymentContextCode);
            Assert.AreEqual(
                request.ExternalApiRequest!.Risk.V3PaymentContextCode,
                response.ExternalApiResponse!.Risk.V3PaymentContextCode);
            bool responseRiskContractPresentIndicatorMayBeMissingOrWrong =
                customBehaviour?.ResponseRiskContractPresentIndicatorMayBeMissingOrWrong ?? false;
            if (!responseRiskContractPresentIndicatorMayBeMissingOrWrong)
            {
                Assert.AreEqual(
                    request.ExternalApiRequest!.Risk.ContractPresentIndicator,
                    response.ExternalApiResponse!.Risk.ContractPresentIndicator);
            }
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
        Assert.IsNull(response.Warnings);

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
        Assert.IsNull(response.Warnings);
        Assert.IsNotNull(response.AuthUrl);

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
        Assert.IsNull(response.Warnings);

        return response;
    }

    public async Task<DomesticPaymentResponse> DomesticPaymentRead(
        ExternalEntityReadParams readParams,
        DomesticPaymentCustomBehaviour? customBehaviour)
    {
        // Read object
        var uriPath = $"/pisp/domestic-payments/{readParams.ExternalApiId}";
        var response =
            await client.GetAsync<DomesticPaymentResponse>(
                uriPath,
                [
                    new KeyValuePair<string, IEnumerable<string>>(
                        "x-obc-bank-registration-id",
                        [$"{readParams.BankRegistrationId}"])
                ]);

        // Checks
        Assert.IsNull(response.Warnings);
        Assert.IsNotNull(response.ExternalApiResponse);

        // Check status
        bool responseDataStatusMayBeWrong = customBehaviour?.ResponseDataStatusMayBeWrong ?? false;
        if (!responseDataStatusMayBeWrong)
        {
            CollectionAssert.Contains(
                new[]
                {
                    PaymentInitiationModelsPublic.Data4Status.ACCC, PaymentInitiationModelsPublic.Data4Status.ACCP,
                    PaymentInitiationModelsPublic.Data4Status.ACFC, PaymentInitiationModelsPublic.Data4Status.ACSC,
                    PaymentInitiationModelsPublic.Data4Status.ACSP, PaymentInitiationModelsPublic.Data4Status.ACTC,
                    PaymentInitiationModelsPublic.Data4Status.ACWC, PaymentInitiationModelsPublic.Data4Status.ACWP,
                    PaymentInitiationModelsPublic.Data4Status.PDNG
                },
                response.ExternalApiResponse.Data.Status);
        }

        // Check refund account
        bool responseDataRefundMayBeMissingOrWrong = customBehaviour?.ResponseDataRefundMayBeMissingOrWrong ?? false;
        if (!responseDataRefundMayBeMissingOrWrong)
        {
            Assert.IsNotNull(response.ExternalApiResponse.Data.Refund);

            bool responseDataRefundAccountSchemeNameMayBeMissingOrWrong =
                customBehaviour?.ResponseDataRefundAccountSchemeNameMayBeMissingOrWrong ?? false;
            if (!responseDataRefundAccountSchemeNameMayBeMissingOrWrong)
            {
                Assert.AreEqual(
                    "UK.OBIE.SortCodeAccountNumber",
                    response.ExternalApiResponse.Data.Refund!.Account.SchemeName);
            }

            bool responseDataRefundAccountIdentificationMayBeMissingOrWrong =
                customBehaviour?.ResponseDataRefundAccountIdentificationMayBeMissingOrWrong ?? false;
            if (!responseDataRefundAccountIdentificationMayBeMissingOrWrong)
            {
                Assert.AreEqual(14, response.ExternalApiResponse.Data.Refund!.Account.Identification.Length);
            }
        }

        // Check debtor account
        bool responseDataDebtorMayBeMissingOrWrong = customBehaviour?.ResponseDataDebtorMayBeMissingOrWrong ?? false;
        if (!responseDataDebtorMayBeMissingOrWrong)
        {
            Assert.IsNotNull(response.ExternalApiResponse.Data.Debtor);

            bool responseDataDebtorSchemeNameMayBeMissingOrWrong =
                customBehaviour?.ResponseDataDebtorSchemeNameMayBeMissingOrWrong ?? false;
            if (!responseDataDebtorSchemeNameMayBeMissingOrWrong)
            {
                Assert.AreEqual(
                    "UK.OBIE.SortCodeAccountNumber",
                    response.ExternalApiResponse.Data.Debtor!.SchemeName);
            }

            bool responseDataDebtorIdentificationMayBeMissingOrWrong =
                customBehaviour?.ResponseDataDebtorIdentificationMayBeMissingOrWrong ?? false;
            if (!responseDataDebtorIdentificationMayBeMissingOrWrong)
            {
                Assert.AreEqual(14, response.ExternalApiResponse.Data.Debtor!.Identification!.Length);
            }
        }

        return response;
    }

    public async Task<DomesticPaymentPaymentDetailsResponse> DomesticPaymentReadPaymentDetails(
        ExternalEntityReadParams readParams)
    {
        // Read object
        var uriPath = $"/pisp/domestic-payments/{readParams.ExternalApiId}/payment-details";
        var response =
            await client.GetAsync<DomesticPaymentPaymentDetailsResponse>(
                uriPath,
                [
                    new KeyValuePair<string, IEnumerable<string>>(
                        "x-obc-bank-registration-id",
                        [$"{readParams.BankRegistrationId}"])
                ]);

        // Checks
        Assert.IsNull(response.Warnings);
        Assert.IsNotNull(response.ExternalApiResponse);

        return response;
    }

    public async Task<DomesticPaymentResponse> DomesticPaymentCreate(
        DomesticPaymentRequest request,
        ConsentExternalCreateParams createParams,
        DomesticPaymentCustomBehaviour? customBehaviour)
    {
        // Create object
        var uriPath = "/pisp/domestic-payments";
        DomesticPaymentResponse response =
            await client.CreateAsync<DomesticPaymentResponse, DomesticPaymentRequest>(
                uriPath,
                request);

        // Checks
        Assert.IsNull(response.Warnings);
        Assert.IsNotNull(response.ExternalApiResponse);

        // Check refund account
        bool responseDataRefundMayBeMissingOrWrong = customBehaviour?.ResponseDataRefundMayBeMissingOrWrong ?? false;
        if (!responseDataRefundMayBeMissingOrWrong)
        {
            Assert.IsNotNull(response.ExternalApiResponse.Data.Refund);

            bool responseDataRefundAccountSchemeNameMayBeMissingOrWrong =
                customBehaviour?.ResponseDataRefundAccountSchemeNameMayBeMissingOrWrong ?? false;
            if (!responseDataRefundAccountSchemeNameMayBeMissingOrWrong)
            {
                Assert.AreEqual(
                    "UK.OBIE.SortCodeAccountNumber",
                    response.ExternalApiResponse.Data.Refund!.Account.SchemeName);
            }

            bool responseDataRefundAccountIdentificationMayBeMissingOrWrong =
                customBehaviour?.ResponseDataRefundAccountIdentificationMayBeMissingOrWrong ?? false;
            if (!responseDataRefundAccountIdentificationMayBeMissingOrWrong)
            {
                Assert.AreEqual(14, response.ExternalApiResponse.Data.Refund!.Account.Identification.Length);
            }
        }

        // Check debtor account
        bool responseDataDebtorMayBeMissingOrWrong = customBehaviour?.ResponseDataDebtorMayBeMissingOrWrong ?? false;
        if (!responseDataDebtorMayBeMissingOrWrong)
        {
            Assert.IsNotNull(response.ExternalApiResponse.Data.Debtor);

            bool responseDataDebtorSchemeNameMayBeMissingOrWrong =
                customBehaviour?.ResponseDataDebtorSchemeNameMayBeMissingOrWrong ?? false;
            if (!responseDataDebtorSchemeNameMayBeMissingOrWrong)
            {
                Assert.AreEqual(
                    "UK.OBIE.SortCodeAccountNumber",
                    response.ExternalApiResponse.Data.Debtor!.SchemeName);
            }

            bool responseDataDebtorIdentificationMayBeMissingOrWrong =
                customBehaviour?.ResponseDataDebtorIdentificationMayBeMissingOrWrong ?? false;
            if (!responseDataDebtorIdentificationMayBeMissingOrWrong)
            {
                Assert.AreEqual(14, response.ExternalApiResponse.Data.Debtor!.Identification!.Length);
            }
        }

        return response;
    }
}
