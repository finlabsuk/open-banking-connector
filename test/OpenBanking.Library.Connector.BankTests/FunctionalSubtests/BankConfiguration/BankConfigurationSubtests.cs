// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.BankConfiguration;

public static class BankConfigurationSubtests
{
    public static async
        Task<Guid> PostAndGetObjects(
            BankTestData1 testData1,
            BankTestData2 testData2,
            IRequestBuilder requestBuilder,
            BankProfile bankProfile,
            string testNameUnique,
            string modifiedBy,
            FilePathBuilder testDataProcessorFluentRequestLogging)
    {
        // Create bankRegistration or use existing
        var registrationRequest = new BankRegistration
        {
            BankProfile = bankProfile.BankProfileEnum,
            SoftwareStatementProfileId = testData1.SoftwareStatementProfileId,
            SoftwareStatementProfileOverrideCase =
                testData1.SoftwareStatementAndCertificateProfileOverride,
            RegistrationScope = testData1.RegistrationScope
        };
        await testDataProcessorFluentRequestLogging
            .AppendToPath("bankRegistration")
            .AppendToPath("postRequest")
            .WriteFile(registrationRequest);
        if (testData2.BankRegistrationExternalApiId is not null)
        {
            registrationRequest.ExternalApiObject =
                new ExternalApiBankRegistration
                {
                    ExternalApiId = testData2.BankRegistrationExternalApiId,
                    ExternalApiSecret = testData2.BankRegistrationExternalApiSecret,
                    RegistrationAccessToken = testData2.BankRegistrationRegistrationAccessToken
                };
        }

        registrationRequest.Reference = testNameUnique;
        registrationRequest.CreatedBy = modifiedBy;
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
                modifiedBy);

        // Checks
        bankRegistrationReadResponse.Should().NotBeNull();
        bankRegistrationReadResponse.Warnings.Should().BeNull();

        // ObjectDeleteResponse bankRegistrationDeleteResponse = await requestBuilder
        //     .BankConfiguration
        //     .BankRegistrations
        //     .DeleteAsync(
        //         bankRegistrationId,
        //         modifiedBy,
        //         bankProfile.BankProfileEnum);

        return bankRegistrationId;
    }

    public static async Task DeleteObjects(
        BankTestData2 testData2,
        IRequestBuilder requestBuilder,
        string modifiedBy,
        Guid bankRegistrationId,
        BankProfile bankProfile,
        AppTests.BankRegistrationOptions bankRegistrationOptions)
    {
        // Delete bankRegistration
        var includeExternalApiOperation = false;
        if (bankProfile.BankConfigurationApiSettings.UseRegistrationDeleteEndpoint)
        {
            includeExternalApiOperation = bankRegistrationOptions switch
            {
                // Never delete at API when creating reg
                AppTests.BankRegistrationOptions.OnlyCreateRegistration => false,

                // Always delete at API when deleting reg
                AppTests.BankRegistrationOptions.OnlyDeleteRegistration => true,

                // Normally delete based on whether external API reg supplied or created
                AppTests.BankRegistrationOptions.TestRegistration => testData2.BankRegistrationExternalApiId is null,

                _ => throw new ArgumentOutOfRangeException(nameof(bankRegistrationOptions), bankRegistrationOptions, null)
            };
        }

        ObjectDeleteResponse bankRegistrationDeleteResponse = await requestBuilder
            .BankConfiguration
            .BankRegistrations
            .DeleteAsync(
                bankRegistrationId,
                modifiedBy,
                includeExternalApiOperation,
                bankProfile.BankConfigurationApiSettings.UseRegistrationAccessToken);

        // Checks
        bankRegistrationDeleteResponse.Should().NotBeNull();
        bankRegistrationDeleteResponse.Warnings.Should().BeNull();
    }
}
