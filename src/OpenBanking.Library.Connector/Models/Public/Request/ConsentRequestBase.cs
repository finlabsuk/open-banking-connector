// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

public class ConsentRequestBase : Base
{
    /// <summary>
    ///     Information about external Consent created using external (bank) API.
    ///     When non-null, this will be referenced
    ///     instead of
    ///     creating a new external Consent object at the external API.
    /// </summary>
    public ExternalApiConsent? ExternalApiObject { get; set; }
}
