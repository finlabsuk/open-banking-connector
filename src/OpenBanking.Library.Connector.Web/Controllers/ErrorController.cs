// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    [Route("/error")]
    public IActionResult HandleError()
    {
        // Exception exception =
        //     HttpContext.Features.Get<IExceptionHandlerFeature>()!.Error;

        // Catch unhandled input validation errors
        // if (exception is ArgumentNullException)
        // {
        //     var problemDetails = new ProblemDetails
        //     {
        //         Detail = exception.Message,
        //         Status = StatusCodes.Status400BadRequest,
        //         Title = "An input argument was null."
        //     };
        //
        //     return new ObjectResult(problemDetails)
        //     {
        //         StatusCode = StatusCodes.Status400BadRequest
        //     };
        // }

        // Default to returning no details
        return Problem();
    }
}
