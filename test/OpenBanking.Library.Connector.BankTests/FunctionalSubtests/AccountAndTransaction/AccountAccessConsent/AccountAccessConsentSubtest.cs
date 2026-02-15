// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.AccountAndTransaction;
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
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using AccountAccessConsentAuthContext =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request.
    AccountAccessConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.AccountAndTransaction.
    AccountAccessConsent;

public class AccountAccessConsentSubtest(
    AccountAndTransactionApiClient accountAndTransactionApiClient,
    AuthContextsApiClient authContextsApiClient)
{
    public static ISet<AccountAccessConsentSubtestEnum> AccountAccessConsentSubtestsSupported(
        BankProfile bankProfile) =>
        AccountAccessConsentSubtestHelper.AllAccountAccessConsentSubtests;

    public async Task RunTest(
        AccountAccessConsentSubtestEnum subtestEnum,
        BankProfile bankProfile,
        BankTestData testData,
        Guid bankRegistrationId,
        OAuth2ResponseMode defaultResponseMode,
        bool testAuth,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder aispFluentRequestLogging,
        ConsentAuth consentAuth,
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

        bool useReauth = bankProfile.AccountAndTransactionApiSettings.UseReauth;

        // Create fresh AccountAccessConsent
        AccountAccessConsentCreateResponse accountAccessConsentCreateResponseTmp =
            await accountAndTransactionApiClient.AccountAccessConsentCreate(accountAccessConsentRequest);

        // Read fresh AccountAccessConsent
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

        // Delete fresh AccountAccessConsent unless using for auth (where re-auth not supported)
        if (!testAuth || useReauth)
        {
            BaseResponse baseResponse = await accountAndTransactionApiClient.AccountAccessConsentDelete(
                new ConsentDeleteParams
                {
                    ExtraHeaders = null,
                    ExcludeExternalApiOperation = false,
                    Id = accountAccessConsentCreateResponseTmp.Id,
                    ModifiedBy = null
                });
        }

        // Perform testing which requires auth
        if (testAuth)
        {
            Guid accountAccessConsentId;
            string? accountAccessConsentExternalApiId = testData.AccountAccessConsentExternalApiId;
            if (useReauth)
            {
                // Create AccountAccessConsent using existing external API consent
                accountAccessConsentRequest.ExternalApiObject =
                    new ExternalApiConsent
                    {
                        ExternalApiId =
                            accountAccessConsentExternalApiId ??
                            throw new InvalidOperationException(
                                "No AccountAccessConsent external API ID provided for use in re-auth."),
                        AuthContext = testData.AccountAccessConsentAuthContextNonce is null
                            ? null
                            : new AuthContextRequest
                            {
                                State = "",
                                Nonce = testData.AccountAccessConsentAuthContextNonce ?? "",
                                ModifiedBy = modifiedBy
                            }
                    };
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
                if (accountAccessConsentExternalApiId is not null)
                {
                    throw new InvalidOperationException("External API ID provided but re-auth not supported.");
                }

                // Continue using fresh consent
                accountAccessConsentId = accountAccessConsentCreateResponseTmp.Id;
            }

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

            var readForAllAccounts = new AccountAccessConsentExternalReadParams
            {
                ExternalApiAccountId = null,
                QueryString = null,
                ConsentId = accountAccessConsentId,
                ModifiedBy = null,
                ExtraHeaders = null,
                PublicRequestUrlWithoutQuery = null
            };

            // GET /accounts
            AccountsResponse accountsResp =
                await accountAndTransactionApiClient.AccountsRead(readForAllAccounts);

            if (accountsResp.ExternalApiResponse.Data.Account is not null)
            {
                foreach (AccountAndTransactionModelsPublic.OBAccount6 account in accountsResp.ExternalApiResponse.Data
                             .Account)
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
                    bool accountIsCreditCardType =
                        (account.V3AccountSubType is not null &&
                         account.V3AccountSubType is AccountAndTransactionModelsV3p1p11.OBExternalAccountSubType1Code
                             .CreditCard) ||
                        (account.AccountTypeCode is not null &&
                         account.AccountTypeCode is AccountAndTransactionModelsPublic.OBExternalAccountSubType1CodeV4
                             .CARD);

                    // GET /accounts/{accountId}
                    AccountsResponse accountsResp2 =
                        await accountAndTransactionApiClient.AccountsRead(readForSingleAccount);

                    // GET /accounts/{AccountId}/balances
                    bool testGetBalances =
                        requestedPermissions.Contains(AccountAndTransactionModelsPublic.Permissions.ReadBalances);
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
                        requestedPermissions.Contains(AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU);

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

                        if (bankProfile.AccountAndTransactionApiSettings.UseGetParty2Endpoint)
                        {
                            Parties2Response partiesResp =
                                await accountAndTransactionApiClient.Parties2Read(readForSingleAccount);
                        }
                    }

                    // GET /accounts/{AccountId}/direct-debits
                    bool testGetDirectDebits =
                        requestedPermissions.Contains(AccountAndTransactionModelsPublic.Permissions.ReadDirectDebits) &&
                        !accountIsCreditCardType;
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
                        !accountIsCreditCardType;
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

            // If refresh token known available, delete access token to check refresh token grant
            bool refreshTokenMayBeAbsent = bankProfile.CustomBehaviour
                ?.AccountAccessConsentAuthCodeGrantPost
                ?.ExpectedResponseRefreshTokenMayBeAbsent ?? false;
            if (!refreshTokenMayBeAbsent)
            {
                {
                    // Get new application services scope
                    using IServiceScopeContainer serviceScopeContainer =
                        new ServiceScopeFromDependencyInjection(appServiceProvider);

                    // Get consent
                    IDbService dbService = serviceScopeContainer.DbService;
                    IDbMethods dbMethods = dbService.GetDbMethods();
                    IDbEntityMethods<Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent>
                        consentEntityMethods =
                            dbService
                                .GetDbEntityMethods<
                                    Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent>();
                    IDbEntityMethods<AccountAccessConsentAccessToken> accessTokenMethods =
                        dbService.GetDbEntityMethods<AccountAccessConsentAccessToken>();
                    IDbEntityMethods<AccountAccessConsentRefreshToken> refreshTokenMethods =
                        dbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>();

                    Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent consent;
                    AccountAccessConsentAccessToken? storedAccessToken;
                    if (dbMethods.DbProvider is not DbProvider.MongoDb)
                    {
                        consent =
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

                        storedAccessToken = consent
                            .AccountAccessConsentAccessTokensNavigation.SingleOrDefault(x => !x.IsDeleted);
                    }
                    else
                    {
                        consent =
                            consentEntityMethods
                                .DbSet
                                .SingleOrDefault(x => x.Id == accountAccessConsentId) ??
                            throw new KeyNotFoundException();

                        // Ensure refresh token available
                        AccountAccessConsentRefreshToken unused2 =
                            await refreshTokenMethods
                                .DbSetNoTracking
                                .Where(x => EF.Property<string>(x, "_t") == nameof(AccountAccessConsentRefreshToken))
                                .SingleOrDefaultAsync(x => x.AccountAccessConsentId == consent.Id && !x.IsDeleted) ??
                            throw new Exception("Refresh token not found.");

                        storedAccessToken =
                            await accessTokenMethods
                                .DbSet
                                .Where(x => EF.Property<string>(x, "_t") == nameof(AccountAccessConsentAccessToken))
                                .SingleOrDefaultAsync(x => x.AccountAccessConsentId == consent.Id && !x.IsDeleted);
                    }

                    // If available, delete cached access token (to force use of refresh token)
                    memoryCache.Remove(consent.GetCacheKey());

                    // If available, delete stored access token (to force use of refresh token) 
                    if (storedAccessToken is not null)
                    {
                        storedAccessToken.UpdateIsDeleted(true, DateTimeOffset.UtcNow, modifiedBy);
                        await dbService.GetDbMethods().SaveChangesAsync();
                    }
                }

                // GET /accounts
                AccountsResponse accountsResponse2 =
                    await accountAndTransactionApiClient.AccountsRead(readForAllAccounts);
            }

            // Delete AccountAccessConsent (excludes external API delete)
            BaseResponse baseResponse = await accountAndTransactionApiClient.AccountAccessConsentDelete(
                new ConsentDeleteParams
                {
                    ExtraHeaders = null,
                    ExcludeExternalApiOperation = useReauth,
                    Id = accountAccessConsentId,
                    ModifiedBy = null
                });
            BaseResponse temp = baseResponse;
        }
    }

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

        // Resolve external API request
        accountAccessConsentRequest.ExternalApiRequest
            ??= AccountAccessTemplates.AccountAccessConsentExternalApiRequest(
                accountAccessConsentRequest.TemplateRequest?.Type ??
                throw new InvalidOperationException(
                    "Both ExternalApiRequest and TemplateRequest specified as null so not possible to create external API request."),
                bankProfile.AccountAndTransactionApiSettings);
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
