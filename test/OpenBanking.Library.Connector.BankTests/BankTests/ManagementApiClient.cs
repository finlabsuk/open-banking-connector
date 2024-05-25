// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class ManagementApiClient(WebAppClient client)
{
    public async Task<BankRegistrationResponse> BankRegistrationRead(BankRegistrationReadParams readParams)
    {
        // Read object
        var uriPath = $"/manage/bank-registrations/{readParams.Id}";
        var response =
            await client.GetAsync<BankRegistrationResponse>(
                uriPath,
                readParams.ExcludeExternalApiOperation
                    ? [new KeyValuePair<string, IEnumerable<string>>("x-obc-exclude-external-api-operation", ["true"])]
                    : []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<BankRegistrationResponse> BankRegistrationCreate(BankRegistration request)
    {
        // Create object
        var uriPath = "/manage/bank-registrations";
        BankRegistrationResponse response =
            await client.CreateAsync<BankRegistrationResponse, BankRegistration>(uriPath, request);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<BaseResponse> BankRegistrationDelete(BankRegistrationDeleteParams deleteParams)
    {
        // Delete object
        var uriPath = $"/manage/bank-registrations/{deleteParams.Id}";
        var response =
            await client.DeleteAsync<BaseResponse>(
                uriPath,
                deleteParams.ExcludeExternalApiOperation
                    ? [new KeyValuePair<string, IEnumerable<string>>("x-obc-exclude-external-api-operation", ["true"])]
                    : []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<ObWacCertificateResponse> ObWacCertificateCreate(ObWacCertificate request)
    {
        // Create object
        var uriPath = "/manage/obwac-certificates";
        ObWacCertificateResponse response =
            await client.CreateAsync<ObWacCertificateResponse, ObWacCertificate>(uriPath, request);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<ObWacCertificateResponse> ObWacCertificateRead(Guid id)
    {
        // Read object
        var uriPath = $"/manage/obwac-certificates/{id}";
        var response =
            await client.GetAsync<ObWacCertificateResponse>(uriPath, []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<BaseResponse> ObWacCertificateDelete(Guid id)
    {
        // Delete object
        var uriPath = $"/manage/obwac-certificates/{id}";
        var response =
            await client.DeleteAsync<BaseResponse>(uriPath, []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<BaseResponse> ObSealCertificateDelete(Guid id)
    {
        // Delete object
        var uriPath = $"/manage/obseal-certificates/{id}";
        var response =
            await client.DeleteAsync<BaseResponse>(uriPath, []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<BaseResponse> SoftwareStatementDelete(Guid id)
    {
        // Delete object
        var uriPath = $"/manage/software-statements/{id}";
        var response =
            await client.DeleteAsync<BaseResponse>(uriPath, []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }


    public async Task<ObSealCertificateResponse> ObSealCertificateCreate(ObSealCertificate request)
    {
        // Create object
        var uriPath = "/manage/obseal-certificates";
        ObSealCertificateResponse response =
            await client.CreateAsync<ObSealCertificateResponse, ObSealCertificate>(uriPath, request);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<ObSealCertificateResponse> ObSealCertificateRead(Guid id)
    {
        // Read object
        var uriPath = $"/manage/obseal-certificates/{id}";
        var response =
            await client.GetAsync<ObSealCertificateResponse>(uriPath, []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<SoftwareStatementResponse> SoftwareStatementCreate(SoftwareStatement request)
    {
        // Create object
        var uriPath = "/manage/software-statements";
        SoftwareStatementResponse response =
            await client.CreateAsync<SoftwareStatementResponse, SoftwareStatement>(uriPath, request);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }

    public async Task<SoftwareStatementResponse> SoftwareStatementRead(Guid id)
    {
        // Read object
        var uriPath = $"/manage/software-statements/{id}";
        var response =
            await client.GetAsync<SoftwareStatementResponse>(uriPath, []);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }
}
