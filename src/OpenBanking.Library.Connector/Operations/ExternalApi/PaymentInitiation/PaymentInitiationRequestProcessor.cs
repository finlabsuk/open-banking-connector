// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation
{
    internal class PaymentInitiationRequestProcessor<TVariantApiRequest> : RequestProcessor<TVariantApiRequest>
        where TVariantApiRequest : class
    {
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly JwtFactory _jwtFactory;
        private readonly string _orgId;
        private readonly PaymentInitiationApi _paymentInitiationApi;
        private readonly SoftwareStatementProfile _softwareStatementProfile;
        private readonly TokenEndpointResponse _tokenEndpointResponse;

        public PaymentInitiationRequestProcessor(
            string orgId,
            TokenEndpointResponse tokenEndpointResponse,
            SoftwareStatementProfile softwareStatementProfile,
            PaymentInitiationApi paymentInitiationApi,
            JwtFactory jwtFactory,
            IInstrumentationClient instrumentationClient)
        {
            _orgId = orgId;
            _tokenEndpointResponse = tokenEndpointResponse;
            _softwareStatementProfile = softwareStatementProfile;
            _paymentInitiationApi = paymentInitiationApi;
            _jwtFactory = jwtFactory;
            _instrumentationClient = instrumentationClient;
        }

        protected override (List<HttpHeader> headers, string body, string contentType) HttpPostRequestData(
            TVariantApiRequest variantRequest,
            string requestDescription)
        {
            // Create JWT and log
            string jwt = _jwtFactory.CreateJwt(
                GetJoseHeaders(
                    _softwareStatementProfile.SoftwareStatementPayload.OrgId,
                    _softwareStatementProfile.SoftwareStatementPayload.SoftwareId,
                    _softwareStatementProfile.SigningKeyId,
                    _paymentInitiationApi.PaymentInitiationApiVersion),
                variantRequest,
                _softwareStatementProfile.SigningKey,
                _softwareStatementProfile.SigningCertificate);
            StringBuilder requestTraceSb = new StringBuilder()
                .AppendLine($"#### JWT ({requestDescription})")
                .Append(jwt);
            _instrumentationClient.Info(requestTraceSb.ToString());

            // Assemble headers and body
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader("x-fapi-financial-id", _orgId),
                new HttpHeader("Authorization", "Bearer " + _tokenEndpointResponse.AccessToken),
                new HttpHeader("x-idempotency-key", Guid.NewGuid().ToString()),
            };
            headers.Add(CreateJwsSignatureHeader(jwt));
            string content = JsonConvert.SerializeObject(
                variantRequest,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            return (headers, content, "application/json");
        }

        private static HttpHeader CreateJwsSignatureHeader(string jwt)
        {
            // Create headers
            string[] jwsComponents = jwt.Split('.');
            string jwsSignature = $"{jwsComponents[0]}..{jwsComponents[2]}";
            return new HttpHeader("x-jws-signature", jwsSignature);
        }

        private static Dictionary<string, object> GetJoseHeaders(
            string orgId,
            string softwareId,
            string signingId,
            PaymentInitiationApiVersion paymentInitiationPaymentInitiationApiVersion)
        {
            signingId.ArgNotNull(nameof(signingId));
            orgId.ArgNotNull(nameof(orgId));
            softwareId.ArgNotNull(nameof(softwareId));

            // b64 header was removed from 3.1.4 onwards
            string[] crit;
            bool? b64;
            if (paymentInitiationPaymentInitiationApiVersion < PaymentInitiationApiVersion.Version3p1p4)
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
            dict.Add("http://openbanking.org.uk/iss", $"{orgId}/{softwareId}");
            dict.Add("http://openbanking.org.uk/tan", "openbanking.org.uk");
            if (!(b64 is null))
            {
                dict.Add("b64", b64.Value);
            }

            return dict;
        }
    }
}
