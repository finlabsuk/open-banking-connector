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

    public async Task<AccountAccessConsentCreateResponse> AccountAccessConsentCreate(
        AccountAccessConsentRequest request)
    {
        // Create object
        var uriPath = "/aisp/account-access-consents";
        AccountAccessConsentCreateResponse response =
            await client.CreateAsync<AccountAccessConsentCreateResponse, AccountAccessConsentRequest>(uriPath, request);

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
        response.AuthUrl.Should().NotBeNull();

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

    public async Task<AccountsResponse> AccountsRead(AccountAccessConsentExternalReadParams readParams)
    {
        // Read object
        var uriPath = "/aisp/accounts";
        if (readParams.ExternalApiAccountId is not null)
        {
            uriPath += $"/{readParams.ExternalApiAccountId}";
        }
        if (readParams.QueryString is not null)
        {
            uriPath += "?" + readParams.QueryString.TrimStart('?');
        }

        var response =
            await client.GetAsync<AccountsResponse>(
                uriPath,
                GetExtraHeaders(readParams));

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    private static List<KeyValuePair<string, IEnumerable<string>>> GetExtraHeaders(
        AccountAccessConsentExternalReadParams readParams)
    {
        List<KeyValuePair<string, IEnumerable<string>>> extraHeaders =
            readParams.ExtraHeaders?
                .Select(x => new KeyValuePair<string, IEnumerable<string>>(x.Name, [x.Value]))
                .ToList()
            ?? [];
        extraHeaders.Add(
            new KeyValuePair<string, IEnumerable<string>>(
                "x-obc-account-access-consent-id",
                [$"{readParams.ConsentId}"]));
        return extraHeaders;
    }

    public async Task<BalancesResponse> BalancesRead(AccountAccessConsentExternalReadParams readParams)
    {
        // Read object
        var uriPath = "/aisp";
        if (readParams.ExternalApiAccountId is not null)
        {
            uriPath += $"/accounts/{readParams.ExternalApiAccountId}";
        }
        uriPath += "/balances";
        if (readParams.QueryString is not null)
        {
            uriPath += "?" + readParams.QueryString.TrimStart('?');
        }

        var response =
            await client.GetAsync<BalancesResponse>(
                uriPath,
                GetExtraHeaders(readParams));

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    public async Task<DirectDebitsResponse> DirectDebitsRead(AccountAccessConsentExternalReadParams readParams)
    {
        // Read object
        var uriPath = "/aisp";
        if (readParams.ExternalApiAccountId is not null)
        {
            uriPath += $"/accounts/{readParams.ExternalApiAccountId}";
        }
        uriPath += "/direct-debits";
        if (readParams.QueryString is not null)
        {
            uriPath += "?" + readParams.QueryString.TrimStart('?');
        }

        var response =
            await client.GetAsync<DirectDebitsResponse>(
                uriPath,
                GetExtraHeaders(readParams));

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    public async Task<MonzoPotsResponse> MonzoPotsRead(AccountAccessConsentExternalReadParams readParams)
    {
        // Read object
        var uriPath = "/aisp";
        if (readParams.ExternalApiAccountId is not null)
        {
            throw new ArgumentException();
        }
        uriPath += "/monzo-pots";
        if (readParams.QueryString is not null)
        {
            uriPath += "?" + readParams.QueryString.TrimStart('?');
        }

        var response =
            await client.GetAsync<MonzoPotsResponse>(
                uriPath,
                GetExtraHeaders(readParams));

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    public async Task<PartiesResponse> PartiesRead(AccountAccessConsentExternalReadParams readParams)
    {
        // Read object
        var uriPath = "/aisp";
        if (readParams.ExternalApiAccountId is not null)
        {
            uriPath += $"/accounts/{readParams.ExternalApiAccountId}";
        }
        uriPath += "/party";
        if (readParams.QueryString is not null)
        {
            uriPath += "?" + readParams.QueryString.TrimStart('?');
        }

        var response =
            await client.GetAsync<PartiesResponse>(
                uriPath,
                GetExtraHeaders(readParams));

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    public async Task<Parties2Response> Parties2Read(AccountAccessConsentExternalReadParams readParams)
    {
        // Read object
        var uriPath = "/aisp";
        if (readParams.ExternalApiAccountId is not null)
        {
            uriPath += $"/accounts/{readParams.ExternalApiAccountId}";
        }
        else
        {
            throw new ArgumentException();
        }
        uriPath += "/parties";
        if (readParams.QueryString is not null)
        {
            uriPath += "?" + readParams.QueryString.TrimStart('?');
        }

        var response =
            await client.GetAsync<Parties2Response>(
                uriPath,
                GetExtraHeaders(readParams));

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    public async Task<StandingOrdersResponse> StandingOrdersRead(AccountAccessConsentExternalReadParams readParams)
    {
        // Read object
        var uriPath = "/aisp";
        if (readParams.ExternalApiAccountId is not null)
        {
            uriPath += $"/accounts/{readParams.ExternalApiAccountId}";
        }
        uriPath += "/standing-orders";
        if (readParams.QueryString is not null)
        {
            uriPath += "?" + readParams.QueryString.TrimStart('?');
        }

        var response =
            await client.GetAsync<StandingOrdersResponse>(
                uriPath,
                GetExtraHeaders(readParams));

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }

    public async Task<TransactionsResponse> TransactionsRead(AccountAccessConsentExternalReadParams readParams)
    {
        // Read object
        var uriPath = "/aisp";
        if (readParams.ExternalApiAccountId is not null)
        {
            uriPath += $"/accounts/{readParams.ExternalApiAccountId}";
        }
        uriPath += "/transactions";
        if (readParams.QueryString is not null)
        {
            uriPath += "?" + readParams.QueryString.TrimStart('?');
        }

        var response =
            await client.GetAsync<TransactionsResponse>(
                uriPath,
                GetExtraHeaders(readParams));

        // Checks
        response.Warnings.Should().BeNull();
        response.ExternalApiResponse.Should().NotBeNull();

        return response;
    }
}
