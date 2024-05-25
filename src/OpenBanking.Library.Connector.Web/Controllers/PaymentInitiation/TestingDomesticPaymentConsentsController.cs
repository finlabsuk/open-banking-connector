// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Controllers.PaymentInitiation;

[ApiController]
[ApiExplorerSettings(GroupName = "test")]
[Tags("Domestic Payment Consent Auth Contexts")]
[Route("pisp/domestic-payment-consents")]
public class TestingDomesticPaymentConsentsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public TestingDomesticPaymentConsentsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    public static string BrowserCookieKey => "__Host-Session-Id";

    /// <summary>
    ///     Create domestic payment consent auth context and redirect to auth URL
    /// </summary>
    /// <param name="domesticPaymentConsentId">ID of domestic payment consent</param>
    /// <param name="modifiedBy"></param>
    /// <param name="reference"></param>
    /// <returns></returns>
    [HttpGet("{domesticPaymentConsentId:guid}/auth")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<ActionResult> GetAsync(
        Guid domesticPaymentConsentId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy,
        [FromHeader(Name = "reference")]
        string? reference)
    {
        // Create auth context
        var authContextRequest = new DomesticPaymentConsentAuthContext
        {
            DomesticPaymentConsentId = domesticPaymentConsentId,
            Reference = reference,
            CreatedBy = modifiedBy
        };
        DomesticPaymentConsentAuthContextCreateResponse authContextResponse =
            await _requestBuilder
                .PaymentInitiation
                .DomesticPaymentConsents
                .AuthContexts
                .CreateLocalAsync(authContextRequest);

        // Set cookie
        Response.Cookies.Append(
            BrowserCookieKey,
            authContextResponse.AppSessionId,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax, // require cookie in OAuth2 redirect
                MaxAge = TimeSpan.FromMinutes(10),
                Secure = true
            });

        return Redirect(authContextResponse.AuthUrl);
    }
}
