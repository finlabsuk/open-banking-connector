// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    /// <summary>
    ///     UK Open Banking API configured in bank registration scope
    /// </summary>
    public enum RegistrationScopeApi
    {
        [EnumMember(Value = "accountAndTransaction")]
        AccountAndTransaction,

        [EnumMember(Value = "paymentInitiation")]
        PaymentInitiation,

        [EnumMember(Value = "fundsConfirmation")]
        FundsConfirmation,
    }

    public static class RegistrationScopeApiHelper
    {
        static RegistrationScopeApiHelper()
        {
            AllApiTypes = Enum.GetValues(typeof(RegistrationScopeApi))
                .Cast<RegistrationScopeApi>();
        }

        public static IEnumerable<RegistrationScopeApi> AllApiTypes { get; }

        public static RegistrationScopeApiSet ApiTypeSetWithSingleApiType(RegistrationScopeApi registrationScopeApi) =>
            registrationScopeApi switch
            {
                RegistrationScopeApi.AccountAndTransaction => RegistrationScopeApiSet.AccountAndTransaction,
                RegistrationScopeApi.PaymentInitiation => RegistrationScopeApiSet.PaymentInitiation,
                RegistrationScopeApi.FundsConfirmation => RegistrationScopeApiSet.FundsConfirmation,
                _ => throw new ArgumentException(
                    $"{nameof(registrationScopeApi)} is not valid ApiType or needs to be added to this switch statement.")
            };

        /// <summary>
        ///     Mapping from individual API type to scope word
        /// </summary>
        public static string ScopeWord(RegistrationScopeApi registrationScopeApi) =>
            registrationScopeApi switch
            {
                RegistrationScopeApi.AccountAndTransaction => "accounts",
                RegistrationScopeApi.PaymentInitiation => "payments",
                RegistrationScopeApi.FundsConfirmation => "fundsconfirmations",
                _ => throw new ArgumentException(
                    $"{nameof(registrationScopeApi)} is not valid ApiType or needs to be added to this switch statement.")
            };
    }
}
