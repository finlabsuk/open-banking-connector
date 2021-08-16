// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.Connector.UkRwApi.V3p1p4.PaymentInitiation.Models;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.UkRwApi.V3p1p6.PaymentInitiation.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentInitiationApiVersion
    {
        [EnumMember(Value = "v3p1p4")] Version3p1p4,
        [EnumMember(Value = "v3p1p6")] Version3p1p6,
        Default = Version3p1p6
    }

    public static class PaymentInitiationApiVersionExtensions
    {
        public static Type DomesticPaymentApiRequestType(this PaymentInitiationApiVersion apiVersion)
        {
            return apiVersion switch
            {
                PaymentInitiationApiVersion.Version3p1p4 => typeof(PaymentInitiationModelsV3p1p4.OBWriteDomestic2),
                PaymentInitiationApiVersion.Version3p1p6 => typeof(PaymentInitiationModelsPublic.OBWriteDomestic2),
                _ => throw new ArgumentOutOfRangeException(nameof(apiVersion), apiVersion, null)
            };
        }
    }
}
