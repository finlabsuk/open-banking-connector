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
    /// OBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnum
    {
        [EnumMember(Value = "Any")]
        Any,
        [EnumMember(Value = "Single")]
        Single
    }
    internal static class OBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnum.Any:
                    return "Any";
                case OBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnum.Single:
                    return "Single";
            }
            return null;
        }

        internal static OBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnum? ParseOBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnum(this string value)
        {
            switch( value )
            {
                case "Any":
                    return OBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnum.Any;
                case "Single":
                    return OBWriteInternationalStandingOrderConsentResponse7DataAuthorisationAuthorisationTypeEnum.Single;
            }
            return null;
        }
    }
}
