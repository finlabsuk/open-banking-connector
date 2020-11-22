// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p6.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnum
    {
        [EnumMember(Value = "Any")]
        Any,
        [EnumMember(Value = "Single")]
        Single
    }
    internal static class OBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnum.Any:
                    return "Any";
                case OBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnum.Single:
                    return "Single";
            }
            return null;
        }

        internal static OBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnum? ParseOBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnum(this string value)
        {
            switch( value )
            {
                case "Any":
                    return OBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnum.Any;
                case "Single":
                    return OBWriteDomesticStandingOrderConsent5DataAuthorisationAuthorisationTypeEnum.Single;
            }
            return null;
        }
    }
}
