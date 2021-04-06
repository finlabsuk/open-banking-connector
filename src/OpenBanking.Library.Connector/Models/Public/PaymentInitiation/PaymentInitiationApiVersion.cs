﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentInitiationApiVersion
    {
        [EnumMember(Value = "v3p1p4")] Version3p1p4,
        [EnumMember(Value = "v3p1p6")] Version3p1p6
    }
}
