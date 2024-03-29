﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var messages = new HttpResponseMessages
        {
            InformationMessages = null,
            WarningMessages = null,
            ErrorMessages = new List<string> { context.Exception.Message }
        };

        // Catch unhandled input validation errors
        if (context.Exception is ArgumentNullException)
        {
            context.Result = new ObjectResult(messages) { StatusCode = StatusCodes.Status400BadRequest };
        }
    }
}
