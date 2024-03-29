// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p2.Pisp.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for OBWriteFileConsentResponse3DataStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteFileConsentResponse3DataStatusEnum
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
    internal static class OBWriteFileConsentResponse3DataStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteFileConsentResponse3DataStatusEnum? value)
        {
            return value == null ? null : ((OBWriteFileConsentResponse3DataStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteFileConsentResponse3DataStatusEnum value)
        {
            switch( value )
            {
                case OBWriteFileConsentResponse3DataStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteFileConsentResponse3DataStatusEnum.AwaitingAuthorisation:
                    return "AwaitingAuthorisation";
                case OBWriteFileConsentResponse3DataStatusEnum.AwaitingUpload:
                    return "AwaitingUpload";
                case OBWriteFileConsentResponse3DataStatusEnum.Consumed:
                    return "Consumed";
                case OBWriteFileConsentResponse3DataStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteFileConsentResponse3DataStatusEnum? ParseOBWriteFileConsentResponse3DataStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteFileConsentResponse3DataStatusEnum.Authorised;
                case "AwaitingAuthorisation":
                    return OBWriteFileConsentResponse3DataStatusEnum.AwaitingAuthorisation;
                case "AwaitingUpload":
                    return OBWriteFileConsentResponse3DataStatusEnum.AwaitingUpload;
                case "Consumed":
                    return OBWriteFileConsentResponse3DataStatusEnum.Consumed;
                case "Rejected":
                    return OBWriteFileConsentResponse3DataStatusEnum.Rejected;
            }
            return null;
        }
    }
}
