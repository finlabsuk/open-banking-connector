// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests
{
    public static class ClientRegistrationSubtests
    {
        public static async Task<(Guid bankId, Guid bankRegistrationId, Guid bankApiInformationId)> PostAndGetObjects(
            string softwareStatementProfileId,
            RegistrationScope registrationScope,
            IRequestBuilder requestBuilder,
            BankProfile bankProfile,
            string testNameUnique,
            TestDataWriter testDataProcessorFluentRequestLogging,
            TestDataWriter? testDataProcessorApiLogging,
            TestDataWriter testDataProcessorApiOverrides)
        {
            // Create bank
            Bank bankRequest = bankProfile.BankObject("placeholder: dynamically generated based on unused names");
            await testDataProcessorFluentRequestLogging
                .AppendToPath("bank")
                .AppendToPath("postRequest")
                .ProcessData(bankRequest, ".json");
            bankRequest.Name = testNameUnique;
            IFluentResponse<BankResponse> bankResp = await requestBuilder.ClientRegistration
                .Banks
                .PostLocalAsync(bankRequest);

            bankResp.Should().NotBeNull();
            bankResp.Messages.Should().BeEmpty();
            bankResp.Data.Should().NotBeNull();
            Guid bankId = bankResp.Data!.Id;

            // Create bank registration
            string filePath = testDataProcessorApiOverrides
                .AppendToPath("bankRegistration")
                .AppendToPath("postResponse")
                .GetFilePath(".json");
            string? apiResponseOverrideFile = File.Exists(filePath) ? filePath : null;

            BankRegistration registrationRequest = bankProfile.BankRegistration(
                "placeholder: dynamically generated based on unused names",
                default,
                softwareStatementProfileId,
                registrationScope);
            await testDataProcessorFluentRequestLogging
                .AppendToPath("bankRegistration")
                .AppendToPath("postRequest")
                .ProcessData(registrationRequest, ".json");

            registrationRequest.Name = testNameUnique;
            registrationRequest.BankId = bankId;
            IFluentResponse<BankRegistrationResponse> registrationResp = await requestBuilder.ClientRegistration
                .BankRegistrations
                .PostAsync(
                    registrationRequest,
                    null,
                    testDataProcessorApiLogging?.AppendToPath("bankRegistration").AppendToPath("postRequest")
                        .GetFilePath(".json"),
                    testDataProcessorApiLogging?.AppendToPath("bankRegistration").AppendToPath("postResponse")
                        .GetFilePath(".json"),
                    apiResponseOverrideFile);

            registrationResp.Should().NotBeNull();
            registrationResp.Messages.Should().BeEmpty();
            registrationResp.Data.Should().NotBeNull();
            Guid bankRegistrationId = registrationResp.Data!.Id;

            // Create bank API information
            BankApiInformation apiInformationRequest = bankProfile.BankApiInformation(
                "placeholder: dynamically generated based on unused names",
                default);
            await testDataProcessorFluentRequestLogging
                .AppendToPath("bankApiInformation")
                .AppendToPath("postRequest")
                .ProcessData(apiInformationRequest, ".json");

            apiInformationRequest.Name = testNameUnique;
            apiInformationRequest.BankId = bankId;
            IFluentResponse<BankApiInformationResponse> apiInformationResponse = await requestBuilder
                .ClientRegistration
                .BankApiInformationObjects
                .PostLocalAsync(apiInformationRequest);

            apiInformationResponse.Should().NotBeNull();
            apiInformationResponse.Messages.Should().BeEmpty();
            apiInformationResponse.Data.Should().NotBeNull();
            Guid bankApiInformationId = apiInformationResponse.Data!.Id;

            return (bankId, bankRegistrationId, bankApiInformationId);
        }

        public static async Task DeleteObjects(
            IRequestBuilder requestBuilder,
            Guid bankApiInformationId,
            Guid bankRegistrationId,
            Guid bankId,
            ClientRegistrationApiSettings clientRegistrationApiSettings)
        {
            // Delete objects
            IFluentResponse bankApiInformationResp = await requestBuilder.ClientRegistration
                .BankApiInformationObjects
                .DeleteLocalAsync(bankApiInformationId);
            bankApiInformationResp.Should().NotBeNull();
            bankApiInformationResp.Messages.Should().BeEmpty();

            IFluentResponse bankRegistrationResp;
            if (clientRegistrationApiSettings.UseDeleteEndpoint)
            {
                bankRegistrationResp = await requestBuilder.ClientRegistration
                    .BankRegistrations
                    .DeleteAsync(bankRegistrationId, null, clientRegistrationApiSettings.UseRegistrationAccessToken);
            }
            else
            {
                bankRegistrationResp = await requestBuilder.ClientRegistration
                    .BankRegistrations
                    .DeleteLocalAsync(bankRegistrationId);
            }

            bankRegistrationResp.Should().NotBeNull();
            bankRegistrationResp.Messages.Should().BeEmpty();

            IFluentResponse bankResp = await requestBuilder.ClientRegistration
                .Banks
                .DeleteLocalAsync(bankId);
            bankResp.Should().NotBeNull();
            bankResp.Messages.Should().BeEmpty();
        }
    }
}
