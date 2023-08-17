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
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

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
    private readonly ITestOutputHelper _outputHelper;
    protected readonly IServiceProvider _serviceProvider;

    protected AppTests(ITestOutputHelper outputHelper, AppContextFixture appContextFixture)
    {
        _outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
        _serviceProvider = appContextFixture.Host.Services;
        _appContextFixture = appContextFixture;
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
        var bankProfilesSettings = AppConfiguration.GetSettings<BankProfilesSettings>();
        var bankProfilesSettingsProvider =
            new DefaultSettingsProvider<BankProfilesSettings>(bankProfilesSettings);
        var bankProfileDefinitions =
            new BankProfileService(bankProfilesSettingsProvider);
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

                // Determine whether test case should be skipped based on registration scope
                BankProfile bankProfile = bankProfileDefinitions.GetBankProfile(bankProfileEnum);
                bool registrationScopeValid =
                    bankProfile.BankConfigurationApiSettings.RegistrationScopeIsValid(testGroup.RegistrationScope);
                bool testCaseShouldBeSkipped = !registrationScopeValid;

                // Add test case to theory data if skip status matches that of theory data
                if (testCaseShouldBeSkipped == skippedNotUnskipped)
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
        Func<IRequestBuilderContainer> requestBuilderGenerator,
        bool genericNotPlainAppTest)
    {
        // Set test name
        var testName =
            $"{testData2.BankProfileEnum}_{testData1.SoftwareStatementProfileId}_{testData2.RegistrationScope.AbbreviatedName()}";
        var testNameUnique = $"{testName}_{Guid.NewGuid()}";

        // Set test options
        var bankRegistrationOptions = BankRegistrationOptions.TestRegistration;
        var accountAccessConsentOptions = AccountAccessConsentOptions.TestConsent;

        // Get bank test settings
        BankTestSettings bankTestSettings =
            _serviceProvider.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();

        // Get bank profile definitions
        var bankProfileDefinitions =
            _serviceProvider.GetRequiredService<IBankProfileService>();
        BankProfile bankProfile = bankProfileDefinitions.GetBankProfile(testData2.BankProfileEnum);

        // Get bank user
        BankUser? bankUser = testData2.AuthUiInputUserName is not null
            ? new BankUser(
                testData2.AuthUiInputUserName,
                testData2.AuthUiInputPassword!, // not null when AuthUiInputUserName not null
                new List<Account>(),
                new List<DomesticVrpAccountIndexPair>())
            : null;

        // Get API client
        var processedSoftwareStatementProfileStore =
            _serviceProvider.GetRequiredService<IProcessedSoftwareStatementProfileStore>();
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await processedSoftwareStatementProfileStore.GetAsync(
                testData1.SoftwareStatementProfileId,
                testData1.SoftwareStatementAndCertificateProfileOverride);
        IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

        // Get request builder
        using IRequestBuilderContainer requestBuilderContainer = requestBuilderGenerator();
        IRequestBuilder requestBuilder = requestBuilderContainer.RequestBuilder;

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
                Devtools = launchOptions.DevTools,
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

        // CREATE and READ bank configuration objects
        // Create bankRegistration or use existing
        string softwareStatementProfileId = testData1.SoftwareStatementProfileId;
        string? statementProfileOverride = testData1.SoftwareStatementAndCertificateProfileOverride;
        RegistrationScopeEnum registrationScope = testData2.RegistrationScope;
        BankRegistration bankRegistrationRequest = await GetBankRegistrationRequest(
            bankProfile,
            softwareStatementProfileId,
            statementProfileOverride,
            registrationScope,
            testDataProcessorFluentRequestLogging,
            testNameUnique,
            modifiedBy);

        // Handle "only delete" case
        if (bankRegistrationOptions is BankRegistrationOptions.OnlyDeleteRegistration)
        {
            // Create BankRegistration using existing external API registration
            bankRegistrationRequest.ExternalApiObject =
                GetRequiredExternalApiBankRegistration(testData2);
            Guid bankRegistrationId =
                await CreateBankRegistration(requestBuilder, bankRegistrationRequest);

            // Delete BankRegistration (includes forced external API delete which may fail)
            await DeleteBankRegistration(requestBuilder, bankRegistrationId, modifiedBy, true);
        }

        // Handle "only create" case
        else if (bankRegistrationOptions is BankRegistrationOptions.OnlyCreateRegistration)
        {
            // Create fresh BankRegistration
            Guid _ =
                await CreateBankRegistration(requestBuilder, bankRegistrationRequest);
        }

        // Handle "normal" case
        else
        {
            if (bankProfile.BankConfigurationApiSettings.TestTemporaryBankRegistration)
            {
                // Create fresh BankRegistration
                Guid bankRegistrationIdTmp =
                    await CreateBankRegistration(requestBuilder, bankRegistrationRequest);

                // Delete BankRegistration (includes external API delete as appropriate)
                await DeleteBankRegistration(requestBuilder, bankRegistrationIdTmp, modifiedBy, null);
            }

            // Create BankRegistration using existing external API registration
            bankRegistrationRequest.ExternalApiObject =
                GetRequiredExternalApiBankRegistration(testData2);
            Guid bankRegistrationId =
                await CreateBankRegistration(requestBuilder, bankRegistrationRequest);

            // Read BankRegistration
            BankRegistrationResponse bankRegistrationReadResponse = await requestBuilder
                .BankConfiguration
                .BankRegistrations
                .ReadAsync(
                    bankRegistrationId,
                    modifiedBy);

            // Checks
            bankRegistrationReadResponse.Should().NotBeNull();
            bankRegistrationReadResponse.Warnings.Should().BeNull();

            // Run account access consent subtests
            string authUrlLeftPart =
                new Uri(bankRegistrationReadResponse.DefaultRedirectUri)
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
                        bankProfile.AccountAndTransactionApiSettings,
                        requestBuilder,
                        requestBuilderGenerator,
                        testNameUnique,
                        modifiedBy,
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("config"),
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("aisp")
                            .AppendToPath($"{subTest.ToString()}"),
                        consentAuth,
                        authUrlLeftPart,
                        bankUser,
                        accountAccessConsentOptions,
                        apiClient);
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
                        bankProfile.PaymentInitiationApiSettings,
                        requestBuilder,
                        requestBuilderGenerator,
                        testNameUnique,
                        modifiedBy,
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("config"),
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("pisp")
                            .AppendToPath($"{subTest.ToString()}"),
                        consentAuth,
                        bankUser,
                        apiClient);
                }

                // Run domestic VRP consent subtests
                foreach (DomesticVrpSubtestEnum subTest in
                         DomesticVrpSubtest.DomesticVrpFunctionalSubtestsSupported(bankProfile))
                {
                    await DomesticVrpSubtest.RunTest(
                        subTest,
                        bankProfile,
                        bankRegistrationId,
                        bankProfile.VariableRecurringPaymentsApiSettings,
                        requestBuilder,
                        requestBuilderGenerator,
                        testNameUnique,
                        modifiedBy,
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("config"),
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("vrp")
                            .AppendToPath($"{subTest.ToString()}"),
                        consentAuth,
                        bankUser);
                }
            }


            // Delete BankRegistration (excludes external API delete)
            await DeleteBankRegistration(requestBuilder, bankRegistrationId, modifiedBy, false);
        }
    }

    private static async Task<Guid> CreateBankRegistration(
        IRequestBuilder requestBuilder,
        BankRegistration bankRegistrationRequest)
    {
        BankRegistrationResponse registrationResp = await requestBuilder
            .BankConfiguration
            .BankRegistrations
            .CreateAsync(bankRegistrationRequest);

        // Checks and assignments
        registrationResp.Should().NotBeNull();
        registrationResp.Warnings.Should().BeNull();
        if (bankRegistrationRequest.ExternalApiObject is not null)
        {
            registrationResp.ExternalApiResponse.Should().BeNull();
        }
        else
        {
            registrationResp.ExternalApiResponse.Should().NotBeNull();
        }

        Guid bankRegistrationId1 = registrationResp.Id;
        return bankRegistrationId1;
    }

    private static ExternalApiBankRegistration GetRequiredExternalApiBankRegistration(BankTestData2 testData2) =>
        new()
        {
            ExternalApiId =
                testData2.BankRegistrationExternalApiId ??
                throw new InvalidOperationException("No external API BankRegistration ID provided."),
            ExternalApiSecret = testData2.BankRegistrationExternalApiSecret,
            RegistrationAccessToken = testData2.BankRegistrationRegistrationAccessToken
        };

    private static async Task DeleteBankRegistration(
        IRequestBuilder requestBuilder,
        Guid bankRegistrationId,
        string modifiedBy,
        bool? includeExternalApiOperation)
    {
        ObjectDeleteResponse bankRegistrationDeleteResponse = await requestBuilder
            .BankConfiguration
            .BankRegistrations
            .DeleteAsync(
                bankRegistrationId,
                modifiedBy,
                includeExternalApiOperation);

        // Checks
        bankRegistrationDeleteResponse.Should().NotBeNull();
        bankRegistrationDeleteResponse.Warnings.Should().BeNull();
    }

    private static async Task<BankRegistration> GetBankRegistrationRequest(
        BankProfile bankProfile,
        string softwareStatementProfileId,
        string? statementProfileOverride,
        RegistrationScopeEnum registrationScope,
        FilePathBuilder testDataProcessorFluentRequestLogging,
        string testNameUnique,
        string modifiedBy)
    {
        var registrationRequest = new BankRegistration
        {
            BankProfile = bankProfile.BankProfileEnum,
            SoftwareStatementProfileId = softwareStatementProfileId,
            SoftwareStatementProfileOverrideCase =
                statementProfileOverride,
            RegistrationScope = registrationScope
        };
        await testDataProcessorFluentRequestLogging
            .AppendToPath("config")
            .AppendToPath("bankRegistration")
            .AppendToPath("postRequest")
            .WriteFile(registrationRequest);
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
