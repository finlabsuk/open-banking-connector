// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Controllers.VariableRecurringPayments;

[ApiController]
[ApiExplorerSettings(GroupName = "test")]
[Tags("Domestic VRP Consent Auth Contexts")]
[Route("vrp/domestic-vrp-consents")]
public class TestingDomesticVrpConsentsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public TestingDomesticVrpConsentsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    public static string BrowserCookieKey => "__Host-Session-Id";

    /// <summary>
    ///     Create domestic VRP consent auth context and redirect to auth URL
    /// </summary>
    /// <param name="domesticVrpConsentId">ID of domestic VRP consent</param>
    /// <param name="modifiedBy"></param>
    /// <param name="reference"></param>
    /// <returns></returns>
    [HttpGet("{domesticVrpConsentId:guid}/auth")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<ActionResult> GetAsync(
        Guid domesticVrpConsentId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy,
        [FromHeader(Name = "reference")]
        string? reference)
    {
        // Create auth context

        // Register state parameter
        DomesticVrpConsentAuthContextCreateResponse authContextResponse;
        if (TestingMethods.Instance.CreateDomesticVrpConsentAuthContext is not null)
        {
            authContextResponse =
                await TestingMethods.Instance.CreateDomesticVrpConsentAuthContext(domesticVrpConsentId);
        }
        else
        {
            var authContextRequest = new DomesticVrpConsentAuthContext
            {
                DomesticVrpConsentId = domesticVrpConsentId,
                Reference = reference,
                CreatedBy = modifiedBy
            };
            authContextResponse = await _requestBuilder
                .VariableRecurringPayments
                .DomesticVrpConsents
                .AuthContexts
                .CreateLocalAsync(authContextRequest);
        }

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
