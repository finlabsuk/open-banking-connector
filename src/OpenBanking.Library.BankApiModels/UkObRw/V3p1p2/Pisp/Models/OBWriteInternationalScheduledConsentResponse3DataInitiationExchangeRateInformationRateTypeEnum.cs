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
    /// Defines values for
    /// OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum
    {
        [EnumMember(Value = "Actual")]
        Actual,
        [EnumMember(Value = "Agreed")]
        Agreed,
        [EnumMember(Value = "Indicative")]
        Indicative
    }
    internal static class OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum.Actual:
                    return "Actual";
                case OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum.Agreed:
                    return "Agreed";
                case OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum.Indicative:
                    return "Indicative";
            }
            return null;
        }

        internal static OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum? ParseOBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum(this string value)
        {
            switch( value )
            {
                case "Actual":
                    return OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum.Actual;
                case "Agreed":
                    return OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum.Agreed;
                case "Indicative":
                    return OBWriteInternationalScheduledConsentResponse3DataInitiationExchangeRateInformationRateTypeEnum.Indicative;
            }
            return null;
        }
    }
}
