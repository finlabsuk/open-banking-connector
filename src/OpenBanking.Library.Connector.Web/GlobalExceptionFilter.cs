// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web;

public class GlobalExceptionFilter(ProblemDetailsFactory problemDetailsFactory) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is HttpResponseException exception)
        {
            ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails(
                context.HttpContext,
                exception.StatusCode,
                GetTitleString(exception.Title),
                null,
                exception.Message);
            if (exception.Extensions is not null)
            {
                problemDetails.Extensions = exception.Extensions;
            }

            context.Result = new ObjectResult(problemDetails) { StatusCode = exception.StatusCode };
            context.ExceptionHandled = true;
        }
    }

    private static string GetTitleString(ProblemDetailsTitle title)
    {
        var titleString = title.ToString();
        if (string.IsNullOrEmpty(titleString) ||
            char.IsLower(titleString[0]))
        {
            return titleString;
        }
        return char.ToLower(titleString[0]) + titleString[1..];
    }
}
