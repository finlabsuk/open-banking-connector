// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;

public class ReadWriteGetCustomBehaviour
{
    public bool? ResponseLinksAddSlash { get; set; }

    public bool? ResponseLinksMayHaveIncorrectUrlBeforeQuery { get; set; }

    /// <summary>
    ///     Use when response links provided by bank substitute newValue for oldValue.
    /// </summary>
    public (string oldValue, string newValue)? ResponseLinksReplace { get; set; }
}
