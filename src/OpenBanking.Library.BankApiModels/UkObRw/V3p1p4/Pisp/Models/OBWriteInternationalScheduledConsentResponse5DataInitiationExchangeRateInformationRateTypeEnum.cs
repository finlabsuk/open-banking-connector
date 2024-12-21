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
    /// OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum
    {
        [EnumMember(Value = "Actual")]
        Actual,
        [EnumMember(Value = "Agreed")]
        Agreed,
        [EnumMember(Value = "Indicative")]
        Indicative
    }
    internal static class OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum.Actual:
                    return "Actual";
                case OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum.Agreed:
                    return "Agreed";
                case OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum.Indicative:
                    return "Indicative";
            }
            return null;
        }

        internal static OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum? ParseOBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum(this string value)
        {
            switch( value )
            {
                case "Actual":
                    return OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum.Actual;
                case "Agreed":
                    return OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum.Agreed;
                case "Indicative":
                    return OBWriteInternationalScheduledConsentResponse5DataInitiationExchangeRateInformationRateTypeEnum.Indicative;
            }
            return null;
        }
    }
}