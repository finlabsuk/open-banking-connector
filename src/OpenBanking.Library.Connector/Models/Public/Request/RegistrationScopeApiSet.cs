// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    /// <summary>
    ///     Set of <see cref="RegistrationScopeApi" />.
    /// </summary>
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RegistrationScopeApiSet
    {
        [EnumMember(Value = "none")] None = 0,

        [EnumMember(Value = "accountAndTransaction")]
        AccountAndTransaction = 1,

        [EnumMember(Value = "paymentInitiation")]
        PaymentInitiation = 2,

        [EnumMember(Value = "fundsConfirmation")]
        FundsConfirmation = 4,

        [EnumMember(Value = "all")] All = AccountAndTransaction | PaymentInitiation | FundsConfirmation,
    }

    public static class RegistrationScopeApiSetHelper
    {
        /// <summary>
        ///     Mapping from API set to abbreviated name
        /// </summary>
        public static string AbbreviatedName(RegistrationScopeApiSet registrationScopeApiSet) =>
            registrationScopeApiSet switch
            {
                RegistrationScopeApiSet.AccountAndTransaction => "AT",
                RegistrationScopeApiSet.PaymentInitiation => "PI",
                RegistrationScopeApiSet.FundsConfirmation => "FC",
                RegistrationScopeApiSet.All => "All",
                _ => throw new ArgumentException(
                    $"{nameof(registrationScopeApiSet)} is not valid ${nameof(RegistrationScopeApiSet)} or needs to be added to this switch statement.")
            };
    }
}
