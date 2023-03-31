// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.AccountAndTransaction.
    AccountAccessConsent;

public class AccountAccessConsentSubtest
{
    public static ISet<AccountAccessConsentSubtestEnum> AccountAccessConsentSubtestsSupported(
        BankProfile bankProfile) =>
        bankProfile.AccountAndTransactionApi is null
            ? new HashSet<AccountAccessConsentSubtestEnum>()
            : AccountAccessConsentSubtestHelper.AllAccountAccessConsentSubtests;

    public static async Task RunTest(
        AccountAccessConsentSubtestEnum subtestEnum,
        BankProfile bankProfile,
        BankTestData2 testData2,
        Guid bankId,
        Guid bankRegistrationId,
        AccountAndTransactionApiSettings accountAndTransactionApiSettings,
        IRequestBuilder requestBuilderIn,
        Func<IRequestBuilderContainer> requestBuilderGenerator,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder configFluentRequestLogging,
        FilePathBuilder aispFluentRequestLogging,
        ConsentAuth? consentAuth,
        List<BankUser> bankUserList,
        IApiClient apiClient)
    {
        // For now, we just use first bank user in list. Maybe later we can use different users for
        // different sub-tests.
        BankUser bankUser = bankUserList[0];

        IRequestBuilder requestBuilder = requestBuilderIn;

        // Create AccountAccessConsent
        (AccountAccessConsentRequest accountAccessConsentRequest,
            IList<OBReadConsent1DataPermissionsEnum> requestedPermissions) = await GetAccountAccessConsentRequest(
            subtestEnum,
            bankProfile,
            bankRegistrationId,
            testNameUnique,
            modifiedBy,
            aispFluentRequestLogging);
        AccountAccessConsentCreateResponse accountAccessConsentCreateResp =
            await requestBuilder
                .AccountAndTransaction
                .AccountAccessConsents
                .CreateAsync(accountAccessConsentRequest);

        // Checks
        accountAccessConsentCreateResp.Should().NotBeNull();
        accountAccessConsentCreateResp.Warnings.Should().BeNull();
        accountAccessConsentCreateResp.ExternalApiResponse.Should().NotBeNull();

        // Delete AccountAccessConsent
        ObjectDeleteResponse accountAccessConsentDeleteResp = await requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .DeleteAsync(accountAccessConsentCreateResp.Id, modifiedBy);

        // Checks
        accountAccessConsentDeleteResp.Should().NotBeNull();
        accountAccessConsentDeleteResp.Warnings.Should().BeNull();

        // Use existing AccountAccessConsent for further testing (to avoid creating orphan object at bank if test terminates)
        if (testData2.AccountAccessConsentExternalApiId is not null)
        {
            accountAccessConsentRequest.ExternalApiObject =
                new ExternalApiConsent
                {
                    ExternalApiId = testData2.AccountAccessConsentExternalApiId,
                    AccessToken = testData2.AccountAccessConsentRefreshToken is null
                        ? null
                        : new AccessToken
                        {
                            Token = "",
                            ExpiresIn = 0, // to trigger use of refresh token
                            RefreshToken = testData2.AccountAccessConsentRefreshToken,
                            ModifiedBy = modifiedBy
                        },
                    AuthContext = testData2.AccountAccessConsentAuthContextNonce is null
                        ? null
                        : new AuthContextRequest
                        {
                            State = "",
                            Nonce = testData2.AccountAccessConsentAuthContextNonce,
                            ModifiedBy = modifiedBy
                        }
                };

            // Create AccountAccessConsent using existing external API consent
            AccountAccessConsentCreateResponse accountAccessConsentCreateResp2 =
                await requestBuilder
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .CreateAsync(accountAccessConsentRequest);

            // Checks
            accountAccessConsentCreateResp2.Should().NotBeNull();
            accountAccessConsentCreateResp2.Warnings.Should().BeNull();
            accountAccessConsentCreateResp2.ExternalApiResponse.Should().BeNull();

            Guid accountAccessConsentId = accountAccessConsentCreateResp2.Id;

            // GET /account access consents/{consentId}
            AccountAccessConsentCreateResponse accountAccessConsentGetResp =
                await requestBuilder
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .ReadAsync(accountAccessConsentId, modifiedBy);

            // Checks
            accountAccessConsentGetResp.Should().NotBeNull();
            accountAccessConsentGetResp.Warnings.Should().BeNull();
            accountAccessConsentGetResp.ExternalApiResponse.Should().NotBeNull();

            // ObjectDeleteResponse accountAccessConsentDeleteResp2 = await requestBuilder
            //     .AccountAndTransaction
            //     .AccountAccessConsents
            //     .DeleteAsync(accountAccessConsentId, modifiedBy, true);

            // POST auth context
            var authContextRequest = new AccountAccessConsentAuthContext
            {
                AccountAccessConsentId = accountAccessConsentId,
                Reference = testNameUnique + "_AccountAccessConsent",
                CreatedBy = modifiedBy
            };
            AccountAccessConsentAuthContextCreateResponse authContextResponse =
                await requestBuilder
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .AuthContexts
                    .CreateLocalAsync(authContextRequest);

            // Checks
            authContextResponse.Should().NotBeNull();
            authContextResponse.Warnings.Should().BeNull();
            authContextResponse.AuthUrl.Should().NotBeNull();

            Guid authContextId = authContextResponse.Id;
            string authUrl = authContextResponse.AuthUrl;

            // GET auth context
            AccountAccessConsentAuthContextReadResponse authContextResponse2 =
                await requestBuilder.AccountAndTransaction
                    .AccountAccessConsents
                    .AuthContexts
                    .ReadLocalAsync(authContextId);

            // Checks
            authContextResponse2.Should().NotBeNull();
            authContextResponse2.Warnings.Should().BeNull();

            // Consent authorisation
            if (consentAuth is not null)
            {
                // Authorise consent in UI via Playwright
                if (testData2.AccountAccessConsentRefreshToken is null)
                {
                    async Task<bool> AuthIsComplete()
                    {
                        AccountAccessConsentCreateResponse consentResponse =
                            await requestBuilder
                                .AccountAndTransaction
                                .AccountAccessConsents
                                .ReadAsync(
                                    accountAccessConsentId,
                                    modifiedBy,
                                    false);
                        return consentResponse.Created < consentResponse.AuthContextModified;
                    }

                    await consentAuth.AuthoriseAsync(
                        authUrl,
                        bankProfile,
                        ConsentVariety.AccountAccessConsent,
                        bankUser,
                        AuthIsComplete);
                }

                // Refresh scope to ensure user token acquired following consent is available
                using IRequestBuilderContainer scopedRequestBuilderNew = requestBuilderGenerator();
                IRequestBuilder requestBuilderNew = scopedRequestBuilderNew.RequestBuilder;

                // GET /accounts
                AccountsResponse accountsResp =
                    await requestBuilderNew
                        .AccountAndTransaction
                        .Accounts
                        .ReadAsync(
                            accountAccessConsentId,
                            null,
                            modifiedBy);

                // Checks
                accountsResp.Should().NotBeNull();
                accountsResp.Warnings.Should().BeNull();
                accountsResp.ExternalApiResponse.Should().NotBeNull();

                foreach (OBAccount6 account in accountsResp.ExternalApiResponse.Data.Account)
                {
                    string externalAccountId = account.AccountId;

                    // GET /accounts/{accountId}
                    AccountsResponse accountsResp2 =
                        await requestBuilderNew
                            .AccountAndTransaction
                            .Accounts
                            .ReadAsync(
                                accountAccessConsentId,
                                externalAccountId,
                                modifiedBy);

                    // Checks
                    accountsResp2.Should().NotBeNull();
                    accountsResp2.Warnings.Should().BeNull();
                    accountsResp2.ExternalApiResponse.Should().NotBeNull();

                    // GET /accounts/{AccountId}/balances
                    bool testGetBalances =
                        requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadBalances);
                    if (testGetBalances)
                    {
                        BalancesResponse balancesResp =
                            await requestBuilderNew
                                .AccountAndTransaction
                                .Balances
                                .ReadAsync(
                                    accountAccessConsentId,
                                    externalAccountId,
                                    modifiedBy);

                        // Checks
                        balancesResp.Should().NotBeNull();
                        balancesResp.Warnings.Should().BeNull();
                        balancesResp.ExternalApiResponse.Should().NotBeNull();
                    }

                    // GET /accounts/{AccountId}/transactions
                    bool hasReadTransactionsBasicOrDetail =
                        requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadTransactionsBasic) ||
                        requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadTransactionsDetail);
                    bool hasReadTransactionsCreditsOrDebits =
                        requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadTransactionsCredits) ||
                        requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadTransactionsDebits);
                    bool testGetTransactions =
                        hasReadTransactionsBasicOrDetail && hasReadTransactionsCreditsOrDebits;
                    if (testGetTransactions)
                    {
                        const int maxPages = 30;
                        var page = 0;
                        string? queryString = null;
                        do
                        {
                            TransactionsResponse transactionsResp =
                                await requestBuilderNew
                                    .AccountAndTransaction
                                    .Transactions
                                    .ReadAsync(
                                        accountAccessConsentId,
                                        externalAccountId,
                                        null,
                                        modifiedBy,
                                        queryString);

                            // Checks
                            transactionsResp.Should().NotBeNull();
                            transactionsResp.Warnings.Should().BeNull();
                            transactionsResp.ExternalApiResponse.Should().NotBeNull();

                            // Update query string based on "Next" link
                            queryString = transactionsResp.ExternalApiResponse.Links.Next;
                            page++;
                        } while (queryString is not null && page < maxPages);
                    }

                    // GET /party
                    bool testReadPartyPsu =
                        requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadPartyPSU);

                    if (testReadPartyPsu)
                    {
                        PartiesResponse partyResp =
                            await requestBuilderNew
                                .AccountAndTransaction
                                .Parties
                                .ReadAsync(
                                    accountAccessConsentId,
                                    null,
                                    modifiedBy);

                        // Checks
                        partyResp.Should().NotBeNull();
                        partyResp.Warnings.Should().BeNull();
                        partyResp.ExternalApiResponse.Should().NotBeNull();
                    }

                    // GET /accounts/{AccountId}/party
                    // GET /accounts/{AccountId}/parties
                    bool testGetParties =
                        requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadParty);
                    if (testGetParties)
                    {
                        if (bankProfile.AccountAndTransactionApiSettings.UseGetPartyEndpoint)
                        {
                            PartiesResponse partyResp =
                                await requestBuilderNew
                                    .AccountAndTransaction
                                    .Parties
                                    .ReadAsync(
                                        accountAccessConsentId,
                                        externalAccountId,
                                        modifiedBy);

                            // Checks
                            partyResp.Should().NotBeNull();
                            partyResp.Warnings.Should().BeNull();
                            partyResp.ExternalApiResponse.Should().NotBeNull();
                        }

                        Parties2Response partiesResp =
                            await requestBuilderNew
                                .AccountAndTransaction
                                .Parties2
                                .ReadAsync(
                                    accountAccessConsentId,
                                    externalAccountId,
                                    modifiedBy);

                        // Checks
                        partiesResp.Should().NotBeNull();
                        partiesResp.Warnings.Should().BeNull();
                        partiesResp.ExternalApiResponse.Should().NotBeNull();
                    }

                    // GET /accounts/{AccountId}/direct-debits
                    bool testGetDirectDebits =
                        requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadDirectDebits);
                    if (testGetDirectDebits)
                    {
                        DirectDebitsResponse directDebitsResp =
                            await requestBuilderNew
                                .AccountAndTransaction
                                .DirectDebits
                                .ReadAsync(
                                    accountAccessConsentId,
                                    externalAccountId,
                                    modifiedBy);

                        // Checks
                        directDebitsResp.Should().NotBeNull();
                        directDebitsResp.Warnings.Should().BeNull();
                        directDebitsResp.ExternalApiResponse.Should().NotBeNull();
                    }

                    // GET /accounts/{AccountId}/standing-orders
                    bool testGetStandingOrders =
                        requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadStandingOrdersBasic) ||
                        requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadStandingOrdersDetail);
                    if (testGetStandingOrders)
                    {
                        StandingOrdersResponse standingOrdersResp =
                            await requestBuilderNew
                                .AccountAndTransaction
                                .StandingOrders
                                .ReadAsync(
                                    accountAccessConsentId,
                                    externalAccountId,
                                    modifiedBy);

                        // Checks
                        standingOrdersResp.Should().NotBeNull();
                        standingOrdersResp.Warnings.Should().BeNull();
                        standingOrdersResp.ExternalApiResponse.Should().NotBeNull();
                    }
                }
            }
        }
    }

    private static async Task<AccountAndTransactionApiRequest> GetAccountAndTransactionApiRequest(
        BankProfile bankProfile,
        Guid bankId,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder configFluentRequestLogging)
    {
        var accountAndTransactionApiRequest = new AccountAndTransactionApiRequest
        {
            BankProfile = bankProfile.BankProfileEnum,
            BankId = Guid.Empty,
            ApiVersion = bankProfile.GetRequiredAccountAndTransactionApi().ApiVersion,
            BaseUrl = bankProfile.GetRequiredAccountAndTransactionApi().BaseUrl
        };
        await configFluentRequestLogging
            .AppendToPath("accountAndTransactionApi")
            .AppendToPath("postRequest")
            .WriteFile(accountAndTransactionApiRequest);
        accountAndTransactionApiRequest.BankId = bankId;
        accountAndTransactionApiRequest.Reference = testNameUnique;
        accountAndTransactionApiRequest.CreatedBy = modifiedBy;
        return accountAndTransactionApiRequest;
    }

    private static async
        Task<(AccountAccessConsentRequest accountAccessConsentRequest, IList<OBReadConsent1DataPermissionsEnum>
            requestedPermissions)> GetAccountAccessConsentRequest(
            AccountAccessConsentSubtestEnum subtestEnum,
            BankProfile bankProfile,
            Guid bankRegistrationId,
            string testNameUnique,
            string modifiedBy,
            FilePathBuilder aispFluentRequestLogging)
    {
        var accountAccessConsentRequest =
            new AccountAccessConsentRequest
            {
                BankRegistrationId = default, // substitute logging placeholder
                TemplateRequest = new AccountAccessConsentTemplateRequest
                {
                    Type = AccountAccessConsentSubtestHelper
                        .GetAccountAccessConsentTemplateType(subtestEnum)
                }
            };

        accountAccessConsentRequest.ExternalApiRequest =
            AccountAccessConsentPublicMethods.ResolveExternalApiRequest(
                accountAccessConsentRequest.ExternalApiRequest,
                accountAccessConsentRequest.TemplateRequest,
                bankProfile); // Resolve for fuller logging
        DateTimeOffset?
            expDateTime =
                accountAccessConsentRequest.ExternalApiRequest!.Data
                    .ExpirationDateTime; // ExternalApiRequest always non-null in low-level request
        if (expDateTime is not null)
        {
            accountAccessConsentRequest.ExternalApiRequest!.Data.ExpirationDateTime =
                default(DateTimeOffset); // substitute logging placeholder
        }

        await aispFluentRequestLogging
            .AppendToPath("accountAccessConsent")
            .AppendToPath("postRequest")
            .WriteFile(accountAccessConsentRequest);
        accountAccessConsentRequest.BankRegistrationId = bankRegistrationId; // remove logging placeholder
        if (expDateTime is not null)
        {
            accountAccessConsentRequest.ExternalApiRequest!.Data.ExpirationDateTime =
                expDateTime; // remove logging placeholder
        }

        accountAccessConsentRequest.Reference = testNameUnique; // remove logging placeholder
        accountAccessConsentRequest.CreatedBy = modifiedBy; // remove logging placeholder

        IList<OBReadConsent1DataPermissionsEnum> requestedPermissions =
            accountAccessConsentRequest.ExternalApiRequest.Data.Permissions;
        return (accountAccessConsentRequest, requestedPermissions);
    }
}
