// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum
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
    internal static class OBWriteInternationalStandingOrderConsentResponse6DataStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum.AwaitingAuthorisation:
                    return "AwaitingAuthorisation";
                case OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum.Consumed:
                    return "Consumed";
                case OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum? ParseOBWriteInternationalStandingOrderConsentResponse6DataStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum.Authorised;
                case "AwaitingAuthorisation":
                    return OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum.AwaitingAuthorisation;
                case "Consumed":
                    return OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum.Consumed;
                case "Rejected":
                    return OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum.Rejected;
            }
            return null;
        }
    }
}
