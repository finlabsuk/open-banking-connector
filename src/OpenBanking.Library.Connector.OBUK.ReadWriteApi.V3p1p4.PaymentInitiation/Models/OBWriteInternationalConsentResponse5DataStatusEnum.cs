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
    /// Defines values for OBWriteInternationalConsentResponse5DataStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalConsentResponse5DataStatusEnum
    {
        [EnumMember(Value = "Authorised")]
        Authorised,
        [EnumMember(Value = "AwaitingAuthorisation")]
        AwaitingAuthorisation,
        [EnumMember(Value = "Consumed")]
        Consumed,
        [EnumMember(Value = "Rejected")]
        Rejected
    }
    internal static class OBWriteInternationalConsentResponse5DataStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalConsentResponse5DataStatusEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalConsentResponse5DataStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalConsentResponse5DataStatusEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalConsentResponse5DataStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteInternationalConsentResponse5DataStatusEnum.AwaitingAuthorisation:
                    return "AwaitingAuthorisation";
                case OBWriteInternationalConsentResponse5DataStatusEnum.Consumed:
                    return "Consumed";
                case OBWriteInternationalConsentResponse5DataStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteInternationalConsentResponse5DataStatusEnum? ParseOBWriteInternationalConsentResponse5DataStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteInternationalConsentResponse5DataStatusEnum.Authorised;
                case "AwaitingAuthorisation":
                    return OBWriteInternationalConsentResponse5DataStatusEnum.AwaitingAuthorisation;
                case "Consumed":
                    return OBWriteInternationalConsentResponse5DataStatusEnum.Consumed;
                case "Rejected":
                    return OBWriteInternationalConsentResponse5DataStatusEnum.Rejected;
            }
            return null;
        }
    }
}
