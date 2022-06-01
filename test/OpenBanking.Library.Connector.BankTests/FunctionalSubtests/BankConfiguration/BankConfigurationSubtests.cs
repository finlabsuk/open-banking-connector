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
                FilePathBuilder testDataProcessorFluentRequestLogging,
                FilePathBuilder? testDataProcessorApiLogging,
                FilePathBuilder testDataProcessorApiOverrides)
        {
            var modifiedBy = "Automated bank tests";

            // Create bank
            Bank bankRequest = bankProfile.GetBankRequest();
            await testDataProcessorFluentRequestLogging
                .AppendToPath("bank")
                .AppendToPath("postRequest")
                .WriteFile(bankRequest);
            bankRequest.CreatedBy = modifiedBy;
            bankRequest.Reference = testNameUnique;
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
            string filePath = testDataProcessorApiOverrides
                .AppendToPath("bankRegistration")
                .AppendToPath("postResponse")
                .GetFilePath();
            string? apiResponseOverrideFile = File.Exists(filePath) ? filePath : null;
            BankRegistration registrationRequest = bankProfile.GetBankRegistrationRequest(
                default,
                testData1.SoftwareStatementProfileId,
                testData1.SoftwareStatementAndCertificateProfileOverride,
                testData1.RegistrationScope);
            await testDataProcessorFluentRequestLogging
                .AppendToPath("bankRegistration")
                .AppendToPath("postRequest")
                .WriteFile(registrationRequest);

            if (testData2.BankRegistrationExternalApiId is not null)
            {
                registrationRequest.ExternalApiObject = new ExternalApiBankRegistration
                {
                    ExternalApiId = testData2.BankRegistrationExternalApiId
                };
            }

            registrationRequest.BankId = bankId;
            registrationRequest.CreatedBy = modifiedBy;
            registrationRequest.Reference = testNameUnique;
            BankRegistrationResponse registrationResp = await requestBuilder
                .BankConfiguration
                .BankRegistrations
                .CreateAsync(
                    registrationRequest,
                    null,
                    testDataProcessorApiLogging?.AppendToPath("bankRegistration").AppendToPath("postRequest")
                        .GetFilePath(),
                    testDataProcessorApiLogging?.AppendToPath("bankRegistration").AppendToPath("postResponse")
                        .GetFilePath(),
                    apiResponseOverrideFile);

            // Checks and assignments
            registrationResp.Should().NotBeNull();
            registrationResp.Warnings.Should().BeNull();
            registrationResp.ExternalApiResponse.Should().NotBeNull();
            Guid bankRegistrationId = registrationResp.Id;

            // Read bankRegistration
            BankRegistrationResponse bankRegistrationReadResponse = await requestBuilder
                .BankConfiguration
                .BankRegistrations
                .ReadAsync(bankRegistrationId, modifiedBy);

            // Checks
            bankRegistrationReadResponse.Should().NotBeNull();
            bankRegistrationReadResponse.Warnings.Should().BeNull();

            return (bankId, bankRegistrationId);
        }

        public static async Task DeleteObjects(
            BankTestData2 testData2,
            IRequestBuilder requestBuilder,
            Guid bankRegistrationId,
            Guid bankId,
            BankConfigurationApiSettings bankConfigurationApiSettings)
        {
            var modifiedBy = "Automated bank tests";

            // Delete bankRegistration
            bool includeExternalApiOperation = bankConfigurationApiSettings.UseDeleteEndpoint;
            ObjectDeleteResponse bankRegistrationDeleteResponse = await requestBuilder
                .BankConfiguration
                .BankRegistrations
                .DeleteAsync(
                    bankRegistrationId,
                    modifiedBy,
                    includeExternalApiOperation,
                    bankConfigurationApiSettings.UseRegistrationAccessToken);

            // Checks
            bankRegistrationDeleteResponse.Should().NotBeNull();
            bankRegistrationDeleteResponse.Warnings.Should().BeNull();

            // Delete bank
            ObjectDeleteResponse bankResp = await requestBuilder
                .BankConfiguration
                .Banks
                .DeleteLocalAsync(bankId);

            // Checks
            bankResp.Should().NotBeNull();
            bankResp.Warnings.Should().BeNull();
        }
    }
}
