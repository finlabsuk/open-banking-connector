// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class ExternalApiHttpErrorException : Exception
{
    public ExternalApiHttpErrorException(
        int responseStatusCode,
        string requestHttpMethod,
        string requestUrl,
        string responseMessage) : base("Pass-through error from external API endpoint")
    {
        ResponseStatusCode = responseStatusCode;
        RequestHttpMethod = requestHttpMethod;
        RequestUrl = requestUrl;
        ResponseMessage = responseMessage;
    }

    public int ResponseStatusCode { get; }

    public string RequestHttpMethod { get; }

    public string RequestUrl { get; }

    public string ResponseMessage { get; }
}
