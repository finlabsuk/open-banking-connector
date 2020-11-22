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
    /// OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum
    {
        [EnumMember(Value = "Actual")]
        Actual,
        [EnumMember(Value = "Agreed")]
        Agreed,
        [EnumMember(Value = "Indicative")]
        Indicative
    }
    internal static class OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum.Actual:
                    return "Actual";
                case OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum.Agreed:
                    return "Agreed";
                case OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum.Indicative:
                    return "Indicative";
            }
            return null;
        }

        internal static OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum? ParseOBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum(this string value)
        {
            switch( value )
            {
                case "Actual":
                    return OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum.Actual;
                case "Agreed":
                    return OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum.Agreed;
                case "Indicative":
                    return OBWriteInternationalConsent5DataInitiationExchangeRateInformationRateTypeEnum.Indicative;
            }
            return null;
        }
    }
}
