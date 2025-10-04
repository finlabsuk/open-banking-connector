// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;

public delegate (string oldValue, string newValue)? GetResponseLinksAllowReplace(bool useV4NotV3);

public class ReadWriteGetCustomBehaviour
{
    public bool? ResponseLinksMayAddSlash { get; set; }

    public bool? ResponseLinksMayHaveIncorrectUrlBeforeQuery { get; set; }

    /// <summary>
    ///     Use when response links provided by bank substitute newValue for oldValue.
    /// </summary>
    public GetResponseLinksAllowReplace? GetResponseLinksAllowReplace { get; set; }
}
