// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web;

public class ExternalApiHttpErrorExceptionFilter : IActionFilter, IOrderedFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is ExternalApiAccessException httpResponseException)
        {
            var jsonObject = new JsonObject
            {
                ["title"] = httpResponseException.Message,
                ["detail"] =
                    $"External API endpoint responded with HTTP status code {httpResponseException.ResponseStatusCode}. See properties " +
                    "'endpointHttpMethod', 'endpointUrl' and 'endpointResponse' for more details.",
                ["status"] = httpResponseException.ResponseStatusCode,
                ["endpointHttpMethod"] = httpResponseException.RequestHttpMethod,
                ["endpointUrl"] = httpResponseException.RequestUrl,
            };

            if (context.Exception is ExternalApiResponseDeserialisationException ex)
            {
                jsonObject["deserialisationError"] = ex.DeserialisationErrorMessage;
            }

            JsonNode? responseMessage = JsonNode.Parse(httpResponseException.ResponseMessage);
            if (responseMessage is not null)
            {
                jsonObject["endpointResponse"] = responseMessage;
            }
            else
            {
                jsonObject["endpointResponse"] = httpResponseException.ResponseMessage;
            }

            string jsonString = JsonSerializer.Serialize(jsonObject);

            MediaTypeHeaderValue mediaTypeHeaderValue = MediaTypeHeaderValue.Parse("application/problem+json");
            mediaTypeHeaderValue.Encoding = Encoding.UTF8;
            context.Result = new ContentResult
            {
                Content = jsonString,
                ContentType = mediaTypeHeaderValue.ToString(),
                StatusCode = httpResponseException.ResponseStatusCode
            };

            context.ExceptionHandled = true;
        }
    }

    public int Order => int.MaxValue - 10;
}
