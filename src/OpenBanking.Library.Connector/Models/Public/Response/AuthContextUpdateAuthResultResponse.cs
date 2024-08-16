// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

public class AuthContextUpdateAuthResultResponse
{
    public AuthContextUpdateAuthResultResponse(ConsentType consentType, Guid consentId, IList<string>? warnings)
    {
        ConsentType = consentType;
        ConsentId = consentId;
        Warnings = warnings;
    }

    /// <summary>
    ///     Type of consent.
    /// </summary>
    public ConsentType ConsentType { get; }

    /// <summary>
    ///     Consent ID.
    /// </summary>
    public Guid ConsentId { get; }

    /// <summary>
    ///     Optional list of warning messages from Open Banking Connector.
    /// </summary>
    public IList<string>? Warnings { get; }
}
