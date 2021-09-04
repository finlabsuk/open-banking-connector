// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Security;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    internal class JwtRequestProcessor<TVariantApiRequest> :
        IPostRequestProcessor<TVariantApiRequest>,
        IGetRequestProcessor
        where TVariantApiRequest : class
    {
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly ProcessedSoftwareStatementProfile _processedSoftwareStatementProfile;

        public JwtRequestProcessor(
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient)
        {
            _processedSoftwareStatementProfile = processedSoftwareStatementProfile;
            _instrumentationClient = instrumentationClient;
        }

        (List<HttpHeader> headers, string acceptType) IGetRequestProcessor.HttpGetRequestData(string requestDescription)
        {
            throw new NotImplementedException();
        }

        (List<HttpHeader> headers, string body, string contentType) IPostRequestProcessor<TVariantApiRequest>.
            HttpPostRequestData(
                TVariantApiRequest variantRequest,
                string requestDescription)
        {
            // Create JWT and log
            string jwt = JwtFactory.CreateJwt(
                JwtFactory.DefaultJwtHeadersIncludingTyp(_processedSoftwareStatementProfile.SigningKeyId),
                variantRequest,
                _processedSoftwareStatementProfile.SigningKey,
                _processedSoftwareStatementProfile.SigningCertificate);
            StringBuilder requestTraceSb = new StringBuilder()
                .AppendLine($"#### JWT ({requestDescription})")
                .Append(jwt);
            _instrumentationClient.Info(requestTraceSb.ToString());

            return (new List<HttpHeader>(), jwt, "application/jwt");
        }
    }
}
