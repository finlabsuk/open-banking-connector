// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Filters;

public class GlobalExceptionFilter(ProblemDetailsFactory problemDetailsFactory) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is HttpResponseException exception)
        {
            ServerError serverError = exception.ServerError;
            ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails(
                context.HttpContext,
                serverError.StatusCode,
                serverError.Title,
                null,
                serverError.Detail);

            foreach ((string key, object? value) in serverError.Extensions)
            {
                problemDetails.Extensions[key.ToCamelCase()] = value;
            }

            problemDetails.Extensions["serverErrorType"] = serverError.ServerErrorType.ToString().ToCamelCase();

            context.Result = new ObjectResult(problemDetails) { StatusCode = serverError.StatusCode };
            context.ExceptionHandled = true;
        }
    }
}
