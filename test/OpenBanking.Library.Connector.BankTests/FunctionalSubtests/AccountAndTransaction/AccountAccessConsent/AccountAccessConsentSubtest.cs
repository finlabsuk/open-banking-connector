// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Web;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using AccountAccessConsentAuthContext =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request.
    AccountAccessConsentAuthContext;
using OBAccount6 = FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.NSwagAisp.Models.OBAccount6;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.AccountAndTransaction.
    AccountAccessConsent;

public class AccountAccessConsentSubtest(
    AccountAndTransactionApiClient accountAndTransactionApiClient,
    AuthContextsApiClient authContextsApiClient)
{
    public static ISet<AccountAccessConsentSubtestEnum> AccountAccessConsentSubtestsSupported(
        BankProfile bankProfile) =>
        bankProfile.AccountAndTransactionApi is null
            ? new HashSet<AccountAccessConsentSubtestEnum>()
            : AccountAccessConsentSubtestHelper.AllAccountAccessConsentSubtests;

    public async Task RunTest(
        AccountAccessConsentSubtestEnum subtestEnum,
        BankProfile bankProfile,
        BankTestData2 testData2,
        Guid bankRegistrationId,
        OAuth2ResponseMode defaultResponseMode,
        Func<IServiceScopeContainer> serviceScopeGenerator,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder aispFluentRequestLogging,
        ConsentAuth? consentAuth,
        string authUrlLeftPart,
        BankUser? bankUser,
        IMemoryCache memoryCache)
    {
        (AccountAccessConsentRequest accountAccessConsentRequest,
                ICollection<AccountAndTransactionModelsPublic.Permissions> requestedPermissions) =
            await GetAccountAccessConsentRequest(
                subtestEnum,
                bankProfile,
                bankRegistrationId,
                testNameUnique,
                modifiedBy,
                aispFluentRequestLogging);

        bool usingReAuth = bankProfile.AccountAndTransactionApiSettings.UseReauth;
        bool haveExistingConsent = testData2.AccountAccessConsentExternalApiId is not null;

        // Create and read fresh AccountAccessConsent
        AccountAccessConsentCreateResponse accountAccessConsentCreateResponseTmp =
            await AccountAccessConsentCreate(accountAccessConsentRequest);

        if (usingReAuth)
        {
            // Delete fresh AccountAccessConsent (includes external API delete)
            await AccountAccessConsentDelete(accountAccessConsentCreateResponseTmp.Id, false);
        }

        // Perform further testing with existing AccountAccessConsent
        // (to avoid creating orphan object at bank if test terminates) unless re-auth not used
        if (!usingReAuth || haveExistingConsent)
        {
            Guid accountAccessConsentId;
            if (usingReAuth)
            {
                // Create and read AccountAccessConsent using existing external API consent
                accountAccessConsentRequest.ExternalApiObject =
                    GetRequiredExternalApiConsent(testData2, modifiedBy);
                AccountAccessConsentCreateResponse accountAccessConsentCreateResponse =
                    await AccountAccessConsentCreate(accountAccessConsentRequest);
                accountAccessConsentId = accountAccessConsentCreateResponse.Id;
            }
            else
            {
                // Continue using fresh consent
                accountAccessConsentId = accountAccessConsentCreateResponseTmp.Id;
            }

            var readForAllAccounts = new AccountAccessConsentExternalReadParams
            {
                ExternalApiAccountId = null,
                QueryString = null,
                ConsentId = accountAccessConsentId,
                ModifiedBy = null,
                ExtraHeaders = null,
                PublicRequestUrlWithoutQuery = null
            };

            // Consent authorisation
            if (consentAuth is not null &&
                testData2.AuthDisable is not true)
            {
                // Create redirect observer which will "catch" redirect
                async Task<AuthContextUpdateAuthResultResponse> ProcessRedirectFcn(TestingAuthResult result)
                {
                    return await authContextsApiClient.RedirectDelegate(result.RedirectParameters);
                }

                async Task<AccountAccessConsentAuthContextCreateResponse>
                    AccountAccessConsentAuthContextCreateResponse()
                {
                    // Create AuthContext
                    var authContextRequest = new AccountAccessConsentAuthContext
                    {
                        AccountAccessConsentId = accountAccessConsentId,
                        Reference = testNameUnique + "_AccountAccessConsent",
                        CreatedBy = modifiedBy
                    };
                    AccountAccessConsentAuthContextCreateResponse accountAccessConsentAuthContextCreateResponse =
                        await AccountAccessConsentAuthContextCreate(authContextRequest);

                    return accountAccessConsentAuthContextCreateResponse;
                }

                var redirectObserver = new RedirectObserver
                {
                    ConsentId = accountAccessConsentId,
                    ConsentType = ConsentType.AccountAccessConsent,
                    ProcessRedirectFcn = ProcessRedirectFcn,
                    AccountAccessConsentAuthContextCreateFcn = AccountAccessConsentAuthContextCreateResponse
                };

                // Determine auth URL
                string authUrl = bankProfile.SupportsSca
                    ? (await AccountAccessConsentAuthContextCreateResponse()).AuthUrl
                    : $"{authUrlLeftPart}/dev1/aisp/account-access-consents/{accountAccessConsentId}/auth";

                // Peform auth
                AuthContextUpdateAuthResultResponse authResultResponse = await consentAuth.PerformAuth(
                    redirectObserver,
                    authUrl,
                    bankProfile.SupportsSca,
                    defaultResponseMode,
                    bankUser,
                    bankProfile.BankProfileEnum,
                    ConsentVariety.AccountAccessConsent);

                bool refreshTokenMayBeAbsent = bankProfile.CustomBehaviour
                    ?.AccountAccessConsentAuthCodeGrantPost
                    ?.ExpectedResponseRefreshTokenMayBeAbsent ?? false;

                {
                    // Refresh scope to ensure user token acquired following consent is available
                    using IServiceScopeContainer scopedServiceScopeNew = serviceScopeGenerator();
                    IRequestBuilder requestBuilderNew = scopedServiceScopeNew.RequestBuilder;

                    // GET /accounts
                    AccountsResponse accountsResp =
                        await requestBuilderNew
                            .AccountAndTransaction
                            .Accounts
                            .ReadAsync(readForAllAccounts);

                    // Checks
                    accountsResp.Should().NotBeNull();
                    accountsResp.Warnings.Should().BeNull();
                    accountsResp.ExternalApiResponse.Should().NotBeNull();

                    if (accountsResp.ExternalApiResponse.Data.Account is not null)
                    {
                        foreach (OBAccount6 account in accountsResp.ExternalApiResponse.Data.Account)
                        {
                            string externalAccountId = account.AccountId;
                            var readForSingleAccount = new AccountAccessConsentExternalReadParams
                            {
                                ExternalApiAccountId = externalAccountId,
                                QueryString = null,
                                ConsentId = accountAccessConsentId,
                                ModifiedBy = null,
                                ExtraHeaders = null,
                                PublicRequestUrlWithoutQuery = null
                            };
                            AccountAndTransactionModelsPublic.OBExternalAccountSubType1Code? accountSubType =
                                account.AccountSubType;

                            // GET /accounts/{accountId}
                            AccountsResponse accountsResp2 =
                                await requestBuilderNew
                                    .AccountAndTransaction
                                    .Accounts
                                    .ReadAsync(readForSingleAccount);

                            // Checks
                            accountsResp2.Should().NotBeNull();
                            accountsResp2.Warnings.Should().BeNull();
                            accountsResp2.ExternalApiResponse.Should().NotBeNull();

                            // GET /accounts/{AccountId}/balances
                            bool testGetBalances =
                                requestedPermissions.Contains(
                                    AccountAndTransactionModelsPublic.Permissions.ReadBalances);
                            if (testGetBalances)
                            {
                                BalancesResponse balancesResp =
                                    await requestBuilderNew
                                        .AccountAndTransaction
                                        .Balances
                                        .ReadAsync(readForSingleAccount);

                                // Checks
                                balancesResp.Should().NotBeNull();
                                balancesResp.Warnings.Should().BeNull();
                                balancesResp.ExternalApiResponse.Should().NotBeNull();
                            }

                            // GET /accounts/{AccountId}/transactions
                            bool hasReadTransactionsBasicOrDetail =
                                requestedPermissions.Contains(
                                    AccountAndTransactionModelsPublic.Permissions.ReadTransactionsBasic) ||
                                requestedPermissions.Contains(
                                    AccountAndTransactionModelsPublic.Permissions.ReadTransactionsDetail);
                            bool hasReadTransactionsCreditsOrDebits =
                                requestedPermissions.Contains(
                                    AccountAndTransactionModelsPublic.Permissions.ReadTransactionsCredits) ||
                                requestedPermissions.Contains(
                                    AccountAndTransactionModelsPublic.Permissions.ReadTransactionsDebits);
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
                                                new TransactionsReadParams
                                                {
                                                    ExternalApiStatementId = null,
                                                    ConsentId = accountAccessConsentId,
                                                    ModifiedBy = null,
                                                    ExtraHeaders = null,
                                                    PublicRequestUrlWithoutQuery = null,
                                                    ExternalApiAccountId = externalAccountId,
                                                    QueryString = queryString
                                                });

                                    // Checks
                                    transactionsResp.Should().NotBeNull();
                                    transactionsResp.Warnings.Should().BeNull();
                                    transactionsResp.ExternalApiResponse.Should().NotBeNull();

                                    // Update query string based on "Next" link
                                    queryString = transactionsResp.ExternalApiResponse.Links?.Next?.Query;
                                    page++;
                                } while (queryString is not null &&
                                         page < maxPages);
                            }

                            // GET /party
                            bool testReadPartyPsu =
                                requestedPermissions.Contains(
                                    AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU);

                            if (testReadPartyPsu)
                            {
                                PartiesResponse partyResp =
                                    await requestBuilderNew
                                        .AccountAndTransaction
                                        .Parties
                                        .ReadAsync(readForAllAccounts);

                                // Checks
                                partyResp.Should().NotBeNull();
                                partyResp.Warnings.Should().BeNull();
                                partyResp.ExternalApiResponse.Should().NotBeNull();
                            }

                            // GET /accounts/{AccountId}/party
                            // GET /accounts/{AccountId}/parties
                            bool testGetParties =
                                requestedPermissions.Contains(AccountAndTransactionModelsPublic.Permissions.ReadParty);
                            if (testGetParties)
                            {
                                if (bankProfile.AccountAndTransactionApiSettings.UseGetPartyEndpoint)
                                {
                                    PartiesResponse partyResp =
                                        await requestBuilderNew
                                            .AccountAndTransaction
                                            .Parties
                                            .ReadAsync(readForSingleAccount);

                                    // Checks
                                    partyResp.Should().NotBeNull();
                                    partyResp.Warnings.Should().BeNull();
                                    partyResp.ExternalApiResponse.Should().NotBeNull();
                                }

                                Parties2Response partiesResp =
                                    await requestBuilderNew
                                        .AccountAndTransaction
                                        .Parties2
                                        .ReadAsync(readForSingleAccount);

                                // Checks
                                partiesResp.Should().NotBeNull();
                                partiesResp.Warnings.Should().BeNull();
                                partiesResp.ExternalApiResponse.Should().NotBeNull();
                            }

                            // GET /accounts/{AccountId}/direct-debits
                            bool testGetDirectDebits =
                                requestedPermissions.Contains(
                                    AccountAndTransactionModelsPublic.Permissions.ReadDirectDebits) &&
                                accountSubType is not AccountAndTransactionModelsPublic.OBExternalAccountSubType1Code
                                    .CreditCard;
                            if (testGetDirectDebits)
                            {
                                DirectDebitsResponse directDebitsResp =
                                    await requestBuilderNew
                                        .AccountAndTransaction
                                        .DirectDebits
                                        .ReadAsync(readForSingleAccount);

                                // Checks
                                directDebitsResp.Should().NotBeNull();
                                directDebitsResp.Warnings.Should().BeNull();
                                directDebitsResp.ExternalApiResponse.Should().NotBeNull();
                            }

                            // GET /accounts/{AccountId}/standing-orders
                            bool testGetStandingOrders =
                                (requestedPermissions.Contains(
                                     AccountAndTransactionModelsPublic.Permissions.ReadStandingOrdersBasic) ||
                                 requestedPermissions.Contains(
                                     AccountAndTransactionModelsPublic.Permissions.ReadStandingOrdersDetail)) &&
                                accountSubType is not AccountAndTransactionModelsPublic.OBExternalAccountSubType1Code
                                    .CreditCard;
                            if (testGetStandingOrders)
                            {
                                StandingOrdersResponse standingOrdersResp =
                                    await requestBuilderNew
                                        .AccountAndTransaction
                                        .StandingOrders
                                        .ReadAsync(readForSingleAccount);

                                // Checks
                                standingOrdersResp.Should().NotBeNull();
                                standingOrdersResp.Warnings.Should().BeNull();
                                standingOrdersResp.ExternalApiResponse.Should().NotBeNull();
                            }

                            if (bankProfile.BankProfileEnum is BankProfileEnum.Monzo_Sandbox
                                or BankProfileEnum.Monzo_Monzo)
                            {
                                MonzoPotsResponse monzoPotsResp =
                                    await requestBuilderNew
                                        .AccountAndTransaction
                                        .MonzoPots
                                        .ReadAsync(readForAllAccounts);

                                // Checks
                                monzoPotsResp.Should().NotBeNull();
                                monzoPotsResp.Warnings.Should().BeNull();
                                monzoPotsResp.ExternalApiResponse.Should().NotBeNull();
                            }
                        }
                    }

                    // If refresh token known available, delete access token 
                    if (!refreshTokenMayBeAbsent)
                    {
                        // Get consent
                        IDbService dbService = scopedServiceScopeNew.DbService;
                        IDbEntityMethods<Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent>
                            consentEntityMethods =
                                dbService
                                    .GetDbEntityMethodsClass<
                                        Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent>();
                        Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent consent =
                            consentEntityMethods
                                .DbSet
                                .Include(o => o.AccountAccessConsentAccessTokensNavigation)
                                .Include(o => o.AccountAccessConsentRefreshTokensNavigation)
                                .SingleOrDefault(x => x.Id == accountAccessConsentId) ??
                            throw new KeyNotFoundException();

                        // Ensure refresh token available
                        AccountAccessConsentRefreshToken _ =
                            consent
                                .AccountAccessConsentRefreshTokensNavigation.SingleOrDefault(x => !x.IsDeleted) ??
                            throw new Exception("Refresh token not found.");

                        // If available, delete cached access token (to force use of refresh token)
                        memoryCache.Remove(consent.GetCacheKey());

                        // If available, delete stored access token (to force use of refresh token) 
                        AccountAccessConsentAccessToken? storedAccessToken =
                            consent
                                .AccountAccessConsentAccessTokensNavigation.SingleOrDefault(x => !x.IsDeleted);
                        if (storedAccessToken is not null)
                        {
                            storedAccessToken.UpdateIsDeleted(true, DateTimeOffset.UtcNow, modifiedBy);
                            await dbService.GetDbSaveChangesMethodClass().SaveChangesAsync();
                        }
                    }
                }

                // Get accounts using refresh token
                if (!refreshTokenMayBeAbsent)
                {
                    // Refresh scope to catch any DB changes
                    using IServiceScopeContainer scopedServiceScopeNew = serviceScopeGenerator();
                    IRequestBuilder requestBuilderNew = scopedServiceScopeNew.RequestBuilder;

                    // GET /accounts
                    AccountsResponse accountsResp =
                        await requestBuilderNew
                            .AccountAndTransaction
                            .Accounts
                            .ReadAsync(readForAllAccounts);
                }
            }

            // Delete AccountAccessConsent (excludes external API delete)
            await AccountAccessConsentDelete(
                accountAccessConsentId,
                usingReAuth);
        }
    }


    private async Task<AccountAccessConsentAuthContextCreateResponse> AccountAccessConsentAuthContextCreate(
        AccountAccessConsentAuthContext authContextRequest)
    {
        AccountAccessConsentAuthContextCreateResponse authContextResponse =
            await accountAndTransactionApiClient.AccountAccessConsentAuthContextCreate(authContextRequest);

        // Checks
        authContextResponse.AuthUrl.Should().NotBeNull();

        // Read AuthContext
        await AccountAccessConsentAuthContextRead(authContextResponse.Id);

        return authContextResponse;
    }

    private async Task<AccountAccessConsentAuthContextReadResponse> AccountAccessConsentAuthContextRead(
        Guid authContextId)
    {
        AccountAccessConsentAuthContextReadResponse authContextResponse =
            await accountAndTransactionApiClient.AccountAccessConsentAuthContextRead(
                new LocalReadParams
                {
                    Id = authContextId,
                    ModifiedBy = null
                });

        return authContextResponse;
    }

    private async Task<AccountAccessConsentCreateResponse> AccountAccessConsentRead(
        Guid accountAccessConsentId,
        bool excludeExternalApiOperation)
    {
        AccountAccessConsentCreateResponse accountAccessConsentReadResponse =
            await accountAndTransactionApiClient.AccountAccessConsentRead(
                new ConsentReadParams
                {
                    Id = accountAccessConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = null,
                    PublicRequestUrlWithoutQuery = null,
                    ExcludeExternalApiOperation = excludeExternalApiOperation
                });

        // Checks
        if (excludeExternalApiOperation)
        {
            accountAccessConsentReadResponse.ExternalApiResponse.Should().BeNull();
        }
        else
        {
            accountAccessConsentReadResponse.ExternalApiResponse.Should().NotBeNull();
        }

        return accountAccessConsentReadResponse;
    }

    private async Task<BaseResponse> AccountAccessConsentDelete(
        Guid accountAccessConsentId,
        bool excludeExternalApiOperation)
    {
        // Delete AccountAccessConsent
        BaseResponse baseResponse = await accountAndTransactionApiClient.AccountAccessConsentDelete(
            new ConsentDeleteParams
            {
                ExtraHeaders = null,
                ExcludeExternalApiOperation = excludeExternalApiOperation,
                Id = accountAccessConsentId,
                ModifiedBy = null
            });

        return baseResponse;
    }

    private async Task<AccountAccessConsentCreateResponse> AccountAccessConsentCreate(
        AccountAccessConsentRequest accountAccessConsentRequest)
    {
        // Create AccountAccessConsent
        AccountAccessConsentCreateResponse accountAccessConsentCreateResponse =
            await accountAndTransactionApiClient.AccountAccessConsentCreate(accountAccessConsentRequest);

        // Checks
        if (accountAccessConsentRequest.ExternalApiObject is not null)
        {
            accountAccessConsentCreateResponse.ExternalApiResponse.Should().BeNull();
        }
        else
        {
            accountAccessConsentCreateResponse.ExternalApiResponse.Should().NotBeNull();
        }

        // Read AccountAccessConsent
        await AccountAccessConsentRead(accountAccessConsentCreateResponse.Id, false);

        return accountAccessConsentCreateResponse;
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
        Task<(AccountAccessConsentRequest accountAccessConsentRequest,
            ICollection<AccountAndTransactionModelsPublic.Permissions> requestedPermissions)>
        GetAccountAccessConsentRequest(
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

        ICollection<AccountAndTransactionModelsPublic.Permissions> requestedPermissions =
            accountAccessConsentRequest.ExternalApiRequest.Data.Permissions;
        return (accountAccessConsentRequest, requestedPermissions);
    }
}
