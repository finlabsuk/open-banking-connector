// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.BankConfiguration
{
    public static class BankConfigurationSubtests
    {
        public static async
            Task<(Guid bankId, Guid bankRegistrationId)>
            PostAndGetObjects(
                BankTestData1 testData1,
                BankTestData2 testData2,
                IRequestBuilder requestBuilder,
                BankProfile bankProfile,
                string testNameUnique,
                string modifiedBy,
                FilePathBuilder testDataProcessorFluentRequestLogging)
        {
            // Create bank
            await testDataProcessorFluentRequestLogging
                .AppendToPath("bank")
                .AppendToPath("postRequest")
                .WriteFile(bankProfile.GetBankRequest());
            var bankRequest = new Bank
            {
                Reference = testNameUnique,
                CreatedBy = modifiedBy,
                BankProfile = bankProfile.BankProfileEnum
            };
            BankResponse bankCreateResponse = await requestBuilder
                .BankConfiguration
                .Banks
                .CreateLocalAsync(bankRequest);

            // Checks and assignments
            bankCreateResponse.Should().NotBeNull();
            bankCreateResponse.Warnings.Should().BeNull();
            Guid bankId = bankCreateResponse.Id;

            // Read bank
            BankResponse bankReadResponse = await requestBuilder
                .BankConfiguration
                .Banks
                .ReadLocalAsync(bankId);

            // Checks
            bankReadResponse.Should().NotBeNull();
            bankReadResponse.Warnings.Should().BeNull();

            // Create bankRegistration or use existing
            await testDataProcessorFluentRequestLogging
                .AppendToPath("bankRegistration")
                .AppendToPath("postRequest")
                .WriteFile(
                    bankProfile.GetBankRegistrationRequest(
                        default,
                        testData1.SoftwareStatementProfileId,
                        testData1.SoftwareStatementAndCertificateProfileOverride,
                        testData1.RegistrationScope));

            var registrationRequest = new BankRegistration
            {
                Reference = testNameUnique,
                CreatedBy = modifiedBy,
                BankProfile = bankProfile.BankProfileEnum,
                BankId = bankId,
                SoftwareStatementProfileId = testData1.SoftwareStatementProfileId,
                SoftwareStatementAndCertificateProfileOverrideCase =
                    testData1.SoftwareStatementAndCertificateProfileOverride,
                RegistrationScope = testData1.RegistrationScope,
                ExternalApiObject =
                    testData2.BankRegistrationExternalApiId is not null
                        ? new ExternalApiBankRegistration
                        {
                            ExternalApiId = testData2.BankRegistrationExternalApiId,
                            ExternalApiSecret = testData2.BankRegistrationExternalApiSecret,
                            RegistrationAccessToken = testData2.BankRegistrationRegistrationAccessToken
                        }
                        : null
            };
            BankRegistrationResponse registrationResp = await requestBuilder
                .BankConfiguration
                .BankRegistrations
                .CreateAsync(registrationRequest);

            // Checks and assignments
            registrationResp.Should().NotBeNull();
            registrationResp.Warnings.Should().BeNull();
            if (testData2.BankRegistrationExternalApiId is null)
            {
                registrationResp.ExternalApiResponse.Should().NotBeNull();
            }

            Guid bankRegistrationId = registrationResp.Id;

            // Read bankRegistration
            BankRegistrationResponse bankRegistrationReadResponse = await requestBuilder
                .BankConfiguration
                .BankRegistrations
                .ReadAsync(
                    bankRegistrationId,
                    modifiedBy,
                    bankProfile.BankProfileEnum);

            // Checks
            bankRegistrationReadResponse.Should().NotBeNull();
            bankRegistrationReadResponse.Warnings.Should().BeNull();

            return (bankId, bankRegistrationId);
        }

        public static async Task DeleteObjects(
            BankTestData2 testData2,
            IRequestBuilder requestBuilder,
            string modifiedBy,
            Guid bankRegistrationId,
            Guid bankId,
            BankProfile bankProfile,
            AppTests.TestType testType)
        {
            // Delete bankRegistration
            bool includeExternalApiOperation = false;
            if (bankProfile.BankConfigurationApiSettings.UseDeleteEndpoint)
            {
                includeExternalApiOperation = testType switch
                {
                    // Never delete at API when creating reg
                    AppTests.TestType.CreateRegistration => false,

                    // Always delete at API when deleting reg
                    AppTests.TestType.DeleteRegistration => true,

                    // Normally delete based on whether external API reg supplied or created
                    AppTests.TestType.AllEndpoints => testData2.BankRegistrationExternalApiId is null,

                    _ => throw new ArgumentOutOfRangeException(nameof(testType), testType, null)
                };
            }

            ObjectDeleteResponse bankRegistrationDeleteResponse = await requestBuilder
                .BankConfiguration
                .BankRegistrations
                .DeleteAsync(
                    bankRegistrationId,
                    modifiedBy,
                    null,
                    includeExternalApiOperation,
                    bankProfile.BankConfigurationApiSettings.UseRegistrationAccessToken);

            // Checks
            bankRegistrationDeleteResponse.Should().NotBeNull();
            bankRegistrationDeleteResponse.Warnings.Should().BeNull();

            // Delete bank
            ObjectDeleteResponse bankDeleteResponse = await requestBuilder
                .BankConfiguration
                .Banks
                .DeleteLocalAsync(bankId, modifiedBy);

            // Checks
            bankDeleteResponse.Should().NotBeNull();
            bankDeleteResponse.Warnings.Should().BeNull();
        }
    }
}
