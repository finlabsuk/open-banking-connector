// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;

internal class PaymentInitiationPostRequestProcessor<TVariantApiRequest> : IPostRequestProcessor<TVariantApiRequest>
    where TVariantApiRequest : class
{
    private readonly string _accessToken;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly string _orgId;
    private readonly ProcessedSoftwareStatementProfile _processedSoftwareStatementProfile;
    private readonly bool _useB64;

    public PaymentInitiationPostRequestProcessor(
        string orgId,
        string accessToken,
        IInstrumentationClient instrumentationClient,
        bool useB64,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile)
    {
        _instrumentationClient = instrumentationClient;
        _orgId = orgId;
        _useB64 = useB64;
        _processedSoftwareStatementProfile = processedSoftwareStatementProfile;
        _accessToken = accessToken;
    }

    (List<HttpHeader> headers, string body, string contentType) IPostRequestProcessor<TVariantApiRequest>.
        HttpPostRequestData(
            TVariantApiRequest variantRequest,
            JsonSerializerSettings? requestJsonSerializerSettings,
            string requestDescription)
    {
        // Create JWT and log
        string jwt = JwtFactory.CreateJwt(
            GetJoseHeaders(
                _processedSoftwareStatementProfile.SoftwareStatementPayload.OrgId,
                _processedSoftwareStatementProfile.SoftwareStatementPayload.SoftwareId,
                _processedSoftwareStatementProfile.SigningKeyId,
                _useB64),
            variantRequest,
            _processedSoftwareStatementProfile.SigningKey);
        StringBuilder requestTraceSb = new StringBuilder()
            .AppendLine($"#### JWT ({requestDescription})")
            .Append(jwt);
        _instrumentationClient.Trace(requestTraceSb.ToString());

        // Assemble headers and body
        var headers = new List<HttpHeader>
        {
            new("x-fapi-financial-id", _orgId),
            new("Authorization", "Bearer " + _accessToken),
            new("x-idempotency-key", Guid.NewGuid().ToString()),
        };
        headers.Add(CreateJwsSignatureHeader(jwt));
        JsonSerializerSettings jsonSerializerSettings =
            requestJsonSerializerSettings ?? new JsonSerializerSettings();
        jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        string content = JsonConvert.SerializeObject(
            variantRequest,
            jsonSerializerSettings);
        return (headers, content, "application/json");
    }

    private static HttpHeader CreateJwsSignatureHeader(string jwt)
    {
        // Create headers
        string[] jwsComponents = jwt.Split('.');
        var jwsSignature = $"{jwsComponents[0]}..{jwsComponents[2]}";
        return new HttpHeader("x-jws-signature", jwsSignature);
    }

    private static Dictionary<string, object> GetJoseHeaders(
        string orgId,
        string softwareId,
        string signingId,
        bool useB64)
    {
        signingId.ArgNotNull(nameof(signingId));
        orgId.ArgNotNull(nameof(orgId));
        softwareId.ArgNotNull(nameof(softwareId));

        // b64 header was removed from 3.1.4 onwards
        string[] crit;
        bool? b64;
        if (useB64)
        {
            crit = new[]
            {
                "http://openbanking.org.uk/iat", "http://openbanking.org.uk/iss",
                "http://openbanking.org.uk/tan", "b64"
            };
            b64 = false;
        }
        else
        {
            crit = new[]
            {
                "http://openbanking.org.uk/iat", "http://openbanking.org.uk/iss",
                "http://openbanking.org.uk/tan"
            };
            b64 = null;
        }

        Dictionary<string, object> dict = JwtFactory.DefaultJwtHeadersExcludingTyp(signingId);
        dict.Add("cty", "application/json");
        dict.Add("crit", crit);
        dict.Add("http://openbanking.org.uk/iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        dict.Add(
            "http://openbanking.org.uk/iss",
            $"{orgId}/{softwareId}"); // TODO: adjust. See HSBC implementation guide
        dict.Add("http://openbanking.org.uk/tan", "openbanking.org.uk");
        if (!(b64 is null))
        {
            dict.Add("b64", b64.Value);
        }

        return dict;
    }
}
