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
    /// OBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnum
    {
        [EnumMember(Value = "Any")]
        Any,
        [EnumMember(Value = "Single")]
        Single
    }
    internal static class OBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnum.Any:
                    return "Any";
                case OBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnum.Single:
                    return "Single";
            }
            return null;
        }

        internal static OBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnum? ParseOBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnum(this string value)
        {
            switch( value )
            {
                case "Any":
                    return OBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnum.Any;
                case "Single":
                    return OBWriteDomesticScheduledConsentResponse5DataAuthorisationAuthorisationTypeEnum.Single;
            }
            return null;
        }
    }
}
