// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.BankConfiguration;

internal class BankRegistrationPostRequestProcessor<TVariantApiRequest> :
    IPostRequestProcessor<TVariantApiRequest>
    where TVariantApiRequest : class
{
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly OBSealKey _obSealKey;
    private readonly bool _useApplicationJoseNotApplicationJwtContentTypeHeader;

    public BankRegistrationPostRequestProcessor(
        OBSealKey obSealKey,
        IInstrumentationClient instrumentationClient,
        bool useApplicationJoseNotApplicationJwtContentTypeHeader)
    {
        _obSealKey = obSealKey;
        _instrumentationClient = instrumentationClient;
        _useApplicationJoseNotApplicationJwtContentTypeHeader =
            useApplicationJoseNotApplicationJwtContentTypeHeader;
    }

    (List<HttpHeader> headers, string body, string contentType) IPostRequestProcessor<TVariantApiRequest>.
        HttpPostRequestData(
            TVariantApiRequest variantRequest,
            JsonSerializerSettings? requestJsonSerializerSettings,
            string requestDescription,
            IEnumerable<HttpHeader>? extraHeaders)
    {
        // Create JWT and log
        JsonSerializerSettings jsonSerializerSettings =
            requestJsonSerializerSettings ?? new JsonSerializerSettings();
        jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        string payloadJson = JsonConvert.SerializeObject(
            variantRequest,
            jsonSerializerSettings);
        string jwt = JwtFactory.CreateJwt(
            JwtFactory.DefaultJwtHeadersIncludingTyp(_obSealKey.KeyId),
            payloadJson,
            _obSealKey.Key,
            null);
        StringBuilder requestTraceSb = new StringBuilder()
            .AppendLine($"#### Claims ({requestDescription})")
            .AppendLine(
                JsonConvert.SerializeObject(
                    variantRequest,
                    Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }))
            .AppendLine($"#### JWT ({requestDescription})")
            .Append(jwt);
        _instrumentationClient.Trace(requestTraceSb.ToString());

        var headers = new List<HttpHeader>();
        if (extraHeaders is not null)
        {
            foreach (HttpHeader header in extraHeaders)
            {
                headers.Add(header);
            }
        }

        return (headers, jwt,
            _useApplicationJoseNotApplicationJwtContentTypeHeader ? "application/jose" : "application/jwt");
    }
}
