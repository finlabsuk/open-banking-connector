// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;

/// <summary>
///     Class used to specify overrides to returned OpenID configuration (from
///     "/.well-known/openid-configuration" endpoint).
///     Sometimes values are missing and they can be added using this class to allow
///     OBC to have necessary information about a bank.
///     Default (null) property values do not lead to changes.
/// </summary>
public class OpenIdConfigurationGetCustomBehaviour
{
    /// <summary>
    ///     Explicit URL to use for well-known endpoint instead of that created using Issuer URL.
    /// </summary>
    public string? Url { get; set; }

    public bool? EndpointUnavailable { get; set; }

    public IList<OAuth2ResponseMode>? ResponseModesSupportedResponse { get; set; }

    public IList<OpenIdConfigurationTokenEndpointAuthMethodEnum>? TokenEndpointAuthMethodsSupportedResponse
    {
        get;
        set;
    }
}
