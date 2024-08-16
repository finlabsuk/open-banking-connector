// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost;
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
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder aispFluentRequestLogging,
        ConsentAuth? consentAuth,
        string authUrlLeftPart,
        BankUser? bankUser,
        IServiceProvider appServiceProvider,
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

        // Create fresh AccountAccessConsent
        AccountAccessConsentCreateResponse accountAccessConsentCreateResponseTmp =
            await accountAndTransactionApiClient.AccountAccessConsentCreate(accountAccessConsentRequest);

        // Read AccountAccessConsent
        AccountAccessConsentCreateResponse accountAccessConsentReadResponse =
            await accountAndTransactionApiClient.AccountAccessConsentRead(
                new ConsentReadParams
                {
                    Id = accountAccessConsentCreateResponseTmp.Id,
                    ModifiedBy = null,
                    ExtraHeaders = null,
                    PublicRequestUrlWithoutQuery = null,
                    ExcludeExternalApiOperation = false
                });

        if (usingReAuth)
        {
            // Delete fresh AccountAccessConsent (includes external API delete)
            BaseResponse baseResponse = await accountAndTransactionApiClient.AccountAccessConsentDelete(
                new ConsentDeleteParams
                {
                    ExtraHeaders = null,
                    ExcludeExternalApiOperation = false,
                    Id = accountAccessConsentCreateResponseTmp.Id,
                    ModifiedBy = null
                });
        }

        // Perform further testing with existing AccountAccessConsent
        // (to avoid creating orphan object at bank if test terminates) unless re-auth not used
        if (!usingReAuth || haveExistingConsent)
        {
            Guid accountAccessConsentId;
            if (usingReAuth)
            {
                // Create AccountAccessConsent using existing external API consent
                accountAccessConsentRequest.ExternalApiObject =
                    GetRequiredExternalApiConsent(testData2, modifiedBy);
                AccountAccessConsentCreateResponse accountAccessConsentCreateResponse =
                    await accountAndTransactionApiClient.AccountAccessConsentCreate(accountAccessConsentRequest);

                // Read AccountAccessConsent
                AccountAccessConsentCreateResponse accountAccessConsentReadResponse1 =
                    await accountAndTransactionApiClient.AccountAccessConsentRead(
                        new ConsentReadParams
                        {
                            Id = accountAccessConsentCreateResponse.Id,
                            ModifiedBy = null,
                            ExtraHeaders = null,
                            PublicRequestUrlWithoutQuery = null,
                            ExcludeExternalApiOperation = false
                        });
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
                        await accountAndTransactionApiClient.AccountAccessConsentAuthContextCreate(authContextRequest);

                    // Read AuthContext
                    AccountAccessConsentAuthContextReadResponse accountAccessConsentAuthContextReadResponse =
                        await accountAndTransactionApiClient.AccountAccessConsentAuthContextRead(
                            new LocalReadParams
                            {
                                Id = accountAccessConsentAuthContextCreateResponse.Id,
                                ModifiedBy = null
                            });

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
                string authUrl;
                if (bankProfile.SupportsSca)
                {
                    AccountAccessConsentAuthContextCreateResponse authContext =
                        await AccountAccessConsentAuthContextCreateResponse();
                    authUrl = authContext.AuthUrl;
                    redirectObserver.AssociatedStates.Add(authContext.State);
                }
                else
                {
                    authUrl = $"{authUrlLeftPart}/dev1/aisp/account-access-consents/{accountAccessConsentId}/auth";
                }

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
                    // GET /accounts
                    AccountsResponse accountsResp =
                        await accountAndTransactionApiClient.AccountsRead(readForAllAccounts);

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
                                await accountAndTransactionApiClient.AccountsRead(readForSingleAccount);

                            // GET /accounts/{AccountId}/balances
                            bool testGetBalances =
                                requestedPermissions.Contains(
                                    AccountAndTransactionModelsPublic.Permissions.ReadBalances);
                            if (testGetBalances)
                            {
                                BalancesResponse balancesResp =
                                    await accountAndTransactionApiClient.BalancesRead(readForSingleAccount);
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
                                        await accountAndTransactionApiClient.TransactionsRead(
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
                                    await accountAndTransactionApiClient.PartiesRead(readForAllAccounts);
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
                                        await accountAndTransactionApiClient.PartiesRead(readForSingleAccount);
                                }

                                Parties2Response partiesResp =
                                    await accountAndTransactionApiClient.Parties2Read(readForSingleAccount);
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
                                    await accountAndTransactionApiClient.DirectDebitsRead(readForSingleAccount);
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
                                    await accountAndTransactionApiClient.StandingOrdersRead(readForSingleAccount);
                            }

                            if (bankProfile.BankProfileEnum is BankProfileEnum.Monzo_Sandbox
                                or BankProfileEnum.Monzo_Monzo)
                            {
                                MonzoPotsResponse monzoPotsResp =
                                    await accountAndTransactionApiClient.MonzoPotsRead(readForAllAccounts);
                            }
                        }
                    }

                    // If refresh token known available, delete access token 
                    if (!refreshTokenMayBeAbsent)
                    {
                        // Get new application services scope
                        using IServiceScopeContainer serviceScopeContainer =
                            new ServiceScopeFromDependencyInjection(appServiceProvider);

                        // Get consent
                        IDbService dbService = serviceScopeContainer.DbService;
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
                                .AsSplitQuery()
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
                    // GET /accounts
                    AccountsResponse accountsResp =
                        await accountAndTransactionApiClient.AccountsRead(readForAllAccounts);
                }
            }

            // Delete AccountAccessConsent (excludes external API delete)
            BaseResponse baseResponse = await accountAndTransactionApiClient.AccountAccessConsentDelete(
                new ConsentDeleteParams
                {
                    ExtraHeaders = null,
                    ExcludeExternalApiOperation = usingReAuth,
                    Id = accountAccessConsentId,
                    ModifiedBy = null
                });
            BaseResponse temp = baseResponse;
        }
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
