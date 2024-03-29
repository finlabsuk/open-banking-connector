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
    /// OBWriteDomesticScheduledConsentResponse4DataStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticScheduledConsentResponse4DataStatusEnum
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
    internal static class OBWriteDomesticScheduledConsentResponse4DataStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticScheduledConsentResponse4DataStatusEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticScheduledConsentResponse4DataStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticScheduledConsentResponse4DataStatusEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticScheduledConsentResponse4DataStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteDomesticScheduledConsentResponse4DataStatusEnum.AwaitingAuthorisation:
                    return "AwaitingAuthorisation";
                case OBWriteDomesticScheduledConsentResponse4DataStatusEnum.Consumed:
                    return "Consumed";
                case OBWriteDomesticScheduledConsentResponse4DataStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteDomesticScheduledConsentResponse4DataStatusEnum? ParseOBWriteDomesticScheduledConsentResponse4DataStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteDomesticScheduledConsentResponse4DataStatusEnum.Authorised;
                case "AwaitingAuthorisation":
                    return OBWriteDomesticScheduledConsentResponse4DataStatusEnum.AwaitingAuthorisation;
                case "Consumed":
                    return OBWriteDomesticScheduledConsentResponse4DataStatusEnum.Consumed;
                case "Rejected":
                    return OBWriteDomesticScheduledConsentResponse4DataStatusEnum.Rejected;
            }
            return null;
        }
    }
}
