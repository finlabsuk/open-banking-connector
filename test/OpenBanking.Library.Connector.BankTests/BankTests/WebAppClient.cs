// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class WebAppClient
{
    private readonly HttpClient _client;

    public WebAppClient(WebApplicationFactory<Program> factory)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // factory = factory.WithWebHostBuilder(
            //     builder => builder.UseSolutionRelativeContentRoot(
            //         "src/OpenBanking.WebApp.Connector"));
            factory = factory.WithWebHostBuilder(builder => builder.UseContentRoot(""));
        }
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { BaseAddress = new Uri("http://localhost:5000") });
    }

    public async Task<TResponse> GetAsync<TResponse>(string uriPath)
    {
        // Send request
        using HttpResponseMessage httpResponse = await _client.GetAsync(uriPath);

        // Check status code
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // De-serialise content
        string responseContentString = await httpResponse.Content.ReadAsStringAsync();
        TResponse response = JsonConvert.DeserializeObject<TResponse>(
            responseContentString,
            ApiClient.GetDefaultJsonSerializerSettings) ?? throw new Exception("De-serialisation failure.");

        return response;
    }

    public async Task<TResponse> CreateAsync<TResponse, TRequest>(string uriPath, TRequest request)
    {
        // Serialise content
        string requestContentString = JsonConvert.SerializeObject(
            request,
            ApiClient.GetDefaultJsonSerializerSettings);

        // Send request
        using var requestContent = new StringContent(requestContentString, Encoding.UTF8, "application/json");
        using HttpResponseMessage httpResponse = await _client.PostAsync(
            uriPath,
            requestContent);

        // Check status code
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // De-serialise content
        string responseContentString = await httpResponse.Content.ReadAsStringAsync();
        TResponse response = JsonConvert.DeserializeObject<TResponse>(
            responseContentString,
            ApiClient.GetDefaultJsonSerializerSettings) ?? throw new Exception("De-serialisation failure.");

        return response;
    }

    public async Task<TResponse> DeleteAsync<TResponse>(string uriPath)
    {
        // Send request
        using HttpResponseMessage httpResponse = await _client.DeleteAsync(uriPath);

        // Check status code
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // De-serialise content
        string responseContentString = await httpResponse.Content.ReadAsStringAsync();
        TResponse response = JsonConvert.DeserializeObject<TResponse>(
            responseContentString,
            ApiClient.GetDefaultJsonSerializerSettings) ?? throw new Exception("De-serialisation failure.");

        return response;
    }
}
