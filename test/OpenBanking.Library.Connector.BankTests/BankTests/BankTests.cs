// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FluentAssertions;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    public partial class BankTests : BaseTests
    {
        public BankTests(ITestOutputHelper outputHelper, AppContextFixture appContextFixture) : base(
            outputHelper: outputHelper,
            appContextFixture: appContextFixture) { }

        public static TheoryData<string, RegistrationScopeApiSet> TestedSkippedBanksById(BankTestEnum bankTestEnum) =>
            TestedBanksById(bankTestEnum: bankTestEnum, skippedNotUnskipped: true);

        public static TheoryData<string, RegistrationScopeApiSet> TestedUnskippedBanksById(BankTestEnum bankTestEnum) =>
            TestedBanksById(bankTestEnum: bankTestEnum, skippedNotUnskipped: false);

        public static TheoryData<string, RegistrationScopeApiSet> TestedBanksById(
            BankTestEnum bankTestEnum,
            bool skippedNotUnskipped)
        {
            TheoryData<string, RegistrationScopeApiSet> value = new TheoryData<string, RegistrationScopeApiSet>();
            foreach (BankProfileEnum bankEnum in BankProfileEnumHelper.AllBanks)
            {
                BankProfile bankProfile = BankProfileEnumHelper.GetBank(bankEnum);

                ApiTypeSetsIncludedFilter apiTypeSetsIncludedFilter =
                    BankTestEnumHelper.ApiTypeSetsIncludedFilter(bankTestEnum);
                UnskippedFilter unskippedFilter = SkipSettings.UnskippedFilter(bankTestEnum);

                foreach (RegistrationScopeApiSet apiTypes in bankProfile.ApiTypeSetsSupported)
                {
                    // Only examine API types included in test
                    if (!apiTypeSetsIncludedFilter(apiTypes))
                    {
                        continue;
                    }

                    // Determine skip status and add to Theory if matches skippedNotUnskipped
                    bool testCaseSkipped = !unskippedFilter(
                        bankProfile: bankProfile,
                        registrationScopeApiSet: apiTypes);
                    if (testCaseSkipped == skippedNotUnskipped)
                    {
                        value.Add(p1: bankEnum.ToString(), p2: apiTypes);
                    }
                }
            }

            return value;
        }

        [Theory]
        [MemberData(
            memberName: nameof(TestedSkippedBanksById),
            BankTestEnum.PaymentInitiationTest,
            Skip = "Bank skipped due to skip instruction in " + nameof(SkipSettings))]
        [MemberData(
            memberName: nameof(TestedUnskippedBanksById),
            BankTestEnum.PaymentInitiationTest)]
        public async Task PaymentInitiationTest(string bankEnumString, RegistrationScopeApiSet registrationScopeApiSet)
        {
            await DefaultTest(
                bankEnumString: bankEnumString,
                registrationScopeApiSet: registrationScopeApiSet);
        }

        [Theory]
        [MemberData(
            memberName: nameof(TestedSkippedBanksById),
            BankTestEnum.MultipleApiTypesTest,
            Skip = "Bank skipped due to skip instruction in " + nameof(SkipSettings))]
        [MemberData(
            memberName: nameof(TestedUnskippedBanksById),
            BankTestEnum.MultipleApiTypesTest)]
        public async Task MultipleApiTypesTest(string bankEnumString, RegistrationScopeApiSet registrationScopeApiSet)
        {
            await DefaultTest(
                bankEnumString: bankEnumString,
                registrationScopeApiSet: registrationScopeApiSet);
        }

        private string SoftwareStatementProfileId(RegistrationScopeApiSet registrationScopeApiSet)
        {
            string softwareStatementProfileId = registrationScopeApiSet switch
            {
                RegistrationScopeApiSet.PaymentInitiation => "payment-initiation",
                RegistrationScopeApiSet.All => "all",
                RegistrationScopeApiSet.None => throw new ArgumentOutOfRangeException(
                    $"Software statement profile IDs available for non-empty {nameof(RegistrationScopeApiSet)} only."),
                RegistrationScopeApiSet.AccountAndTransaction => "account-transaction",
                RegistrationScopeApiSet.FundsConfirmation => "funds-confirmation",
                _ => throw new ArgumentOutOfRangeException(nameof(registrationScopeApiSet))
            };
            CustomSoftwareStatementProfileId(
                registrationScopeApiSet: registrationScopeApiSet,
                softwareStatementProfileId: ref softwareStatementProfileId);
            return softwareStatementProfileId;
        }

        /// <summary>
        ///     Customisation point for <paramref name="softwareStatementProfileId" /> as function of
        ///     <paramref name="registrationScopeApiSet" />
        /// </summary>
        /// <param name="registrationScopeApiSet"></param>
        /// <param name="softwareStatementProfileId"></param>
        partial void CustomSoftwareStatementProfileId(
            RegistrationScopeApiSet registrationScopeApiSet,
            ref string softwareStatementProfileId);


        private async Task DefaultTest(
            string bankEnumString,
            RegistrationScopeApiSet registrationScopeApiSet)
        {
            // Create test data processor
            TestDataProcessor testDataProcessor = new TestDataProcessor(
                projectRootPath: AppContextFixture.ProjectRootPath,
                testFolderRelPath:
                $"{RegistrationScopeApiSetHelper.AbbreviatedName(registrationScopeApiSet)}_{bankEnumString}");

            // Connect output to logging
            SetTestLogging();

            // Dereference bank
            BankProfileEnum bankProfileEnum = BankProfileEnumHelper.GetBankEnum(bankEnumString);
            BankProfile bankProfile = BankProfileEnumHelper.GetBank(bankProfileEnum);

            // Get services scope
            using IServiceScope serviceScope1 = _host.Services.CreateScope();
            IServiceProvider services = serviceScope1.ServiceProvider;
            IRequestBuilder builder = services.GetRequiredService<IRequestBuilder>();
            INodeJSService nodeJsService = services.GetRequiredService<INodeJSService>();

            // Determine whether to create new registration
            bool createNewRegistration = true;
            //bank.ClientRegistrationBehaviour == ClientRegistrationBehaviour.OverwritesExisting;

            // Create or re-use registration (incl Bank, BankRegistration, BankProfile)
            Guid bankId;
            if (createNewRegistration)
            {
                // Get/update bank name
                string bankName = await GetBankNameNextVersion(
                    builder: builder,
                    defaultBankName: bankProfileEnum.ToString(),
                    registrationScopeApiSet: registrationScopeApiSet);

                // Create bank
                Bank bankRequest = bankProfile.BankObject(
                    name: "placeholder: dynamically generated based on unused names",
                    registrationScopeApiSet: registrationScopeApiSet);
                testDataProcessor.ProcessData(bankRequest);
                bankRequest.Name = bankName;
                FluentResponse<BankResponse> bankResp = await builder.ClientRegistration
                    .Banks
                    .PostAsync(bankRequest);

                bankResp.Should().NotBeNull();
                bankResp.Messages.Should().BeEmpty();
                bankResp.Data.Should().NotBeNull();
                bankId = bankResp.Data!.Id;

                // Create bank registration
                BankRegistration registrationRequest = bankProfile.BankRegistration(
                    bankId: default,
                    softwareStatementProfileId: SoftwareStatementProfileId(registrationScopeApiSet),
                    registrationScopeApiSet: registrationScopeApiSet);
                testDataProcessor.ProcessData(registrationRequest);
                registrationRequest.BankId = bankId;
                FluentResponse<BankRegistrationResponse> bankClientProfileResp = await builder.ClientRegistration
                    .BankRegistrations
                    .PostAsync(registrationRequest);

                bankClientProfileResp.Should().NotBeNull();
                bankClientProfileResp.Messages.Should().BeEmpty();
                bankClientProfileResp.Data.Should().NotBeNull();

                // Create bank profile
                BankApiInformation apiInformationRequest = bankProfile.BankApiInformation(default);
                testDataProcessor.ProcessData(apiInformationRequest);
                apiInformationRequest.BankId = bankId;
                FluentResponse<BankApiInformationResponse> apiProfileFluentResponse = await builder.ClientRegistration
                    .BankApiInformationObjects
                    .PostAsync(apiInformationRequest);

                apiProfileFluentResponse.Should().NotBeNull();
                apiProfileFluentResponse.Messages.Should().BeEmpty();
                apiProfileFluentResponse.Data.Should().NotBeNull();
            }
            else
            {
                // Get/update bank name
                (bool currentVersionExists, string? bankNameNullable) = await GetBankNameCurrentVersion(
                    builder: builder,
                    defaultBankName: bankProfileEnum.ToString(),
                    registrationScopeApiSet: registrationScopeApiSet);
                currentVersionExists.Should().Be(true);
                string bankName = bankNameNullable!;
                await builder.ClientRegistration
                    .Banks
                    .GetAsync(b => b.Name == bankName);

                try
                {
                    FluentResponse<IQueryable<BankResponse>> x = await builder.ClientRegistration
                        .Banks
                        .GetAsync(b => b.Name == bankName);
                    x.Data.Should().NotBeNull();
                    BankResponse y = x.Data!.Single();
                    bankId = y.Id;
                }
                catch (Exception)
                {
                    throw new ArgumentException("No record found for BankName.");
                }
            }

            foreach (DomesticPaymentFunctionalSubtestEnum subTest in
                DomesticPaymentFunctionalSubtest.DomesticPaymentFunctionalSubtestsSupported(bankProfile))
            {
                bool subtestSkipped = !SkipSettings.DomesticPaymentSubtestUnskippedFilter(
                    domesticPaymentFunctionalSubtestEnum: subTest,
                    bankProfile: bankProfile,
                    registrationScopeApiSet: registrationScopeApiSet);
                if (subtestSkipped)
                {
                    continue;
                }

                TestDataProcessor testDataProcessorSubtest =
                    testDataProcessor.AddTestSubfolder($"PI_{subTest.ToString()}");
                await DomesticPaymentFunctionalSubtest.RunTest(
                    subtestEnum: subTest,
                    bankProfile: bankProfile,
                    bankId: bankId,
                    requestBuilder: builder,
                    testDataProcessor: testDataProcessorSubtest,
                    nodeJsService: nodeJsService);
            }

            UnsetTestLogging();
        }

        private static async Task<string> GetBankNameNextVersion(
            IRequestBuilder builder,
            string defaultBankName,
            RegistrationScopeApiSet registrationScopeApiSet)
        {
            (string prefix, int? postfixIfCurrentVersionExists) tmp = await GetBankNameCurrentVersionInner(
                builder: builder,
                defaultBankName: defaultBankName,
                registrationScopeApiSet: registrationScopeApiSet);
            int postfix = (tmp.postfixIfCurrentVersionExists ?? 0) + 1;
            string bankName = tmp.prefix + postfix;
            return bankName;
        }

        private static async Task<(bool currentVersionExists, string? bankName)> GetBankNameCurrentVersion(
            IRequestBuilder builder,
            string defaultBankName,
            RegistrationScopeApiSet registrationScopeApiSet)
        {
            (string prefix, int? postfixIfCurrentVersionExists) tmp = await GetBankNameCurrentVersionInner(
                builder: builder,
                defaultBankName: defaultBankName,
                registrationScopeApiSet: registrationScopeApiSet);
            bool currentVersionExists = tmp.postfixIfCurrentVersionExists != null;
            string? bankName = currentVersionExists ? tmp.prefix + tmp.postfixIfCurrentVersionExists : null;
            return (currentVersionExists, bankName);
        }

        private static async Task<(string prefix, int? postfixIfCurrentVersionExists)> GetBankNameCurrentVersionInner(
            IRequestBuilder builder,
            string defaultBankName,
            RegistrationScopeApiSet registrationScopeApiSet)
        {
            string prefix = defaultBankName + "_api" + registrationScopeApiSet + "_ver";

            // Get banks
            FluentResponse<IQueryable<BankResponse>> bankList = await builder.ClientRegistration
                .Banks
                .GetAsync(b => b.Name.StartsWith(prefix));

            bankList.Should().NotBeNull();
            bankList.Messages.Should().BeEmpty();
            bankList.Data.Should().NotBeNull();

            // Determine bank name current postfix (highest version number)
            IOrderedEnumerable<int> orderedPostfixes;
            try
            {
                orderedPostfixes =
                    bankList.Data!
                        .AsEnumerable()
                        .Select(x => int.Parse(x.Name.Substring(prefix.Length)))
                        .OrderByDescending(x => x);
            }
            catch
            {
                throw new InvalidOperationException("Unexpected bank name postfix found in database.");
            }

            int currentPostfix = orderedPostfixes.FirstOrDefault();

            return (prefix, currentPostfix != 0 ? (int?) currentPostfix : null);
        }
    }
}
