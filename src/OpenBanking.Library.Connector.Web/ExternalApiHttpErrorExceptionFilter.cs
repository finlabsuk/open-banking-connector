// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web;

public class ExternalApiHttpErrorExceptionFilter : IActionFilter, IOrderedFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is ExternalApiHttpErrorException httpResponseException)
        {
            var problemDetails = new ProblemDetails
            {
                Detail =
                    $"Endpoint {httpResponseException.RequestHttpMethod} {httpResponseException.RequestUrl} " +
                    "responded with an error. Property 'message' on this object conveys any JSON or plain text received.",
                Status = httpResponseException.ResponseStatusCode,
                Title = httpResponseException.Message,
            };

            problemDetails.Extensions.Add("message", $"{httpResponseException.ResponseMessage}");

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = httpResponseException.ResponseStatusCode
            };

            context.ExceptionHandled = true;
        }
    }

    public int Order => int.MaxValue - 10;
}
