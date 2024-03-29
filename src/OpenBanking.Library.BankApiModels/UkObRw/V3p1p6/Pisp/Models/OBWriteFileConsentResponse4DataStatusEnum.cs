// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for OBWriteFileConsentResponse4DataStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteFileConsentResponse4DataStatusEnum
    {
        [EnumMember(Value = "Authorised")]
        Authorised,
        [EnumMember(Value = "AwaitingAuthorisation")]
        AwaitingAuthorisation,
        [EnumMember(Value = "AwaitingUpload")]
        AwaitingUpload,
        [EnumMember(Value = "Consumed")]
        Consumed,
        [EnumMember(Value = "Rejected")]
        Rejected
    }
    internal static class OBWriteFileConsentResponse4DataStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteFileConsentResponse4DataStatusEnum? value)
        {
            return value == null ? null : ((OBWriteFileConsentResponse4DataStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteFileConsentResponse4DataStatusEnum value)
        {
            switch( value )
            {
                case OBWriteFileConsentResponse4DataStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteFileConsentResponse4DataStatusEnum.AwaitingAuthorisation:
                    return "AwaitingAuthorisation";
                case OBWriteFileConsentResponse4DataStatusEnum.AwaitingUpload:
                    return "AwaitingUpload";
                case OBWriteFileConsentResponse4DataStatusEnum.Consumed:
                    return "Consumed";
                case OBWriteFileConsentResponse4DataStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteFileConsentResponse4DataStatusEnum? ParseOBWriteFileConsentResponse4DataStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteFileConsentResponse4DataStatusEnum.Authorised;
                case "AwaitingAuthorisation":
                    return OBWriteFileConsentResponse4DataStatusEnum.AwaitingAuthorisation;
                case "AwaitingUpload":
                    return OBWriteFileConsentResponse4DataStatusEnum.AwaitingUpload;
                case "Consumed":
                    return OBWriteFileConsentResponse4DataStatusEnum.Consumed;
                case "Rejected":
                    return OBWriteFileConsentResponse4DataStatusEnum.Rejected;
            }
            return null;
        }
    }
}
