// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;

public delegate string GetCustomTokenScopeDelegate(RegistrationScopeEnum registrationScope);

/// <summary>
///     Class used to specify options for BankRegistration endpoints at
///     bank API.
///     Default (e.g. null) property values do not lead to changes.
/// </summary>
public class BankRegistrationPutCustomBehaviour
{
    // Override for client credentials grant token scope
    public GetCustomTokenScopeDelegate? GetCustomTokenScope { get; init; }
}
