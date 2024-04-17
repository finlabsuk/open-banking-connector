// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;

public abstract class GrantPostCustomBehaviour
{
    public bool? ScopeResponseIsEmptyString { get; set; }

    public bool? ScopeResponseMayIncludeExtraValues { get; set; }

    public bool? TokenTypeResponseStartsWithLowerCaseLetter { get; set; }
}
