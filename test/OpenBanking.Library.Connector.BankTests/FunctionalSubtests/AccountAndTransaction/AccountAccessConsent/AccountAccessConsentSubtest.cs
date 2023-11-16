// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
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
        Guid bankRegistrationId,
        AccountAndTransactionApiSettings accountAndTransactionApiSettings,
        IRequestBuilder requestBuilderIn,
        Func<IRequestBuilderContainer> requestBuilderGenerator,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder configFluentRequestLogging,
        FilePathBuilder aispFluentRequestLogging,
        ConsentAuth? consentAuth,
        string authUrlLeftPart,
        BankUser? bankUser,
        AppTests.AccountAccessConsentOptions accountAccessConsentOptions,
        IApiClient apiClient)
    {
        IRequestBuilder requestBuilder = requestBuilderIn;

        (AccountAccessConsentRequest accountAccessConsentRequest,
            IList<OBReadConsent1DataPermissionsEnum> requestedPermissions) = await GetAccountAccessConsentRequest(
            subtestEnum,
            bankProfile,
            bankRegistrationId,
            testNameUnique,
            modifiedBy,
            aispFluentRequestLogging);

        // Handle "only delete" case
        if (accountAccessConsentOptions is AppTests.AccountAccessConsentOptions.OnlyDeleteConsent)
        {
            // Create AccountAccessConsent using existing external API consent
            accountAccessConsentRequest.ExternalApiObject =
                GetRequiredExternalApiConsent(testData2, modifiedBy);
            Guid accountAccessConsentId =
                await CreateAccountAccessConsent(accountAccessConsentRequest, requestBuilder);

            // Delete AccountAccessConsent (includes external API delete)
            await DeleteAccountAccessConsent(modifiedBy, requestBuilder, accountAccessConsentId, true);
        }

        // Handle "only create" case
        else if (accountAccessConsentOptions is AppTests.AccountAccessConsentOptions.OnlyCreateConsent)
        {
            // Create fresh AccountAccessConsent
            Guid _ =
                await CreateAccountAccessConsent(accountAccessConsentRequest, requestBuilder);
        }

        // Handle "normal" case
        else
        {
            // Test creation and deletion of fresh consent

            // Create fresh AccountAccessConsent
            Guid accountAccessConsentId1 =
                await CreateAccountAccessConsent(accountAccessConsentRequest, requestBuilder);

            // Read account access consent
            await ReadAccountAccessConsent(modifiedBy, requestBuilder, accountAccessConsentId1);

            // Delete AccountAccessConsent (includes external API delete)
            await DeleteAccountAccessConsent(modifiedBy, requestBuilder, accountAccessConsentId1, true);

            // Perform further testing with existing AccountAccessConsent
            // (to avoid creating orphan object at bank if test terminates) unless re-auth not used
            bool notUsingReAuth = !bankProfile.AccountAndTransactionApiSettings.UseReauth;
            if (notUsingReAuth || testData2.AccountAccessConsentExternalApiId is not null)
            {
                Guid accountAccessConsentId2;
                if (notUsingReAuth)
                {
                    // Create fresh AccountAccessConsent
                    accountAccessConsentId2 =
                        await CreateAccountAccessConsent(accountAccessConsentRequest, requestBuilder);
                }
                else
                {
                    // Create AccountAccessConsent using existing external API consent
                    accountAccessConsentRequest.ExternalApiObject =
                        GetRequiredExternalApiConsent(testData2, modifiedBy);
                    accountAccessConsentId2 = await CreateAccountAccessConsent(
                        accountAccessConsentRequest,
                        requestBuilder);
                }

                // Read account access consent
                await ReadAccountAccessConsent(modifiedBy, requestBuilder, accountAccessConsentId2);

                // Consent authorisation
                if (consentAuth is not null &&
                    testData2.AuthDisable is not true)
                {
                    // Authorise consent in UI via Playwright
                    async Task<bool> AuthIsComplete()
                    {
                        AccountAccessConsentCreateResponse consentResponse =
                            await requestBuilder
                                .AccountAndTransaction
                                .AccountAccessConsents
                                .ReadAsync(
                                    accountAccessConsentId2,
                                    modifiedBy,
                                    false);
                        return consentResponse.Created < consentResponse.AuthContextModified;
                    }

                    // Perform auth
                    string authUrl;
                    if (bankProfile.SupportsSca)
                    {
                        // POST auth context
                        var authContextRequest = new AccountAccessConsentAuthContext
                        {
                            AccountAccessConsentId = accountAccessConsentId2,
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
                        authUrl = authContextResponse.AuthUrl;

                        // GET auth context
                        AccountAccessConsentAuthContextReadResponse authContextResponse2 =
                            await requestBuilder.AccountAndTransaction
                                .AccountAccessConsents
                                .AuthContexts
                                .ReadLocalAsync(authContextId);

                        // Checks
                        authContextResponse2.Should().NotBeNull();
                        authContextResponse2.Warnings.Should().BeNull();

                        // Perform email auth
                        await consentAuth.EmailAuthAsync(
                            authUrl,
                            AuthIsComplete);
                    }
                    else
                    {
                        if (bankUser is null)
                        {
                            throw new ArgumentException("No user specified for consent auth.");
                        }

                        // Perform automated auth
                        authUrl =
                            $"{authUrlLeftPart}/dev1/aisp/account-access-consents/{accountAccessConsentId2}/auth";
                        await consentAuth.AutomatedAuthAsync(
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
                                accountAccessConsentId2,
                                null,
                                modifiedBy);

                    // Checks
                    accountsResp.Should().NotBeNull();
                    accountsResp.Warnings.Should().BeNull();
                    accountsResp.ExternalApiResponse.Should().NotBeNull();

                    foreach (OBAccount6 account in accountsResp.ExternalApiResponse.Data.Account)
                    {
                        string externalAccountId = account.AccountId;
                        OBExternalAccountSubType1CodeEnum? accountSubType = account.AccountSubType;

                        // GET /accounts/{accountId}
                        AccountsResponse accountsResp2 =
                            await requestBuilderNew
                                .AccountAndTransaction
                                .Accounts
                                .ReadAsync(
                                    accountAccessConsentId2,
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
                                        accountAccessConsentId2,
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
                                            accountAccessConsentId2,
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
                            } while (queryString is not null &&
                                     page < maxPages);
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
                                        accountAccessConsentId2,
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
                                            accountAccessConsentId2,
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
                                        accountAccessConsentId2,
                                        externalAccountId,
                                        modifiedBy);

                            // Checks
                            partiesResp.Should().NotBeNull();
                            partiesResp.Warnings.Should().BeNull();
                            partiesResp.ExternalApiResponse.Should().NotBeNull();
                        }

                        // GET /accounts/{AccountId}/direct-debits
                        bool testGetDirectDebits =
                            requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadDirectDebits) &&
                            accountSubType is not OBExternalAccountSubType1CodeEnum.CreditCard;
                        if (testGetDirectDebits)
                        {
                            DirectDebitsResponse directDebitsResp =
                                await requestBuilderNew
                                    .AccountAndTransaction
                                    .DirectDebits
                                    .ReadAsync(
                                        accountAccessConsentId2,
                                        externalAccountId,
                                        modifiedBy);

                            // Checks
                            directDebitsResp.Should().NotBeNull();
                            directDebitsResp.Warnings.Should().BeNull();
                            directDebitsResp.ExternalApiResponse.Should().NotBeNull();
                        }

                        // GET /accounts/{AccountId}/standing-orders
                        bool testGetStandingOrders =
                            (requestedPermissions.Contains(OBReadConsent1DataPermissionsEnum.ReadStandingOrdersBasic) ||
                             requestedPermissions.Contains(
                                 OBReadConsent1DataPermissionsEnum.ReadStandingOrdersDetail)) &&
                            accountSubType is not OBExternalAccountSubType1CodeEnum.CreditCard;
                        if (testGetStandingOrders)
                        {
                            StandingOrdersResponse standingOrdersResp =
                                await requestBuilderNew
                                    .AccountAndTransaction
                                    .StandingOrders
                                    .ReadAsync(
                                        accountAccessConsentId2,
                                        externalAccountId,
                                        modifiedBy);

                            // Checks
                            standingOrdersResp.Should().NotBeNull();
                            standingOrdersResp.Warnings.Should().BeNull();
                            standingOrdersResp.ExternalApiResponse.Should().NotBeNull();
                        }

                        if (bankProfile.BankProfileEnum is BankProfileEnum.Monzo_Sandbox or BankProfileEnum.Monzo_Monzo)
                        {
                            MonzoPotsResponse monzoPotsResp =
                                await requestBuilderNew
                                    .AccountAndTransaction
                                    .MonzoPots
                                    .ReadAsync(
                                        accountAccessConsentId2,
                                        null,
                                        modifiedBy);

                            // Checks
                            monzoPotsResp.Should().NotBeNull();
                            monzoPotsResp.Warnings.Should().BeNull();
                            monzoPotsResp.ExternalApiResponse.Should().NotBeNull();
                        }
                    }
                }

                // Delete AccountAccessConsent (excludes external API delete)
                await DeleteAccountAccessConsent(
                    modifiedBy,
                    requestBuilder,
                    accountAccessConsentId2,
                    notUsingReAuth);
            }
        }
    }

    private static async Task ReadAccountAccessConsent(
        string modifiedBy,
        IRequestBuilder requestBuilder,
        Guid accountAccessConsentId)
    {
        AccountAccessConsentCreateResponse accountAccessConsentGetResp =
            await requestBuilder
                .AccountAndTransaction
                .AccountAccessConsents
                .ReadAsync(accountAccessConsentId, modifiedBy);

        // Checks
        accountAccessConsentGetResp.Should().NotBeNull();
        accountAccessConsentGetResp.Warnings.Should().BeNull();
        accountAccessConsentGetResp.ExternalApiResponse.Should().NotBeNull();
    }

    private static async Task DeleteAccountAccessConsent(
        string modifiedBy,
        IRequestBuilder requestBuilder,
        Guid accountAccessConsentId,
        bool includeExternalApiOperation)
    {
        // Delete AccountAccessConsent
        ObjectDeleteResponse accountAccessConsentDeleteResp2 = await requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .DeleteAsync(accountAccessConsentId, modifiedBy, includeExternalApiOperation);

        // Checks
        accountAccessConsentDeleteResp2.Should().NotBeNull();
        accountAccessConsentDeleteResp2.Warnings.Should().BeNull();
    }

    private static async Task<Guid> CreateAccountAccessConsent(
        AccountAccessConsentRequest accountAccessConsentRequest,
        IRequestBuilder requestBuilder)
    {
        AccountAccessConsentCreateResponse accountAccessConsentCreateResp2 =
            await requestBuilder
                .AccountAndTransaction
                .AccountAccessConsents
                .CreateAsync(accountAccessConsentRequest);

        // Checks
        accountAccessConsentCreateResp2.Should().NotBeNull();
        accountAccessConsentCreateResp2.Warnings.Should().BeNull();
        if (accountAccessConsentRequest.ExternalApiObject is not null)
        {
            accountAccessConsentCreateResp2.ExternalApiResponse.Should().BeNull();
        }
        else
        {
            accountAccessConsentCreateResp2.ExternalApiResponse.Should().NotBeNull();
        }

        Guid accountAccessConsentId = accountAccessConsentCreateResp2.Id;
        return accountAccessConsentId;
    }

    private static ExternalApiConsent GetRequiredExternalApiConsent(BankTestData2 testData2, string modifiedBy) =>
        new()
        {
            ExternalApiId =
                testData2.AccountAccessConsentExternalApiId ??
                throw new InvalidOperationException("No external API AccountAccessConsent ID provided."),
            AuthContext = testData2.AccountAccessConsentAuthContextNonce is null
                ? null
                : new AuthContextRequest
                {
                    State = "",
                    Nonce = testData2.AccountAccessConsentAuthContextNonce,
                    ModifiedBy = modifiedBy
                }
        };

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
