// Licensed to Finnovation Labs Limited under one or more agreements.
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

            var createdBy = "Automated bank tests";

            IRequestBuilder requestBuilder = requestBuilderIn;

            // Create AccountAndTransactionApi
            AccountAndTransactionApiRequest accountAndTransactionApiRequest =
                bankProfile.GetAccountAndTransactionApiRequest(Guid.Empty);
            await configFluentRequestLogging
                .AppendToPath("accountAndTransactionApi")
                .AppendToPath("postRequest")
                .WriteFile(accountAndTransactionApiRequest);
            accountAndTransactionApiRequest.BankId = bankId;
            accountAndTransactionApiRequest.CreatedBy = createdBy;
            accountAndTransactionApiRequest.Reference = testNameUnique;
            IFluentResponse<AccountAndTransactionApiResponse> accountAndTransactionApiResponse =
                await requestBuilder
                    .BankConfiguration
                    .AccountAndTransactionApis
                    .CreateLocalAsync(accountAndTransactionApiRequest);
            accountAndTransactionApiResponse.Should().NotBeNull();
            accountAndTransactionApiResponse.Messages.Should().BeEmpty();
            accountAndTransactionApiResponse.Data.Should().NotBeNull();
            Guid accountAndTransactionApiId = accountAndTransactionApiResponse.Data!.Id;

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
                            ModifiedBy = createdBy
                        }
                };
            }

            accountAccessConsentRequest.BankRegistrationId = bankRegistrationId;
            accountAccessConsentRequest.AccountAndTransactionApiId = accountAndTransactionApiId;
            accountAccessConsentRequest.CreatedBy = createdBy;
            accountAccessConsentRequest.Reference = testNameUnique;
            IFluentResponse<AccountAccessConsentReadResponse> accountAccessConsentResp =
                await requestBuilder
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .CreateAsync(accountAccessConsentRequest);

            // Checks
            accountAccessConsentResp.Should().NotBeNull();
            accountAccessConsentResp.Messages.Should().BeEmpty();
            accountAccessConsentResp.Data.Should().NotBeNull();
            Guid accountAccessConsentId = accountAccessConsentResp.Data!.Id;

            // GET /account access consents/{consentId}
            IFluentResponse<AccountAccessConsentReadResponse> accountAccessConsentResp2 =
                await requestBuilder
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .ReadAsync(accountAccessConsentId);

            // Checks
            accountAccessConsentResp2.Should().NotBeNull();
            accountAccessConsentResp2.Messages.Should().BeEmpty();
            accountAccessConsentResp2.Data.Should().NotBeNull();

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
            IFluentResponse<AccountAccessConsentAuthContextCreateLocalResponse> authContextResponse =
                await requestBuilder
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .AuthContexts
                    .CreateLocalAsync(authContextRequest);

            // Checks
            authContextResponse.Should().NotBeNull();
            authContextResponse.Messages.Should().BeEmpty();
            authContextResponse.Data.Should().NotBeNull();
            authContextResponse.Data!.AuthUrl.Should().NotBeNull();
            Guid authContextId = authContextResponse.Data!.Id;
            string authUrl = authContextResponse.Data!.AuthUrl!;

            // GET auth context
            IFluentResponse<AccountAccessConsentAuthContextReadLocalResponse> authContextResponse2 =
                await requestBuilder.AccountAndTransaction
                    .AccountAccessConsents
                    .AuthContexts
                    .ReadLocalAsync(authContextId);

            // Checks
            authContextResponse2.Should().NotBeNull();
            authContextResponse2.Messages.Should().BeEmpty();
            authContextResponse2.Data.Should().NotBeNull();

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
                IFluentResponse<AccountsResponse> accountsResp =
                    await requestBuilderNew
                        .AccountAndTransaction
                        .Accounts
                        .ReadAsync(accountAccessConsentId);

                // Checks
                accountsResp.Should().NotBeNull();
                accountsResp.Messages.Should().BeEmpty();
                accountsResp.Data.Should().NotBeNull();

                foreach (OBAccount6 account in accountsResp.Data!.ExternalApiResponse.Data.Account)
                {
                    string externalAccountId = account.AccountId;

                    // GET /accounts/{accountId}
                    IFluentResponse<AccountsResponse> accountsResp2 =
                        await requestBuilderNew
                            .AccountAndTransaction
                            .Accounts
                            .ReadAsync(accountAccessConsentId, externalAccountId);

                    // Checks
                    accountsResp2.Should().NotBeNull();
                    accountsResp2.Messages.Should().BeEmpty();
                    accountsResp2.Data.Should().NotBeNull();

                    // GET /balances/{accountId}
                    IFluentResponse<BalancesResponse> balancesResp =
                        await requestBuilderNew
                            .AccountAndTransaction
                            .Balances
                            .ReadAsync(accountAccessConsentId, externalAccountId);

                    // Checks
                    balancesResp.Should().NotBeNull();
                    balancesResp.Messages.Should().BeEmpty();
                    balancesResp.Data.Should().NotBeNull();

                    // GET /transactions/{accountId}
                    var queryString = "";
                    do
                    {
                        IFluentResponse<TransactionsResponse> transactionsResp =
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
                                    createdBy,
                                    null,
                                    queryString);

                        // Checks
                        transactionsResp.Should().NotBeNull();
                        transactionsResp.Messages.Should().BeEmpty();
                        transactionsResp.Data.Should().NotBeNull();

                        // Update query string based on "Next" link
                        queryString = transactionsResp.Data!.ExternalApiResponse.Links.Next;
                    } while (queryString is not null);

                    // GET /party/{accountId}
                    IFluentResponse<PartiesResponse> partyResp =
                        await requestBuilderNew
                            .AccountAndTransaction
                            .Parties
                            .ReadAsync(accountAccessConsentId, externalAccountId);

                    // Checks
                    partyResp.Should().NotBeNull();
                    partyResp.Messages.Should().BeEmpty();
                    partyResp.Data.Should().NotBeNull();

                    // GET /parties/{accountId}
                    IFluentResponse<Parties2Response> partiesResp =
                        await requestBuilderNew
                            .AccountAndTransaction
                            .Parties2
                            .ReadAsync(accountAccessConsentId, externalAccountId);

                    // Checks
                    partiesResp.Should().NotBeNull();
                    partiesResp.Messages.Should().BeEmpty();
                    partiesResp.Data.Should().NotBeNull();
                }

                // DELETE account access consent
                IFluentResponse accountAccessConsentResp3 = await requestBuilderNew
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .DeleteAsync(accountAccessConsentId);

                // Checks
                accountAccessConsentResp3.Should().NotBeNull();
                accountAccessConsentResp3.Messages.Should().BeEmpty();
            }

            // DELETE API object
            IFluentResponse apiResponse = await requestBuilder
                .BankConfiguration
                .AccountAndTransactionApis
                .DeleteLocalAsync(accountAndTransactionApiId);

            // Checks
            apiResponse.Should().NotBeNull();
            apiResponse.Messages.Should().BeEmpty();
        }
    }
}
