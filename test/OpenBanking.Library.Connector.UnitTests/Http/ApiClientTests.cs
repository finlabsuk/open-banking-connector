﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using RichardSzalay.MockHttp;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Http
{
    public class ApiClientTests
    {
        [Theory]
        [InlineData("https://yadayada.com", "just a test")]
        public async Task SendAsync_ResponseReturned(string url, string content)
        {
            string contentType = "text/plain";

            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, url).Respond(contentType, content);

            var client = mockHttp.ToHttpClient();

            ApiClient apiClient = new ApiClient(
                Substitute.For<IInstrumentationClient>(),
                client);

            HttpRequestMessage req = new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(url)
                .Create();

            HttpResponseMessage response = await apiClient.SendAsync(req);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            string? responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().Be(content);
            response.Content.Headers.ContentType.ToString().Should().Be(contentType + "; charset=utf-8");
        }

        [Theory]
        [InlineData("https://yadayada.com", "just a test")]
        public async Task SendAsync_TraceStarted(string url, string content)
        {
            string contentType = "text/plain";

            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, url).Respond(contentType, content);

            var client = mockHttp.ToHttpClient();

            var instrumentationClient = Substitute.For<IInstrumentationClient>();

            ApiClient apiClient = new ApiClient(instrumentationClient, client);

            HttpRequestMessage req = new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(url)
                .Create();

            HttpResponseMessage response = await apiClient.SendAsync(req);

            instrumentationClient.Received(1).StartTrace(Arg.Any<TraceInfo>());
            instrumentationClient.Received(1).EndTrace(Arg.Any<TraceInfo>());
        }

        [Theory]
        [InlineData("https://yadayada.com")]
        public void SendAsync_ExceptionLogged(string url)
        {
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, url).Respond(x => throw new HttpRequestException());

            var client = mockHttp.ToHttpClient();

            var instrumentationClient = Substitute.For<IInstrumentationClient>();

            ApiClient apiClient = new ApiClient(instrumentationClient, client);

            HttpRequestMessage req = new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(url)
                .Create();

            Func<Task> a = async () => await apiClient.SendAsync(req);

            a.Should().Throw<HttpRequestException>();

            instrumentationClient.Received(1).StartTrace(Arg.Any<TraceInfo>());
            instrumentationClient.Received(0).EndTrace(Arg.Any<TraceInfo>());
            instrumentationClient.Received(1).Exception(Arg.Any<Exception>(), Arg.Any<string>());
        }


        [Theory]
        [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
        public async Task ApiClient_RequestJsonAsync_Success(string url)
        {
            HttpRequestMessage req = new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(url)
                .Create();

            SerialisedEntity entity = new SerialisedEntity { Message = "test message" };
            string content = JsonConvert.SerializeObject(entity);

            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, url).Respond("application/json", content);

            using (var http = mockHttp.ToHttpClient())
            {
                ApiClient api = new ApiClient(
                    Substitute.For<IInstrumentationClient>(),
                    http);
                SerialisedEntity result = await api.RequestJsonAsync<SerialisedEntity>(
                    req,
                    false,
                    null);

                result.Message.Should().Be(entity.Message);
            }
        }

        [Theory]
        [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
        public async Task ApiClient_RequestJsonAsync_Success_NoContent(string url)
        {
            HttpRequestMessage req = new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(url)
                .Create();

            SerialisedEntity entity = new SerialisedEntity { Message = "test message" };
            string content = JsonConvert.SerializeObject(entity);

            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, url).Respond(r => new HttpResponseMessage(HttpStatusCode.OK));

            using (var http = mockHttp.ToHttpClient())
            {
                ApiClient api = new ApiClient(
                    Substitute.For<IInstrumentationClient>(),
                    http);
                SerialisedEntity result = await api.RequestJsonAsync<SerialisedEntity>(
                    req,
                    false,
                    null,
                    true);
                result.Should().BeNull();
            }
        }

        [Theory]
        [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
        public void ApiClient_RequestJsonAsync_Failure(string url)
        {
            HttpRequestMessage req = new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(url)
                .Create();

            SerialisedEntity entity = new SerialisedEntity { Message = "test message" };
            string content = JsonConvert.SerializeObject(entity);

            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, url).Respond(
                r => new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(
                        content,
                        Encoding.UTF8,
                        "application/json")
                });

            using (var http = mockHttp.ToHttpClient())
            {
                ApiClient api = new ApiClient(
                    Substitute.For<IInstrumentationClient>(),
                    http);

                Action a = () =>
                {
                    SerialisedEntity _ = api.RequestJsonAsync<SerialisedEntity>(
                        req,
                        false,
                        null).Result;
                };

                a.Should().Throw<HttpRequestException>();
            }
        }


        [Theory]
        [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
        public void ApiClient_RequestJsonAsync_Failure_StartEndTraceLogged(string url)
        {
            HttpRequestMessage req = new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(url)
                .Create();

            SerialisedEntity entity = new SerialisedEntity { Message = "test message" };
            string content = JsonConvert.SerializeObject(entity);
            var instrumentationClient = Substitute.For<IInstrumentationClient>();

            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, url).Respond(
                r => new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(
                        content,
                        Encoding.UTF8,
                        "application/json")
                });

            using (var http = mockHttp.ToHttpClient())
            {
                ApiClient api = new ApiClient(instrumentationClient, http);

                Action a = () =>
                {
                    SerialisedEntity _ = api.RequestJsonAsync<SerialisedEntity>(
                        req,
                        false,
                        null).Result;
                };

                a.Should().Throw<HttpRequestException>();
                instrumentationClient.Received(1).Info(Arg.Any<string>());
            }
        }

        public class SerialisedEntity
        {
            [JsonProperty("message")]
            public string? Message { get; set; }
        }
    }
}
