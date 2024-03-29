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
    /// OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum
    {
        [EnumMember(Value = "Actual")]
        Actual,
        [EnumMember(Value = "Agreed")]
        Agreed,
        [EnumMember(Value = "Indicative")]
        Indicative
    }
    internal static class OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum.Actual:
                    return "Actual";
                case OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum.Agreed:
                    return "Agreed";
                case OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum.Indicative:
                    return "Indicative";
            }
            return null;
        }

        internal static OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum? ParseOBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum(this string value)
        {
            switch( value )
            {
                case "Actual":
                    return OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum.Actual;
                case "Agreed":
                    return OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum.Agreed;
                case "Indicative":
                    return OBWriteInternationalScheduledResponse5DataInitiationExchangeRateInformationRateTypeEnum.Indicative;
            }
            return null;
        }
    }
}
