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
    /// Defines values for
    /// OBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnum
    {
        [EnumMember(Value = "Any")]
        Any,
        [EnumMember(Value = "Single")]
        Single
    }
    internal static class OBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnum.Any:
                    return "Any";
                case OBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnum.Single:
                    return "Single";
            }
            return null;
        }

        internal static OBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnum? ParseOBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnum(this string value)
        {
            switch( value )
            {
                case "Any":
                    return OBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnum.Any;
                case "Single":
                    return OBWriteInternationalScheduledConsentResponse6DataAuthorisationAuthorisationTypeEnum.Single;
            }
            return null;
        }
    }
}
