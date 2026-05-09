// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

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

    public async Task<DomesticVrpConsentCreateResponse> DomesticVrpConsentCreate(
        DomesticVrpConsentRequest request,
        DomesticVrpConsentCustomBehaviour? customBehaviour)
    {
        // Create object
        var uriPath = "/vrp/domestic-vrp-consents";
        DomesticVrpConsentCreateResponse response =
            await client.CreateAsync<DomesticVrpConsentCreateResponse, DomesticVrpConsentRequest>(
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

    public async Task<DomesticVrpConsentFundsConfirmationResponse> DomesticVrpConsentCreateFundsConfirmation(
        VrpConsentFundsConfirmationCreateParams createParams,
        DomesticVrpConsentCustomBehaviour? customBehaviour)
    {
        // Create object
        var uriPath = $"/vrp/domestic-vrp-consents/{createParams.ConsentId}/funds-confirmation";
        DomesticVrpConsentFundsConfirmationResponse response =
            await client
                .CreateAsync<DomesticVrpConsentFundsConfirmationResponse, DomesticVrpConsentFundsConfirmationRequest>(
                    uriPath,
                    createParams.Request);

        // Checks
        Assert.IsNull(response.Warnings);
        Assert.IsNotNull(response.ExternalApiResponse);

        // Check funds available
        bool responseDataFundsAvailableResultFundsAvailableMayBeWrong =
            customBehaviour?.ResponseDataFundsAvailableResultFundsAvailableMayBeWrong ?? false;
        if (!responseDataFundsAvailableResultFundsAvailableMayBeWrong)
        {
            Assert.AreEqual(
                VariableRecurringPaymentsModelsPublic.OBPAFundsAvailableResult1FundsAvailable.Available,
                response.ExternalApiResponse.Data.FundsAvailableResult.FundsAvailable);
        }
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
        Assert.IsNull(response.Warnings);

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
        Assert.IsNull(response.Warnings);
        Assert.IsNotNull(response.AuthUrl);

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
        Assert.IsNull(response.Warnings);

        return response;
    }

    public async Task<DomesticVrpResponse> DomesticVrpRead(
        ExternalEntityReadParams readParams,
        DomesticVrpCustomBehaviour? customBehaviour)
    {
        // Read object
        var uriPath = $"/vrp/domestic-vrps/{readParams.ExternalApiId}";
        var response =
            await client.GetAsync<DomesticVrpResponse>(
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
        bool responseDataStatusMayBeWrong = customBehaviour?.ResponseDataStatusMayBeMissingOrWrong ?? false;
        if (!responseDataStatusMayBeWrong)
        {
            CollectionAssert.Contains(
                new[]
                {
                    VariableRecurringPaymentsModelsPublic.Data4Status.ACCC,
                    VariableRecurringPaymentsModelsPublic.Data4Status.ACCP,
                    VariableRecurringPaymentsModelsPublic.Data4Status.ACFC,
                    VariableRecurringPaymentsModelsPublic.Data4Status.ACSC,
                    VariableRecurringPaymentsModelsPublic.Data4Status.ACSP,
                    VariableRecurringPaymentsModelsPublic.Data4Status.ACTC,
                    VariableRecurringPaymentsModelsPublic.Data4Status.ACWC,
                    VariableRecurringPaymentsModelsPublic.Data4Status.ACWP,
                    VariableRecurringPaymentsModelsPublic.Data4Status.PDNG
                },
                response.ExternalApiResponse.Data.Status);
        }

        // Check refund account
        bool responseDataRefundMayBeMissingOrWrong = customBehaviour?.ResponseDataRefundMayBeMissingOrWrong ?? false;
        if (!responseDataRefundMayBeMissingOrWrong)
        {
            Assert.IsNotNull(response.ExternalApiResponse.Data.Refund);

            bool responseDataRefundSchemeNameMayBeWrong =
                customBehaviour?.ResponseDataRefundSchemeNameMayBeWrong ?? false;
            if (!responseDataRefundSchemeNameMayBeWrong)
            {
                Assert.AreEqual(
                    "UK.OBIE.SortCodeAccountNumber",
                    response.ExternalApiResponse.Data.Refund!.SchemeName);
            }

            bool responseDataRefundIdentificationMayBeWrong =
                customBehaviour?.ResponseDataRefundIdentificationMayBeWrong ?? false;
            if (!responseDataRefundIdentificationMayBeWrong)
            {
                Assert.AreEqual(14, response.ExternalApiResponse.Data.Refund!.Identification.Length);
            }
        }

        // Check debtor account
        bool responseDataDebtorAccountMayBeMissingOrWrong =
            customBehaviour?.ResponseDataDebtorAccountMayBeMissingOrWrong ?? false;
        if (!responseDataDebtorAccountMayBeMissingOrWrong)
        {
            Assert.IsNotNull(response.ExternalApiResponse.Data.DebtorAccount);

            bool responseDataDebtorAccountSchemeNameMayBeWrong =
                customBehaviour?.ResponseDataDebtorAccountSchemeNameMayBeWrong ?? false;
            if (!responseDataDebtorAccountSchemeNameMayBeWrong)
            {
                Assert.AreEqual(
                    "UK.OBIE.SortCodeAccountNumber",
                    response.ExternalApiResponse.Data.DebtorAccount!.SchemeName);
            }

            bool responseDataDebtorAccountIdentificationMayBeWrong =
                customBehaviour?.ResponseDataDebtorAccountIdentificationMayBeWrong ?? false;
            if (!responseDataDebtorAccountIdentificationMayBeWrong)
            {
                Assert.AreEqual(14, response.ExternalApiResponse.Data.DebtorAccount!.Identification.Length);
            }
        }

        return response;
    }

    public async Task<DomesticVrpPaymentDetailsResponse> DomesticVrpReadPaymentDetails(
        ExternalEntityReadParams readParams)
    {
        // Read object
        var uriPath = $"/vrp/domestic-vrps/{readParams.ExternalApiId}/payment-details";
        var response =
            await client.GetAsync<DomesticVrpPaymentDetailsResponse>(
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

    public async Task<DomesticVrpResponse> DomesticVrpCreate(
        DomesticVrpRequest request,
        ConsentExternalCreateParams createParams,
        DomesticVrpCustomBehaviour? customBehaviour)
    {
        // Create object
        var uriPath = "/vrp/domestic-vrps";
        DomesticVrpResponse response =
            await client.CreateAsync<DomesticVrpResponse, DomesticVrpRequest>(
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

            bool responseDataRefundSchemeNameMayBeWrong =
                customBehaviour?.ResponseDataRefundSchemeNameMayBeWrong ?? false;
            if (!responseDataRefundSchemeNameMayBeWrong)
            {
                Assert.AreEqual(
                    "UK.OBIE.SortCodeAccountNumber",
                    response.ExternalApiResponse.Data.Refund!.SchemeName);
            }

            bool responseDataRefundIdentificationMayBeWrong =
                customBehaviour?.ResponseDataRefundIdentificationMayBeWrong ?? false;
            if (!responseDataRefundIdentificationMayBeWrong)
            {
                Assert.AreEqual(14, response.ExternalApiResponse.Data.Refund!.Identification.Length);
            }
        }

        // Check debtor account
        bool responseDataDebtorAccountMayBeMissingOrWrong =
            customBehaviour?.ResponseDataDebtorAccountMayBeMissingOrWrong ?? false;
        if (!responseDataDebtorAccountMayBeMissingOrWrong)
        {
            Assert.IsNotNull(response.ExternalApiResponse.Data.DebtorAccount);

            bool responseDataDebtorAccountSchemeNameMayBeWrong =
                customBehaviour?.ResponseDataDebtorAccountSchemeNameMayBeWrong ?? false;
            if (!responseDataDebtorAccountSchemeNameMayBeWrong)
            {
                Assert.AreEqual(
                    "UK.OBIE.SortCodeAccountNumber",
                    response.ExternalApiResponse.Data.DebtorAccount!.SchemeName);
            }

            bool responseDataDebtorAccountIdentificationMayBeWrong =
                customBehaviour?.ResponseDataDebtorAccountIdentificationMayBeWrong ?? false;
            if (!responseDataDebtorAccountIdentificationMayBeWrong)
            {
                Assert.AreEqual(14, response.ExternalApiResponse.Data.DebtorAccount!.Identification.Length);
            }
        }

        return response;
    }
}
