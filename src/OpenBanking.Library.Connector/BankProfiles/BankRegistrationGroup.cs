// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

[JsonConverter(typeof(StringEnumConverter))]
public enum BankRegistrationGroup
{

    [EnumMember(Value = "Barclays_Sandbox")]
    Barclays_Sandbox,

    [EnumMember(Value = "Barclays_Production")]
    Barclays_Production,
    
    [EnumMember(Value = "NatWest_NatWestSandbox")]
    NatWest_NatWestSandbox,
    
    [EnumMember(Value = "NatWest_NatWestProduction")]
    NatWest_NatWestProduction,
    
    [EnumMember(Value = "NatWest_RoyalBankOfScotlandSandbox")]
    NatWest_RoyalBankOfScotlandSandbox,
    
    [EnumMember(Value = "NatWest_RoyalBankOfScotlandProduction")]
    NatWest_RoyalBankOfScotlandProduction,
    
    [EnumMember(Value = "NatWest_UlsterBankNiProduction")]
    NatWest_UlsterBankNiProduction

}
