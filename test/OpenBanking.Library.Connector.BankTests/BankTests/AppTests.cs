// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.AccountAndTransaction.
    AccountAccessConsent;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.DomesticVrp;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;
using ObSealCertificateRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request.ObSealCertificate;
using ObWacCertificateRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request.ObWacCertificate;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public abstract class AppTests
{
    public enum AccountAccessConsentOptions
    {
        OnlyCreateConsent,
        TestConsent,
        OnlyDeleteConsent
    }

    public enum BankRegistrationOptions
    {
        /// <summary>
        ///     Test creation of external bank registration.
        /// </summary>
        OnlyCreateRegistration,

        /// <summary>
        ///     Test deletion of existing external bank registration.
        ///     If possible, force bank registration deletion at external (bank) API.
        /// </summary>
        OnlyDeleteRegistration,

        /// <summary>
        ///     Test all possible endpoints (maximal test) using existing external bank registration.
        /// </summary>
        TestRegistration
    }

    private readonly AppContextFixture _appContextFixture;
    private readonly ManagementApiClient _managementApiClient;
    private readonly ITestOutputHelper _outputHelper;
    protected readonly IServiceProvider _serviceProvider;
    private readonly WebAppClient _webAppClient;

    protected AppTests(
        ITestOutputHelper outputHelper,
        AppContextFixture appContextFixture,
        WebApplicationFactory<Program> webApplicationFactory)
    {
        _outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
        _serviceProvider = appContextFixture.Host.Services;
        _appContextFixture = appContextFixture;
        _webAppClient = new WebAppClient(webApplicationFactory);
        _managementApiClient = new ManagementApiClient(_webAppClient);
    }

    public static TheoryData<BankTestData1, BankTestData2>
        TestedSkippedBanksById(bool genericAppNotPlainAppTest) =>
        TestedBanksById(true, genericAppNotPlainAppTest);

    public static TheoryData<BankTestData1, BankTestData2>
        TestedUnskippedBanksById(bool genericAppNotPlainAppTest) =>
        TestedBanksById(false, genericAppNotPlainAppTest);

    public static TheoryData<BankTestData1, BankTestData2> TestedBanksById(
        bool skippedNotUnskipped,
        bool genericAppNotPlainAppTest)
    {
        // Get bank test settings
        var bankTestSettings = AppConfiguration.GetSettings<BankTestSettings>();
        //var env = AppConfiguration.EnvironmentName;

        // Get bank profile definitions
        var data =
            new TheoryData<BankTestData1, BankTestData2>();

        // Loop through test groups
        foreach ((string groupName, TestGroup testGroup) in bankTestSettings.TestGroups)
        {
            List<BankProfileEnum> testedBanks = genericAppNotPlainAppTest
                ? testGroup.GenericHostAppTests
                : testGroup.PlainAppTests;

            // Loop through tested banks
            foreach (BankProfileEnum bankProfileEnum in testedBanks)
            {
                // Get override for software statement and certificate profiles
                testGroup
                    .SoftwareStatementAndCertificateProfileOverrides
                    .TryGetValue(bankProfileEnum, out string? overrideCase);

                // Get external API BankRegistration ID
                testGroup
                    .BankRegistrationExternalApiIds
                    .TryGetValue(bankProfileEnum, out string? bankRegistrationExternalApiId);

                // Get external API BankRegistration secret
                testGroup
                    .BankRegistrationExternalApiSecrets
                    .TryGetValue(bankProfileEnum, out string? bankRegistrationExternalApiSecret);

                // Get external API BankRegistration registration access token
                testGroup
                    .BankRegistrationRegistrationAccessTokens
                    .TryGetValue(bankProfileEnum, out string? bankRegistrationRegistrationAccessToken);

                // Get external API AccountAccessConsent ID
                testGroup
                    .AccountAccessConsentExternalApiIds
                    .TryGetValue(bankProfileEnum, out string? accountAccessConsentExternalApiId);

                // Get external API AccountAccessConsent refresh token
                testGroup
                    .AccountAccessConsentRefreshTokens
                    .TryGetValue(bankProfileEnum, out string? accountAccessConsentRefreshToken);

                // Get external API AccountAccessConsent refresh token
                testGroup
                    .AccountAccessConsentAuthContextNonces
                    .TryGetValue(bankProfileEnum, out string? accountAccessConsentAuthContextNonce);

                // Get consent auth data (for sandboxes)
                bankTestSettings
                    .AuthData
                    .TryGetValue(bankProfileEnum, out AuthData? authData);

                bool? authDisable = authData?.DisableAuth;
                string? authUiInputUserName = authData?.UiInput?.UserName; // can only be null when UiInput null
                string? authUiInputPassword = authData?.UiInput?.Password; // can only be null when UiInput null

                // Add test case to theory data if skip status matches that of theory data
                if (!skippedNotUnskipped)
                {
                    data.Add(
                        new BankTestData1
                        {
                            TestGroupName = groupName,
                            SoftwareStatementProfileId = testGroup.SoftwareStatementProfileId,
                            SoftwareStatementAndCertificateProfileOverride = overrideCase
                        },
                        new BankTestData2
                        {
                            BankProfileEnum = bankProfileEnum,
                            BankRegistrationExternalApiId = bankRegistrationExternalApiId,
                            BankRegistrationExternalApiSecret = bankRegistrationExternalApiSecret,
                            BankRegistrationRegistrationAccessToken =
                                bankRegistrationRegistrationAccessToken,
                            AccountAccessConsentExternalApiId = accountAccessConsentExternalApiId,
                            AccountAccessConsentAuthContextNonce = accountAccessConsentAuthContextNonce,
                            RegistrationScope = testGroup.RegistrationScope,
                            AuthDisable = authDisable,
                            AuthUiInputUserName = authUiInputUserName,
                            AuthUiInputPassword = authUiInputPassword
                        });
                }
            }
        }

        return data;
    }

    protected async Task TestAllInner(
        BankTestData1 testData1,
        BankTestData2 testData2,
        Func<IServiceScopeContainer> serviceScopeGenerator,
        bool genericNotPlainAppTest)
    {
        // Set test name
        var testName =
            $"{testData2.BankProfileEnum}_{testData1.SoftwareStatementProfileId}_{testData2.RegistrationScope.AbbreviatedName()}";
        var testNameUnique = $"{testName}_{Guid.NewGuid()}";

        // Set test options
        var accountAccessConsentOptions = AccountAccessConsentOptions.TestConsent;

        // Get bank test settings
        BankTestSettings bankTestSettings =
            _serviceProvider.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();

        // Get logger
        var instrumentationClient = _serviceProvider.GetRequiredService<IInstrumentationClient>();

        // Get bank profile definitions
        var bankProfileDefinitions =
            _serviceProvider.GetRequiredService<IBankProfileService>();
        BankProfile bankProfile =
            bankProfileDefinitions.GetBankProfile(testData2.BankProfileEnum);

        // Get bank user
        BankUser? bankUser = testData2.AuthUiInputUserName is not null
            ? new BankUser(
                testData2.AuthUiInputUserName,
                testData2.AuthUiInputPassword!, // not null when AuthUiInputUserName not null
                new List<Account>(),
                new List<DomesticVrpAccountIndexPair>())
            : null;

        // Get software statement profile
        var processedSoftwareStatementProfileStore =
            _serviceProvider.GetRequiredService<IProcessedSoftwareStatementProfileStore>();
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await processedSoftwareStatementProfileStore.GetAsync(
                testData1.SoftwareStatementProfileId,
                testData1.SoftwareStatementAndCertificateProfileOverride);

        // Get memory cache
        var memoryCache = _serviceProvider.GetRequiredService<IMemoryCache>();

        // Create test data writers
        string topLevelFolderName = genericNotPlainAppTest ? "genericAppTests" : "plainAppTests";
        var testDataProcessorFluentRequestLogging = new FilePathBuilder(
            Path.Combine(bankTestSettings.GetDataDirectoryForCurrentOs(), $"{topLevelFolderName}/fluent"),
            testName,
            ".json");

        FilePathBuilder? testDataProcessorApiLogging = null;
        if (bankTestSettings.LogExternalApiData)
        {
            testDataProcessorApiLogging = new FilePathBuilder(
                Path.Combine(bankTestSettings.GetDataDirectoryForCurrentOs(), $"{topLevelFolderName}/api"),
                testName,
                ".json");
        }

        // Create consent auth if in use
        ConsentAuth? consentAuth;
        bool useConsentAuth = genericNotPlainAppTest;
        if (useConsentAuth)
        {
            PlaywrightLaunchOptions launchOptions =
                bankTestSettings.Auth.PlaywrightLaunch;

            if (launchOptions is null)
            {
                throw new ArgumentNullException($"{nameof(launchOptions)}");
            }

            var browserTypeLaunchOptions = new BrowserTypeLaunchOptions
            {
                Args = launchOptions.ProcessedArgs,
                ExecutablePath = launchOptions.ProcessedExecutablePath,
                Headless = launchOptions.Headless,
                SlowMo = launchOptions.ProcessedSlowMo,
                Timeout = launchOptions.TimeOut
            };

            EmailOptions emailOptions = bankTestSettings.Auth.Email;

            consentAuth = new ConsentAuth(browserTypeLaunchOptions, emailOptions, bankProfileDefinitions);
        }
        else
        {
            consentAuth = null;
        }

        var modifiedBy = "Automated bank tests";

        // Create software statement
        (Guid obWacCertificateId, Guid obSealCertificateId, Guid softwareStatementId) = await CreateSoftwareStatement(
            processedSoftwareStatementProfile,
            modifiedBy);

        // CREATE and READ bank configuration objects
        // Create bankRegistration or use existing
        RegistrationScopeEnum registrationScope = testData2.RegistrationScope;
        BankRegistration bankRegistrationRequest = await GetBankRegistrationRequest(
            bankProfile,
            softwareStatementId,
            registrationScope,
            testDataProcessorFluentRequestLogging,
            testNameUnique,
            modifiedBy);

        if (bankProfile.BankConfigurationApiSettings.TestTemporaryBankRegistration)
        {
            // Create fresh BankRegistration
            (Guid bankRegistrationIdTmp, _) =
                await CreateBankRegistration(bankRegistrationRequest);

            // Delete BankRegistration (includes external API delete as appropriate)
            await DeleteBankRegistration(serviceScopeGenerator, bankRegistrationIdTmp, modifiedBy, false);
        }

        // Create BankRegistration using existing external API registration
        bankRegistrationRequest.ExternalApiId =
            testData2.BankRegistrationExternalApiId ??
            throw new InvalidOperationException("No external API BankRegistration ID provided.");
        bankRegistrationRequest.ExternalApiSecret = testData2.BankRegistrationExternalApiSecret;
        bankRegistrationRequest.RegistrationAccessToken = testData2.BankRegistrationRegistrationAccessToken;
        (Guid bankRegistrationId, OAuth2ResponseMode? defaultResponseModeOverride) =
            await CreateBankRegistration(bankRegistrationRequest);
        OAuth2ResponseMode defaultResponseMode = defaultResponseModeOverride ?? bankProfile.DefaultResponseMode;

        // Read BankRegistration
        BankRegistrationResponse bankRegistrationReadResponse =
            await _managementApiClient.BankRegistrationRead(bankRegistrationId);

        // Run account access consent subtests
        string redirectUri = processedSoftwareStatementProfile.GetRedirectUri(
            defaultResponseMode,
            bankRegistrationReadResponse.DefaultFragmentRedirectUri,
            bankRegistrationReadResponse.DefaultQueryRedirectUri);
        string authUrlLeftPart =
            new Uri(redirectUri)
                .GetLeftPart(UriPartial.Authority);
        if (registrationScope.HasFlag(RegistrationScopeEnum.AccountAndTransaction))
        {
            foreach (AccountAccessConsentSubtestEnum subTest in
                     AccountAccessConsentSubtest.AccountAccessConsentSubtestsSupported(bankProfile))
            {
                await AccountAccessConsentSubtest.RunTest(
                    subTest,
                    bankProfile,
                    testData2,
                    bankRegistrationId,
                    defaultResponseMode,
                    serviceScopeGenerator,
                    testNameUnique,
                    modifiedBy,
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("aisp")
                        .AppendToPath($"{subTest.ToString()}"),
                    consentAuth,
                    authUrlLeftPart,
                    bankUser,
                    memoryCache,
                    accountAccessConsentOptions);
            }
        }

        // Run domestic payment consent subtests
        if (registrationScope.HasFlag(RegistrationScopeEnum.PaymentInitiation))
        {
            foreach (DomesticPaymentSubtestEnum subTest in
                     DomesticPaymentSubtest.DomesticPaymentFunctionalSubtestsSupported(bankProfile))
            {
                await DomesticPaymentSubtest.RunTest(
                    subTest,
                    bankProfile,
                    bankRegistrationId,
                    defaultResponseMode,
                    serviceScopeGenerator,
                    testNameUnique,
                    modifiedBy,
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("pisp")
                        .AppendToPath($"{subTest.ToString()}"),
                    consentAuth,
                    bankUser);
            }

            // Run domestic VRP consent subtests
            foreach (DomesticVrpSubtestEnum subTest in
                     DomesticVrpSubtest.DomesticVrpFunctionalSubtestsSupported(bankProfile))
            {
                await DomesticVrpSubtest.RunTest(
                    subTest,
                    bankProfile,
                    bankRegistrationId,
                    defaultResponseMode,
                    serviceScopeGenerator,
                    testNameUnique,
                    modifiedBy,
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("vrp")
                        .AppendToPath($"{subTest.ToString()}"),
                    consentAuth,
                    bankUser);
            }
        }

        // Delete BankRegistration (excludes external API delete)
        await DeleteBankRegistration(serviceScopeGenerator, bankRegistrationId, modifiedBy, true);

        await DeleteSoftwareStatement(
            obWacCertificateId,
            obSealCertificateId,
            softwareStatementId);
    }


    private async Task DeleteSoftwareStatement(
        Guid obWacCertificateId,
        Guid obSealCertificateId,
        Guid softwareStatementId)
    {
        _ = await _managementApiClient.ObWacCertificateDelete(obWacCertificateId);

        _ = await _managementApiClient.ObSealCertificateDelete(obSealCertificateId);

        _ = await _managementApiClient.SoftwareStatementDelete(softwareStatementId);
    }

    private async Task<(Guid obWacCertificateId, Guid obSealCertificateId, Guid softwareStatementId)>
        CreateSoftwareStatement(
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            string createdBy)
    {
        // Create OBWAC certificate
        string obWacReference = processedSoftwareStatementProfile.TransportCertificateId;
        var obWacRequest = new ObWacCertificateRequest
        {
            Reference = obWacReference,
            CreatedBy = createdBy,
            AssociatedKey = new SecretDescription
            {
                Name =
                    $"OpenBankingConnector:TransportCertificateProfiles:{obWacReference}:AssociatedKey"
            },
            Certificate = processedSoftwareStatementProfile.TransportCertificate
        };
        ObWacCertificateResponse obWacCertificateResponse =
            await _managementApiClient.ObWacCertificateCreate(obWacRequest);
        Guid obWacCertificateId = obWacCertificateResponse.Id;

        // Read OBWAC certificate
        _ = await _managementApiClient.ObWacCertificateRead(obWacCertificateId);

        // Create OBSeal certificate
        string obSealReference = processedSoftwareStatementProfile.SigningCertificateId;
        var obSealRequest = new ObSealCertificateRequest
        {
            Reference = obSealReference,
            CreatedBy = createdBy,
            AssociatedKeyId = processedSoftwareStatementProfile.OBSealKey.KeyId,
            AssociatedKey = new SecretDescription
            {
                Name =
                    $"OpenBankingConnector:SigningCertificateProfiles:{obSealReference}:AssociatedKey"
            },
            Certificate = processedSoftwareStatementProfile.SigningCertificate
        };
        ObSealCertificateResponse obSealCertificateResponse =
            await _managementApiClient.ObSealCertificateCreate(obSealRequest);
        Guid obSealCertificateId = obSealCertificateResponse.Id;

        // Read OBSeal certificate
        _ = await _managementApiClient.ObSealCertificateRead(obSealCertificateId);

        // Create software statement
        string sReference = processedSoftwareStatementProfile.Id;
        var softwareStatementRequest = new SoftwareStatement
        {
            Reference = sReference,
            CreatedBy = createdBy,
            OrganisationId = processedSoftwareStatementProfile.OrganisationId,
            SoftwareId = processedSoftwareStatementProfile.SoftwareId,
            SandboxEnvironment = processedSoftwareStatementProfile.SandboxEnvironment,
            DefaultObWacCertificateId = obWacCertificateResponse.Id,
            DefaultObSealCertificateId = obSealCertificateResponse.Id,
            DefaultQueryRedirectUrl = processedSoftwareStatementProfile.DefaultQueryRedirectUrl,
            DefaultFragmentRedirectUrl = processedSoftwareStatementProfile.DefaultFragmentRedirectUrl
        };
        SoftwareStatementResponse softwareStatementResponse =
            await _managementApiClient.SoftwareStatementCreate(softwareStatementRequest);
        Guid softwareStatementId = softwareStatementResponse.Id;

        // Read software statement
        _ = await _managementApiClient.SoftwareStatementRead(softwareStatementId);

        return (obWacCertificateId, obSealCertificateId, softwareStatementId);
    }

    private async Task<(Guid bankRegistrationId, OAuth2ResponseMode? defaultResponseModeOverride)>
        CreateBankRegistration(BankRegistration bankRegistrationRequest)
    {
        BankRegistrationResponse registrationResp =
            await _managementApiClient.BankRegistrationCreate(bankRegistrationRequest);

        // Checks and assignments
        if (bankRegistrationRequest.ExternalApiId is not null)
        {
            registrationResp.ExternalApiResponse.Should().BeNull();
        }
        else
        {
            registrationResp.ExternalApiResponse.Should().NotBeNull();
        }

        Guid bankRegistrationId = registrationResp.Id;
        OAuth2ResponseMode? defaultResponseModeOverride = registrationResp.DefaultResponseModeOverride;

        return (bankRegistrationId, defaultResponseModeOverride);
    }

    private static async Task DeleteBankRegistration(
        Func<IServiceScopeContainer> serviceScopeGenerator,
        Guid bankRegistrationId,
        string modifiedBy,
        bool excludeExternalApiOperation)
    {
        // Get request builder
        using IServiceScopeContainer serviceScopeContainer = serviceScopeGenerator();
        IRequestBuilder requestBuilder = serviceScopeContainer.RequestBuilder;

        BaseResponse bankRegistrationDeleteResponse = await requestBuilder
            .Management
            .BankRegistrations
            .DeleteAsync(
                new BankRegistrationDeleteParams
                {
                    ExcludeExternalApiOperation = excludeExternalApiOperation,
                    Id = bankRegistrationId,
                    ModifiedBy = null
                });

        // Checks
        bankRegistrationDeleteResponse.Should().NotBeNull();
        bankRegistrationDeleteResponse.Warnings.Should().BeNull();
    }

    private static async Task<BankRegistration> GetBankRegistrationRequest(
        BankProfile bankProfile,
        Guid softwareStatementId,
        RegistrationScopeEnum registrationScope,
        FilePathBuilder testDataProcessorFluentRequestLogging,
        string testNameUnique,
        string modifiedBy)
    {
        var registrationRequest = new BankRegistration
        {
            BankProfile = bankProfile.BankProfileEnum,
            SoftwareStatementId = default, // substitute logging placeholder
            RegistrationScope = registrationScope
        };
        await testDataProcessorFluentRequestLogging
            .AppendToPath("manage")
            .AppendToPath("bankRegistration")
            .AppendToPath("postRequest")
            .WriteFile(registrationRequest);
        registrationRequest.SoftwareStatementId = softwareStatementId;
        registrationRequest.Reference = testNameUnique;
        registrationRequest.CreatedBy = modifiedBy;
        return registrationRequest;
    }

    protected void SetTestLogging()
    {
        _appContextFixture.OutputHelper = _outputHelper;
    }

    protected void UnsetTestLogging()
    {
        _appContextFixture.OutputHelper = null;
    }
}
