// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
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
            // Create bank
            Bank bankRequest = bankProfile.GetBankRequest();
            await testDataProcessorFluentRequestLogging
                .AppendToPath("bank")
                .AppendToPath("postRequest")
                .WriteFile(bankRequest);
            bankRequest.CreatedBy = "Automated bank tests";
            bankRequest.Reference = testNameUnique;
            IFluentResponse<BankResponse> bankResp = await requestBuilder
                .BankConfiguration
                .Banks
                .CreateLocalAsync(bankRequest);

            bankResp.Should().NotBeNull();
            bankResp.Messages.Should().BeEmpty();
            bankResp.Data.Should().NotBeNull();
            Guid bankId = bankResp.Data!.Id;

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
            registrationRequest.CreatedBy = "Automated bank tests";
            registrationRequest.Reference = testNameUnique;
            IFluentResponse<BankRegistrationReadResponse> registrationResp = await requestBuilder
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

            registrationResp.Should().NotBeNull();
            registrationResp.Messages.Should().BeEmpty();
            registrationResp.Data.Should().NotBeNull();
            Guid bankRegistrationId = registrationResp.Data!.Id;

            return (bankId, bankRegistrationId);
        }

        public static async Task DeleteObjects(
            BankTestData2 testData2,
            IRequestBuilder requestBuilder,
            Guid bankRegistrationId,
            Guid bankId,
            BankConfigurationApiSettings bankConfigurationApiSettings)
        {
            IFluentResponse bankRegistrationResp;
            if (bankConfigurationApiSettings.UseDeleteEndpoint)
            {
                bankRegistrationResp = await requestBuilder
                    .BankConfiguration
                    .BankRegistrations
                    .DeleteAsync(bankRegistrationId, null, bankConfigurationApiSettings.UseRegistrationAccessToken);
            }
            else
            {
                bankRegistrationResp = await requestBuilder
                    .BankConfiguration
                    .BankRegistrations
                    .DeleteLocalAsync(bankRegistrationId);
            }

            bankRegistrationResp.Should().NotBeNull();
            bankRegistrationResp.Messages.Should().BeEmpty();

            IFluentResponse bankResp = await requestBuilder
                .BankConfiguration
                .Banks
                .DeleteLocalAsync(bankId);
            bankResp.Should().NotBeNull();
            bankResp.Messages.Should().BeEmpty();
        }
    }
}
