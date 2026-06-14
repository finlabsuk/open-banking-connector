// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class ExternalApiResponseDeserialisationException : ExternalApiAccessException
{
    public ExternalApiResponseDeserialisationException(
        int responseStatusCode,
        string requestHttpMethod,
        string requestUrl,
        string responseMessage,
        string? xFapiInteractionId,
        string deserialisationErrorMessage,
        bool exposeSuccessResponseBody) : base(
        "De-serialisation error when converting data from external API endpoint",
        responseStatusCode,
        requestHttpMethod,
        requestUrl,
        responseMessage,
        xFapiInteractionId)
    {
        DeserialisationErrorMessage = deserialisationErrorMessage;
        ExposeSuccessResponseBody = exposeSuccessResponseBody;
    }

    public string DeserialisationErrorMessage { get; }

    public bool ExposeSuccessResponseBody { get; }
}
