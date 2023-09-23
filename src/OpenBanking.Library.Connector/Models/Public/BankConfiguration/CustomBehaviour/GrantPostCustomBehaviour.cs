// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;

public class GrantPostCustomBehaviour
{
    public bool? DoNotValidateIdToken { get; set; }

    public bool? ScopeResponseIsEmptyString { get; set; }

    public bool? AllowNullResponseRefreshToken { get; set; }

    public bool? TokenTypeResponseStartsWithLowerCaseLetter { get; set; }

    public bool? DoNotValidateIdTokenAcrClaim { get; set; }
}
