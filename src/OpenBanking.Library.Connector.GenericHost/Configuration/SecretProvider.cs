// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Configuration;

public class SecretProvider(IConfiguration configuration) : ISecretProvider
{
    public async Task<SecretResult> GetSecretAsync(
        SecretDescription secretDescription)
    {
        return secretDescription.Source switch
        {
            SecretSource.AwsSsmParameterStore => await GetSecretFromAwsParameterStore(secretDescription.Name),
            SecretSource.GoogleCloudSecretManagerV1 => await GetSecretFromGoogleCloudSecretManagerV1(
                secretDescription.Name),
            SecretSource.Configuration or _ => GetSecretFromConfiguration(secretDescription.Name)
        };
    }

    private static async Task<SecretResult> GetSecretFromAwsParameterStore(string name)
    {
        try
        {
            using var client = new AmazonSimpleSystemsManagementClient();
            GetParameterResponse? response =
                await client.GetParameterAsync(
                    new GetParameterRequest
                    {
                        Name = name,
                        WithDecryption = true
                    });
            return new SecretResult
            {
                SecretObtained = true,
                Secret = response.Parameter.Value
            };
        }
        catch (AmazonClientException ex)
        {
            return new SecretResult
            {
                SecretObtained = false,
                ErrorMessage = $"The following Amazon SDK exception occurred: {ex.Message}."
            };
        }
        catch (ParameterNotFoundException)
        {
            return new SecretResult
            {
                SecretObtained = false,
                ErrorMessage = $"No value with key {name} can be found."
            };
        }
    }

    private async Task<SecretResult> GetSecretFromGoogleCloudSecretManagerV1(string name)
    {
        try
        {
            var client = await SecretManagerServiceClient.CreateAsync();

            // Name is expected to be in the format projects/{project_id}/secrets/{secret_id}/versions/{secret_version}
            AccessSecretVersionResponse? secret = await client.AccessSecretVersionAsync(name);

            return new SecretResult
            {
                SecretObtained = true,
                Secret = secret.Payload.Data.ToStringUtf8()
            };
        }
        catch (Exception ex)
        {
            return new SecretResult
            {
                SecretObtained = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private SecretResult GetSecretFromConfiguration(string name)
    {
        var tmp = configuration.GetValue<string>(name, "");
        return string.IsNullOrEmpty(tmp)
            ? new SecretResult
            {
                SecretObtained = false,
                ErrorMessage = $"No value with key {name} can be found."
            }
            : new SecretResult
            {
                SecretObtained = true,
                Secret = tmp
            };
    }
}
