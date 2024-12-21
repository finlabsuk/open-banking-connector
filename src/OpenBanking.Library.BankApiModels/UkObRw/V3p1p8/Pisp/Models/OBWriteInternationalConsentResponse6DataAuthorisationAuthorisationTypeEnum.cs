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
    /// OBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnum
    {
        [EnumMember(Value = "Any")]
        Any,
        [EnumMember(Value = "Single")]
        Single
    }
    internal static class OBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnum.Any:
                    return "Any";
                case OBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnum.Single:
                    return "Single";
            }
            return null;
        }

        internal static OBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnum? ParseOBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnum(this string value)
        {
            switch( value )
            {
                case "Any":
                    return OBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnum.Any;
                case "Single":
                    return OBWriteInternationalConsentResponse6DataAuthorisationAuthorisationTypeEnum.Single;
            }
            return null;
        }
    }
}