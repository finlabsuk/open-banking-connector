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
        string deserialisationErrorMessage) : base(
        "De-serialisation error when converting data from external API endpoint",
        responseStatusCode,
        requestHttpMethod,
        requestUrl,
        responseMessage)
    {
        DeserialisationErrorMessage = deserialisationErrorMessage;
    }

    public string DeserialisationErrorMessage { get; }
}
