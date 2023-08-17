// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;

/// <summary>
///     UK Open Banking API configured in bank registration scope
/// </summary>
public enum RegistrationScopeElementEnum
{
    [EnumMember(Value = "AccountAndTransaction")]
    AccountAndTransaction,

    [EnumMember(Value = "PaymentInitiation")]
    PaymentInitiation,

    [EnumMember(Value = "FundsConfirmation")]
    FundsConfirmation
}

public static class RegistrationScopeApiHelper
{
    static RegistrationScopeApiHelper()
    {
        AllApiTypes = Enum.GetValues(typeof(RegistrationScopeElementEnum))
            .Cast<RegistrationScopeElementEnum>();
    }

    public static IEnumerable<RegistrationScopeElementEnum> AllApiTypes { get; }

    public static RegistrationScopeEnum ApiTypeSetWithSingleApiType(
        RegistrationScopeElementEnum registrationScopeElement) =>
        registrationScopeElement switch
        {
            RegistrationScopeElementEnum.AccountAndTransaction => RegistrationScopeEnum.AccountAndTransaction,
            RegistrationScopeElementEnum.PaymentInitiation => RegistrationScopeEnum.PaymentInitiation,
            RegistrationScopeElementEnum.FundsConfirmation => RegistrationScopeEnum.FundsConfirmation,
            _ => throw new ArgumentException(
                $"{nameof(registrationScopeElement)} is not valid ApiType or needs to be added to this switch statement.")
        };

    /// <summary>
    ///     Mapping from individual API type to scope word
    /// </summary>
    public static string ScopeWord(RegistrationScopeElementEnum registrationScopeElement) =>
        registrationScopeElement switch
        {
            RegistrationScopeElementEnum.AccountAndTransaction => "accounts",
            RegistrationScopeElementEnum.PaymentInitiation => "payments",
            RegistrationScopeElementEnum.FundsConfirmation => "fundsconfirmations",
            _ => throw new ArgumentException(
                $"{nameof(registrationScopeElement)} is not valid ApiType or needs to be added to this switch statement.")
        };
}
