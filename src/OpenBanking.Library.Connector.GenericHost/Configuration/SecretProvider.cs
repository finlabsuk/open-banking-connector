// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Configuration;

public class SecretProvider(IConfiguration configuration) : ISecretProvider
{
    public async Task<SecretResult> GetSecretAsync(
        SecretDescription secretDescription)
    {
        if (secretDescription.Source is SecretSource.AwsSsmParameterStore)
        {
            try
            {
                using var client = new AmazonSimpleSystemsManagementClient();
                GetParameterResponse? response =
                    await client.GetParameterAsync(
                        new GetParameterRequest
                        {
                            Name = secretDescription.Name,
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
                    ErrorMessage = $"No value with key {secretDescription.Name} can be found."
                };
            }
        }
        var tmp = configuration.GetValue<string>(secretDescription.Name, "");
        return string.IsNullOrEmpty(tmp)
            ? new SecretResult
            {
                SecretObtained = false,
                ErrorMessage = $"No value with key {secretDescription.Name} can be found."
            }
            : new SecretResult
            {
                SecretObtained = true,
                Secret = tmp
            };
    }
}
