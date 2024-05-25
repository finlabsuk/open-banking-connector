// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class WebAppClient
{
    private readonly HttpClient _client;

    public WebAppClient(BankTestingFixture bankTestingFixture)
    {
        _client = bankTestingFixture.CreateClient(
            new WebApplicationFactoryClientOptions { BaseAddress = new Uri("http://localhost:5000") });
    }

    public async Task<TResponse> GetAsync<TResponse>(
        string uriPath,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> extraHeaders)
    {
        // Assemble request
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uriPath);
        foreach ((string key, IEnumerable<string> value) in extraHeaders)
        {
            httpRequestMessage.Headers.Add(key, value);
        }

        // Send request
        using HttpResponseMessage httpResponse = await _client.SendAsync(httpRequestMessage);

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

    public async Task<TResponse> DeleteAsync<TResponse>(
        string uriPath,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> extraHeaders)
    {
        // Assemble request
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uriPath);
        foreach ((string key, IEnumerable<string> value) in extraHeaders)
        {
            httpRequestMessage.Headers.Add(key, value);
        }

        // Send request
        using HttpResponseMessage httpResponse = await _client.SendAsync(httpRequestMessage);

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
