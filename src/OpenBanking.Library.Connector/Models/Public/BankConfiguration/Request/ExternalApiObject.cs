// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;

public class ExternalApiObject
{
    /// <summary>
    ///     External (bank) API ID, i.e. ID of object at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    public string ExternalApiId { get; set; } = null!;

    /// <summary>
    ///     External (bank) API secret. Present to allow use of legacy token auth method "client_secret_basic" in sandboxes
    ///     etc.
    /// </summary>
    public string? ExternalApiSecret { get; set; }

    /// <summary>
    ///     External (bank) API registration access token. Sometimes used to support registration adjustments etc.
    /// </summary>
    public string? RegistrationAccessToken { get; set; }
}
