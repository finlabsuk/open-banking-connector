﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FluentAssertions;
using Jering.Javascript.NodeJS;

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
            Guid bankId,
            Guid bankRegistrationId,
            AccountAndTransactionApiSettings accountAndTransactionApiSettings,
            IRequestBuilder requestBuilderIn,
            Func<IRequestBuilderContainer> requestBuilderGenerator,
            string testNameUnique,
            FilePathBuilder testDataProcessorFluentRequestLogging,
            bool includeConsentAuth,
            INodeJSService? nodeJsService,
            PuppeteerLaunchOptionsJavaScript? puppeteerLaunchOptions,
            List<BankUser> bankUserList,
            IApiClient apiClient)
        {
            // For now, we just use first bank user in list. Maybe later we can use different users for
            // different sub-tests.
            BankUser bankUser = bankUserList[0];

            IRequestBuilder requestBuilder = requestBuilderIn;

            // Create AccountAndTransactionApi
            AccountAndTransactionApiRequest accountAndTransactionApiRequest =
                bankProfile.GetAccountAndTransactionApiRequest(Guid.Empty);
            await testDataProcessorFluentRequestLogging
                .AppendToPath("accountAndTransactionApi")
                .AppendToPath("postRequest")
                .WriteFile(accountAndTransactionApiRequest);
            accountAndTransactionApiRequest.Name = testNameUnique;
            accountAndTransactionApiRequest.BankId = bankId;
            IFluentResponse<AccountAndTransactionApiResponse> accountAndTransactionApiResponse =
                await requestBuilder
                    .BankConfiguration
                    .AccountAndTransactionApis
                    .CreateLocalAsync(accountAndTransactionApiRequest);
            accountAndTransactionApiResponse.Should().NotBeNull();
            accountAndTransactionApiResponse.Messages.Should().BeEmpty();
            accountAndTransactionApiResponse.Data.Should().NotBeNull();
            Guid accountAndTransactionApiId = accountAndTransactionApiResponse.Data!.Id;

            // Basic request object for account access consent
            Connector.Models.Public.AccountAndTransaction.Request.AccountAccessConsent accountAccessConsentRequest =
                bankProfile.AccountAccessConsentRequest(
                    Guid.Empty, // set below
                    Guid.Empty,
                    AccountAccessConsentSubtestHelper.AccountAccessConsentType(subtestEnum),
                    null, // set below
                    "Automated bank tests");
            await testDataProcessorFluentRequestLogging
                .AppendToPath("accountAccessConsent")
                .AppendToPath("postRequest")
                .WriteFile(accountAccessConsentRequest);

            // POST account access consent
            accountAccessConsentRequest.BankRegistrationId = bankRegistrationId;
            accountAccessConsentRequest.AccountAndTransactionApiId = accountAndTransactionApiId;
            accountAccessConsentRequest.Name = testNameUnique;
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

            // GET account access consent
            IFluentResponse<AccountAccessConsentReadResponse> accountAccessConsentResp2 =
                await requestBuilder
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .ReadAsync(accountAccessConsentId);

            // Checks
            accountAccessConsentResp2.Should().NotBeNull();
            accountAccessConsentResp2.Messages.Should().BeEmpty();
            accountAccessConsentResp2.Data.Should().NotBeNull();

            // POST auth context
            var authContextRequest = new AccountAccessConsentAuthContext
            {
                AccountAccessConsentId = accountAccessConsentId,
                Name = testNameUnique + "_AccountAccessConsent"
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
            if (includeConsentAuth)
            {
                if (puppeteerLaunchOptions is null ||
                    nodeJsService is null)
                {
                    throw new ArgumentNullException($"{nameof(puppeteerLaunchOptions)} or {nameof(nodeJsService)}");
                }

                // Call Node JS to authorise consent in UI via Puppeteer
                object[] args =
                {
                    authUrl,
                    bankProfile.BankProfileEnum.ToString(),
                    "AccountAccessConsent",
                    bankUser,
                    puppeteerLaunchOptions
                };
                await nodeJsService.InvokeFromFileAsync(
                    "authoriseConsent.js",
                    "authoriseConsent",
                    args);

                // Refresh scope to ensure user token acquired following consent is available
                using IRequestBuilderContainer scopedRequestBuilderNew = requestBuilderGenerator();
                IRequestBuilder requestBuilderNew = scopedRequestBuilderNew.RequestBuilder;

                // // GET accounts
                // IFluentResponse<AccountsResponse> accountsResp =
                //     await requestBuilderNew
                //         .AccountAndTransaction
                //         .Accounts
                //         .ReadAsync(accountAccessConsentId);
                //
                // // Checks
                // accountsResp.Should().NotBeNull();
                // accountsResp.Messages.Should().BeEmpty();
                // accountsResp.Data.Should().NotBeNull();

                // GET transactions
                IFluentResponse<TransactionsResponse> transactionsResp =
                    await requestBuilderNew
                        .AccountAndTransaction
                        .Transactions
                        .ReadAsync(accountAccessConsentId);

                // Checks
                transactionsResp.Should().NotBeNull();
                transactionsResp.Messages.Should().BeEmpty();
                transactionsResp.Data.Should().NotBeNull();

                // GET parties
                IFluentResponse<PartiesResponse> partiesResp =
                    await requestBuilderNew
                        .AccountAndTransaction
                        .Parties
                        .ReadAsync(accountAccessConsentId);

                // Checks
                partiesResp.Should().NotBeNull();
                partiesResp.Messages.Should().BeEmpty();
                partiesResp.Data.Should().NotBeNull();

                // GET balances
                IFluentResponse<BalancesResponse> balancesResp =
                    await requestBuilderNew
                        .AccountAndTransaction
                        .Balances
                        .ReadAsync(accountAccessConsentId);

                // Checks
                balancesResp.Should().NotBeNull();
                balancesResp.Messages.Should().BeEmpty();
                balancesResp.Data.Should().NotBeNull();

                // DELETE account access consent
                IFluentResponse accountAccessConsentResp3 = await requestBuilderNew
                    .AccountAndTransaction
                    .AccountAccessConsents
                    .DeleteAsync(accountAccessConsentId);

                // Checks
                accountAccessConsentResp3.Should().NotBeNull();
                accountAccessConsentResp3.Messages.Should().BeEmpty();
            }

            // DELETE auth context
            IFluentResponse authContextResponse3 = await requestBuilder
                .AccountAndTransaction
                .AccountAccessConsents
                .AuthContexts
                .DeleteLocalAsync(authContextId);
            
            // Checks
            authContextResponse3.Should().NotBeNull();
            authContextResponse3.Messages.Should().BeEmpty();
            
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