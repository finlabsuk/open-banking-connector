// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
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
}
