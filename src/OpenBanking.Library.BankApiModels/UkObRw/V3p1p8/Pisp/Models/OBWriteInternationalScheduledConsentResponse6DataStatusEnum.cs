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
    /// Defines values for
    /// OBWriteInternationalScheduledConsentResponse6DataStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalScheduledConsentResponse6DataStatusEnum
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
    internal static class OBWriteInternationalScheduledConsentResponse6DataStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalScheduledConsentResponse6DataStatusEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalScheduledConsentResponse6DataStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalScheduledConsentResponse6DataStatusEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalScheduledConsentResponse6DataStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteInternationalScheduledConsentResponse6DataStatusEnum.AwaitingAuthorisation:
                    return "AwaitingAuthorisation";
                case OBWriteInternationalScheduledConsentResponse6DataStatusEnum.Consumed:
                    return "Consumed";
                case OBWriteInternationalScheduledConsentResponse6DataStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteInternationalScheduledConsentResponse6DataStatusEnum? ParseOBWriteInternationalScheduledConsentResponse6DataStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteInternationalScheduledConsentResponse6DataStatusEnum.Authorised;
                case "AwaitingAuthorisation":
                    return OBWriteInternationalScheduledConsentResponse6DataStatusEnum.AwaitingAuthorisation;
                case "Consumed":
                    return OBWriteInternationalScheduledConsentResponse6DataStatusEnum.Consumed;
                case "Rejected":
                    return OBWriteInternationalScheduledConsentResponse6DataStatusEnum.Rejected;
            }
            return null;
        }
    }
}
