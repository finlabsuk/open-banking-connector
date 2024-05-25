// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class AccountAndTransactionApiClient(WebAppClient client)
{
    public async Task<AccountAccessConsentCreateResponse> AccountAccessConsentRead(ConsentReadParams readParams)
    {
        // Read object
        var uriPath = $"/aisp/account-access-consents/{readParams.Id}";
        var response =
            await client.GetAsync<AccountAccessConsentCreateResponse>(
                uriPath,
                readParams.ExcludeExternalApiOperation
                    ? [new KeyValuePair<string, IEnumerable<string>>("x-obc-exclude-external-api-operation", ["true"])]
                    : []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<AccountAccessConsentCreateResponse> AccountAccessConsentCreate(
        AccountAccessConsentRequest request)
    {
        // Create object
        var uriPath = "/aisp/account-access-consents";
        AccountAccessConsentCreateResponse response =
            await client.CreateAsync<AccountAccessConsentCreateResponse, AccountAccessConsentRequest>(uriPath, request);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<BaseResponse> AccountAccessConsentDelete(ConsentDeleteParams deleteParams)
    {
        // Delete object
        var uriPath = $"/aisp/account-access-consents/{deleteParams.Id}";
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

    public async Task<AccountAccessConsentAuthContextCreateResponse> AccountAccessConsentAuthContextCreate(
        AccountAccessConsentAuthContext request)
    {
        // Create object
        var uriPath = "/aisp/account-access-consent-auth-contexts";
        AccountAccessConsentAuthContextCreateResponse response =
            await client.CreateAsync<AccountAccessConsentAuthContextCreateResponse, AccountAccessConsentAuthContext>(
                uriPath,
                request);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<AccountAccessConsentAuthContextReadResponse> AccountAccessConsentAuthContextRead(
        LocalReadParams readParams)
    {
        // Read object
        var uriPath = $"/aisp/account-access-consent-auth-contexts/{readParams.Id}";
        var response =
            await client.GetAsync<AccountAccessConsentAuthContextReadResponse>(uriPath, []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }
}
