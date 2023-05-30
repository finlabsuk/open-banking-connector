﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

public class AuthContextRequest
{
    /// <summary>
    ///     OAuth2 "state".
    /// </summary>
    public string State { get; set; } = null!;

    /// <summary>
    ///     OpenID Connect "nonce".
    /// </summary>
    public string Nonce { get; set; } = null!;

    public string? ModifiedBy { get; set; }
}

public class ExternalApiConsent
{
    /// <summary>
    ///     External (bank) API ID, i.e. ID of object at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    public string ExternalApiId { get; set; } = null!;

    public AuthContextRequest? AuthContext { get; set; }
}
