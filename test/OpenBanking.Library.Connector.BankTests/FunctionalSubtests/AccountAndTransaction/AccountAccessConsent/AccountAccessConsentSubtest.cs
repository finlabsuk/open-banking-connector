﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.AccountAndTransaction.
    AccountAccessConsent
{
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
            FilePathBuilder configFluentRequestLogging,
            FilePathBuilder aispFluentRequestLogging,
            ConsentAuth? consentAuth,
            List<BankUser> bankUserList,
            IApiClient apiClient)
        {
            // For now, we just use first bank user in list. Maybe later we can use different users for
            // different sub-tests.
            BankUser bankUser = bankUserList[0];

            var modifiedBy = "Automated bank tests";

            IRequestBuilder requestBuilder = requestBuilderIn;

            // Create AccountAndTransactionApi
            AccountAndTransactionApiRequest accountAndTransactionApiRequest =
                bankProfile.GetAccountAndTransactionApiRequest(Guid.Empty);
            await configFluentRequestLogging
                .AppendToPath("accountAndTransactionApi")
                .AppendToPath("postRequest")
                .WriteFile(accountAndTransactionApiRequest);
            accountAndTransactionApiRequest.BankId = bankId;
            accountAndTransactionApiRequest.CreatedBy = modifiedBy;
            accountAndTransactionApiRequest.Reference = testNameUnique;
            AccountAndTransactionApiResponse accountAndTransactionApiResponse =
                await requestBuilder
                    .BankConfiguration
                    .AccountAndTransactionApis
                    .CreateLocalAsync(accountAndTransactionApiRequest);
            accountAndTransactionApiResponse.Should().NotBeNull();
            accountAndTransactionApiResponse.Warnings.Should().BeNull();
            Guid accountAndTransactionApiId = accountAndTransactionApiResponse.Id;

            // Read AccountAndTransactionApi
            AccountAndTransactionApiResponse accountAndTransactionApiReadResponse =
                await requestBuilder
                    .BankConfiguration
                    .AccountAndTransactionApis
                    .ReadLocalAsync(accountAndTransactionApiId);

            // Checks
            accountAndTransactionApiReadResponse.Should().NotBeNull();
            accountAndTransactionApiReadResponse.Warnings.Should().BeNull();

            // Create account access consent or use existing
            Connector.Models.Public.AccountAndTransaction.Request.AccountAccessConsent accountAccessConsentRequest =
                bankProfile.AccountAccessConsentRequest(
                    Guid.Empty, // set below
                    Guid.Empty,
                    AccountAccessConsentSubtestHelper.GetAccountAccessConsentType(subtestEnum));
            await aispFluentRequestLogging
                .AppendToPath("accountAccessConsent")
                .AppendToPath("postRequest")
                .WriteFile(accountAccessConsentRequest);

            if (testData2.AccountAccessConsentExternalApiId is not null)
            {
                accountAccessConsentRequest.ExternalApiObject = new ExternalApiConsent
                {
                    ExternalApiId = testData2.AccountAccessConsentExternalApiId,
                    AccessToken = testData2.AccountAccessConsentRefreshToken is null
                        ? null
                        : new AccessToken
                        {
                            RefreshToken = testData2.AccountAccessConsentRefreshToken,
                            ModifiedBy = modifiedBy
                        }
                };
            }

            accountAccessConsentRequest.BankRegistrationId = bankRegistrationId;
            accountAccessConsentRequest.AccountAndTransactionApiId = accountAndTransactionApiId;
            accountAccessConsentRequest.CreatedBy = modifiedBy;
            accountAccessConsentRequest.Reference = testNameUnique;
            AccountAccessConsentResponse accountAccessConsentResp =
                await requestBuilder
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .CreateAsync(accountAccessConsentRequest);

            // Checks
            accountAccessConsentResp.Should().NotBeNull();
            accountAccessConsentResp.Warnings.Should().BeNull();
            accountAccessConsentResp.ExternalApiResponse.Should().NotBeNull();
            Guid accountAccessConsentId = accountAccessConsentResp.Id;

            // GET /account access consents/{consentId}
            AccountAccessConsentResponse accountAccessConsentResp2 =
                await requestBuilder
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .ReadAsync(accountAccessConsentId);

            // Checks
            accountAccessConsentResp2.Should().NotBeNull();
            accountAccessConsentResp2.Warnings.Should().BeNull();
            accountAccessConsentResp2.ExternalApiResponse.Should().NotBeNull();

            // var xx = await requestBuilder
            //     .AccountAndTransaction
            //     .AccountAccessConsents
            //     .DeleteAsync(accountAccessConsentId);

            // POST auth context
            var authContextRequest = new AccountAccessConsentAuthContext
            {
                AccountAccessConsentId = accountAccessConsentId,
                Reference = testNameUnique + "_AccountAccessConsent"
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
            string authUrl = authContextResponse.AuthUrl!;

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
                    await consentAuth.AuthoriseAsync(
                        authUrl,
                        bankProfile.BankProfileEnum,
                        ConsentVariety.AccountAccessConsent,
                        bankUser);
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
                            null,
                            null,
                            null,
                            "",
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
                                null,
                                null,
                                null,
                                "",
                                modifiedBy);

                    // Checks
                    accountsResp2.Should().NotBeNull();
                    accountsResp2.Warnings.Should().BeNull();
                    accountsResp2.ExternalApiResponse.Should().NotBeNull();

                    // GET /balances/{accountId}
                    BalancesResponse balancesResp =
                        await requestBuilderNew
                            .AccountAndTransaction
                            .Balances
                            .ReadAsync(
                                accountAccessConsentId,
                                externalAccountId,
                                null,
                                null,
                                null,
                                "",
                                modifiedBy);

                    // Checks
                    balancesResp.Should().NotBeNull();
                    balancesResp.Warnings.Should().BeNull();
                    balancesResp.ExternalApiResponse.Should().NotBeNull();

                    // GET /transactions/{accountId}
                    const int maxPages = 30;
                    var page = 0;
                    var queryString = "";
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
                                    null,
                                    null,
                                    null,
                                    modifiedBy,
                                    null,
                                    queryString);

                        // Checks
                        transactionsResp.Should().NotBeNull();
                        transactionsResp.Warnings.Should().BeNull();
                        transactionsResp.ExternalApiResponse.Should().NotBeNull();

                        // Update query string based on "Next" link
                        queryString = transactionsResp.ExternalApiResponse.Links.Next;
                        page++;
                    } while (queryString is not null && page < maxPages);

                    // GET /party/{accountId}
                    PartiesResponse partyResp =
                        await requestBuilderNew
                            .AccountAndTransaction
                            .Parties
                            .ReadAsync(
                                accountAccessConsentId,
                                externalAccountId,
                                null,
                                null,
                                null,
                                "",
                                modifiedBy);

                    // Checks
                    partyResp.Should().NotBeNull();
                    partyResp.Warnings.Should().BeNull();
                    partyResp.ExternalApiResponse.Should().NotBeNull();

                    // GET /parties/{accountId}
                    Parties2Response partiesResp =
                        await requestBuilderNew
                            .AccountAndTransaction
                            .Parties2
                            .ReadAsync(
                                accountAccessConsentId,
                                externalAccountId,
                                null,
                                null,
                                null,
                                "",
                                modifiedBy);

                    // Checks
                    partiesResp.Should().NotBeNull();
                    partiesResp.Warnings.Should().BeNull();
                    partiesResp.ExternalApiResponse.Should().NotBeNull();
                }

                // Delete AccountAccessConsent
                var includeExternalApiOperation = true;
                ObjectDeleteResponse accountAccessConsentResp3 = await requestBuilderNew
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .DeleteAsync(accountAccessConsentId, modifiedBy, includeExternalApiOperation);

                // Checks
                accountAccessConsentResp3.Should().NotBeNull();
                accountAccessConsentResp3.Warnings.Should().BeNull();
            }

            // Delete AccountAndTransactionApi
            ObjectDeleteResponse apiResponse = await requestBuilder
                .BankConfiguration
                .AccountAndTransactionApis
                .DeleteLocalAsync(accountAndTransactionApiId);

            // Checks
            apiResponse.Should().NotBeNull();
            apiResponse.Warnings.Should().BeNull();
        }
    }
}
