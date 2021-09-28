// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Pisp.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for OBWriteDomesticConsentResponse5DataStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticConsentResponse5DataStatusEnum
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
    internal static class OBWriteDomesticConsentResponse5DataStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticConsentResponse5DataStatusEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticConsentResponse5DataStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticConsentResponse5DataStatusEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticConsentResponse5DataStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteDomesticConsentResponse5DataStatusEnum.AwaitingAuthorisation:
                    return "AwaitingAuthorisation";
                case OBWriteDomesticConsentResponse5DataStatusEnum.Consumed:
                    return "Consumed";
                case OBWriteDomesticConsentResponse5DataStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteDomesticConsentResponse5DataStatusEnum? ParseOBWriteDomesticConsentResponse5DataStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteDomesticConsentResponse5DataStatusEnum.Authorised;
                case "AwaitingAuthorisation":
                    return OBWriteDomesticConsentResponse5DataStatusEnum.AwaitingAuthorisation;
                case "Consumed":
                    return OBWriteDomesticConsentResponse5DataStatusEnum.Consumed;
                case "Rejected":
                    return OBWriteDomesticConsentResponse5DataStatusEnum.Rejected;
            }
            return null;
        }
    }
}