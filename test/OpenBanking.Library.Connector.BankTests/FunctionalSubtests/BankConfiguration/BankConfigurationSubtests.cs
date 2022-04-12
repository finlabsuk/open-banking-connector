// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.BankConfiguration
{
    public static class BankConfigurationSubtests
    {
        public static async
            Task<(Guid bankId, Guid bankRegistrationId, Guid bankApiSetId)>
            PostAndGetObjects(
                string softwareStatementProfileId,
                string? softwareStatementAndCertificateProfileOverrideCase,
                RegistrationScopeEnum registrationScope,
                IRequestBuilder requestBuilder,
                BankProfile bankProfile,
                string testNameUnique,
                FilePathBuilder testDataProcessorFluentRequestLogging,
                FilePathBuilder? testDataProcessorApiLogging,
                FilePathBuilder testDataProcessorApiOverrides)
        {
            // Create bank
            Bank bankRequest = bankProfile.BankRequest("placeholder: dynamically generated based on unused names");
            await testDataProcessorFluentRequestLogging
                .AppendToPath("bank")
                .AppendToPath("postRequest")
                .WriteFile(bankRequest);
            bankRequest.Name = testNameUnique;
            IFluentResponse<BankResponse> bankResp = await requestBuilder
                .BankConfiguration
                .Banks
                .CreateLocalAsync(bankRequest);

            bankResp.Should().NotBeNull();
            bankResp.Messages.Should().BeEmpty();
            bankResp.Data.Should().NotBeNull();
            Guid bankId = bankResp.Data!.Id;

            // Create bank registration
            string filePath = testDataProcessorApiOverrides
                .AppendToPath("bankRegistration")
                .AppendToPath("postResponse")
                .GetFilePath();
            string? apiResponseOverrideFile = File.Exists(filePath) ? filePath : null;
            string openIdConfigurationReplacementPath = testDataProcessorApiOverrides
                .AppendToPath("openIdConfiguration")
                .AppendToPath("getResponse")
                .GetFilePath();
            string? openIdConfigurationReplacement =
                File.Exists(openIdConfigurationReplacementPath)
                    ? await File.ReadAllTextAsync(openIdConfigurationReplacementPath)
                    : null;

            BankRegistration registrationRequest = bankProfile.BankRegistrationRequest(
                "placeholder: dynamically generated based on unused names",
                default,
                softwareStatementProfileId,
                softwareStatementAndCertificateProfileOverrideCase,
                registrationScope,
                openIdConfigurationReplacement);
            await testDataProcessorFluentRequestLogging
                .AppendToPath("bankRegistration")
                .AppendToPath("postRequest")
                .WriteFile(registrationRequest);

            registrationRequest.Name = testNameUnique;
            registrationRequest.BankId = bankId;
            IFluentResponse<BankRegistrationResponse> registrationResp = await requestBuilder
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

            // Create bank API information
            BankApiSet apiSetRequest = bankProfile.BankApiSetRequest(
                "placeholder: dynamically generated based on unused names",
                default);
            await testDataProcessorFluentRequestLogging
                .AppendToPath("bankApiInformation")
                .AppendToPath("postRequest")
                .WriteFile(apiSetRequest);

            apiSetRequest.Name = testNameUnique;
            apiSetRequest.BankId = bankId;
            IFluentResponse<BankApiSetResponse> apiSetResponse = await requestBuilder
                .BankConfiguration
                .BankApiSets
                .CreateLocalAsync(apiSetRequest);

            apiSetResponse.Should().NotBeNull();
            apiSetResponse.Messages.Should().BeEmpty();
            apiSetResponse.Data.Should().NotBeNull();
            Guid bankApiSetId = apiSetResponse.Data!.Id;

            return (bankId, bankRegistrationId, bankApiSetId);
        }

        public static async Task DeleteObjects(
            IRequestBuilder requestBuilder,
            Guid bankApiInformationId,
            Guid bankRegistrationId,
            Guid bankId,
            ClientRegistrationApiSettings clientRegistrationApiSettings)
        {
            // Delete objects
            IFluentResponse bankApiInformationResp = await requestBuilder
                .BankConfiguration
                .BankApiSets
                .DeleteLocalAsync(bankApiInformationId);
            bankApiInformationResp.Should().NotBeNull();
            bankApiInformationResp.Messages.Should().BeEmpty();

            IFluentResponse bankRegistrationResp;
            if (clientRegistrationApiSettings.UseDeleteEndpoint)
            {
                bankRegistrationResp = await requestBuilder
                    .BankConfiguration
                    .BankRegistrations
                    .DeleteAsync(bankRegistrationId, null, clientRegistrationApiSettings.UseRegistrationAccessToken);
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
