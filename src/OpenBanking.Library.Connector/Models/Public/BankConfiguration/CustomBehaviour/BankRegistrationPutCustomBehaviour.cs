// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;

/// <summary>
///     Class used to specify options for BankRegistration endpoints at
///     bank API.
///     Default (e.g. null) property values do not lead to changes.
/// </summary>
public class BankRegistrationPutCustomBehaviour
{
    // Override for client credentials grant token scope
    public string? CustomTokenScope { get; set; }
}
