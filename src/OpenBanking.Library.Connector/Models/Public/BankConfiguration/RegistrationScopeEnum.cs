// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;

/// <summary>
///     Registration scope used when creating a bank client.
///     Set of <see cref="RegistrationScopeElementEnum" />.
/// </summary>
[Flags]
[JsonConverter(typeof(StringEnumConverter))]
public enum RegistrationScopeEnum
{
    [EnumMember(Value = "None")]
    None = 0,

    [EnumMember(Value = "AccountAndTransaction")]
    AccountAndTransaction = 1,

    [EnumMember(Value = "PaymentInitiation")]
    PaymentInitiation = 2,

    [EnumMember(Value = "FundsConfirmation")]
    FundsConfirmation = 4,

    [EnumMember(Value = "All")]
    All = AccountAndTransaction | PaymentInitiation | FundsConfirmation
}

public static class RegistrationScopeExtensions
{
    /// <summary>
    ///     Abbreviated name for registration scope
    /// </summary>
    /// <param name="registrationScope"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string AbbreviatedName(this RegistrationScopeEnum registrationScope)
    {
        return registrationScope switch
        {
            // RegistrationScope.None: invalid
            RegistrationScopeEnum.AccountAndTransaction => "AT",
            RegistrationScopeEnum.PaymentInitiation => "PI",
            RegistrationScopeEnum.FundsConfirmation => "FC",
            RegistrationScopeEnum.All => "All",
            _ => throw new ArgumentOutOfRangeException(nameof(registrationScope), registrationScope, null)
        };
    }
}
