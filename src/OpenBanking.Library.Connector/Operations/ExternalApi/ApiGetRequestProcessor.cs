// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal class ApiGetRequestProcessor : IGetRequestProcessor
{
    private readonly string _accessToken;
    private readonly string _financialId;


    public ApiGetRequestProcessor(string financialId, string accessToken)
    {
        _financialId = financialId;
        _accessToken = accessToken;
    }

    (List<HttpHeader> headers, string acceptType) IGetRequestProcessor.HttpGetRequestData(
        string requestDescription,
        IEnumerable<HttpHeader>? extraHeaders)
    {
        // Assemble headers and body
        var headers = new List<HttpHeader>
        {
            new("Authorization", "Bearer " + _accessToken),
            new("x-fapi-financial-id", _financialId)
        };
        if (extraHeaders is not null)
        {
            foreach (HttpHeader header in extraHeaders)
            {
                headers.Add(header);
            }
        }

        return (headers, "application/json");
    }
}
