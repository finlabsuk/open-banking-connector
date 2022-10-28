// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

public class ConsentRequestBase : Base
{
    /// <summary>
    ///     Use existing external API object instead of making external API request.
    ///     The first non-null of ExternalApiObject, ExternalApiRequest, and TemplateRequest (in that order) is used
    ///     and the others are ignored. At least one of these three must be non-null.
    ///     Specifies external Consent created previously using external (bank) API request.
    ///     When non-null, a new external Consent object will
    ///     not be created at the external API.
    /// </summary>
    public ExternalApiConsent? ExternalApiObject { get; set; }
}
