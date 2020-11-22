// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p4.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for OBChargeBearerType1CodeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBChargeBearerType1CodeEnum
    {
        [EnumMember(Value = "BorneByCreditor")]
        BorneByCreditor,
        [EnumMember(Value = "BorneByDebtor")]
        BorneByDebtor,
        [EnumMember(Value = "FollowingServiceLevel")]
        FollowingServiceLevel,
        [EnumMember(Value = "Shared")]
        Shared
    }
    internal static class OBChargeBearerType1CodeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBChargeBearerType1CodeEnum? value)
        {
            return value == null ? null : ((OBChargeBearerType1CodeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBChargeBearerType1CodeEnum value)
        {
            switch( value )
            {
                case OBChargeBearerType1CodeEnum.BorneByCreditor:
                    return "BorneByCreditor";
                case OBChargeBearerType1CodeEnum.BorneByDebtor:
                    return "BorneByDebtor";
                case OBChargeBearerType1CodeEnum.FollowingServiceLevel:
                    return "FollowingServiceLevel";
                case OBChargeBearerType1CodeEnum.Shared:
                    return "Shared";
            }
            return null;
        }

        internal static OBChargeBearerType1CodeEnum? ParseOBChargeBearerType1CodeEnum(this string value)
        {
            switch( value )
            {
                case "BorneByCreditor":
                    return OBChargeBearerType1CodeEnum.BorneByCreditor;
                case "BorneByDebtor":
                    return OBChargeBearerType1CodeEnum.BorneByDebtor;
                case "FollowingServiceLevel":
                    return OBChargeBearerType1CodeEnum.FollowingServiceLevel;
                case "Shared":
                    return OBChargeBearerType1CodeEnum.Shared;
            }
            return null;
        }
    }
}
