// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    internal class JwtRequestProcessor<TVariantApiRequest> :
        IPostRequestProcessor<TVariantApiRequest>,
        IGetRequestProcessor
        where TVariantApiRequest : class
    {
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly ProcessedSoftwareStatementProfile _processedSoftwareStatementProfile;
        private readonly bool _useApplicationJoseNotApplicationJwtContentTypeHeader;

        public JwtRequestProcessor(
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient,
            bool useApplicationJoseNotApplicationJwtContentTypeHeader)
        {
            _processedSoftwareStatementProfile = processedSoftwareStatementProfile;
            _instrumentationClient = instrumentationClient;
            _useApplicationJoseNotApplicationJwtContentTypeHeader =
                useApplicationJoseNotApplicationJwtContentTypeHeader;
        }

        (List<HttpHeader> headers, string acceptType) IGetRequestProcessor.HttpGetRequestData(string requestDescription)
        {
            throw new NotImplementedException();
        }

        (List<HttpHeader> headers, string body, string contentType) IPostRequestProcessor<TVariantApiRequest>.
            HttpPostRequestData(
                TVariantApiRequest variantRequest,
                JsonSerializerSettings? requestJsonSerializerSettings,
                string requestDescription)
        {
            // Create JWT and log
            string jwt = JwtFactory.CreateJwt(
                JwtFactory.DefaultJwtHeadersIncludingTyp(_processedSoftwareStatementProfile.SigningKeyId),
                variantRequest,
                _processedSoftwareStatementProfile.SigningKey,
                _processedSoftwareStatementProfile.SigningCertificate,
                requestJsonSerializerSettings);
            StringBuilder requestTraceSb = new StringBuilder()
                .AppendLine($"#### Claims ({requestDescription})")
                .AppendLine(
                    JsonConvert.SerializeObject(
                        variantRequest,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                        }))
                .AppendLine($"#### JWT ({requestDescription})")
                .Append(jwt);
            _instrumentationClient.Trace(requestTraceSb.ToString());

            return (new List<HttpHeader>(), jwt,
                _useApplicationJoseNotApplicationJwtContentTypeHeader ? "application/jose" : "application/jwt");
        }
    }
}
