// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Security;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    internal class JwtRequestProcessor<TVariantApiRequest> : RequestProcessor<TVariantApiRequest>
        where TVariantApiRequest : class
    {
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly JwtFactory _jwtFactory;
        private readonly SoftwareStatementProfile _softwareStatementProfile;

        public JwtRequestProcessor(
            SoftwareStatementProfile softwareStatementProfile,
            JwtFactory jwtFactory,
            IInstrumentationClient instrumentationClient)
        {
            _softwareStatementProfile = softwareStatementProfile;
            _jwtFactory = jwtFactory;
            _instrumentationClient = instrumentationClient;
        }

        protected override (List<HttpHeader> headers, string body, string contentType) HttpPostRequestData(
            TVariantApiRequest variantRequest,
            string requestDescription)
        {
            // Create JWT and log
            string jwt = _jwtFactory.CreateJwt(
                JwtFactory.DefaultJwtHeadersIncludingTyp(_softwareStatementProfile.SigningKeyId),
                variantRequest,
                _softwareStatementProfile.SigningKey,
                _softwareStatementProfile.SigningCertificate);
            StringBuilder requestTraceSb = new StringBuilder()
                .AppendLine($"#### JWT ({requestDescription})")
                .Append(jwt);
            _instrumentationClient.Info(requestTraceSb.ToString());

            return (new List<HttpHeader>(), jwt, "application/jwt");
        }
    }
}
