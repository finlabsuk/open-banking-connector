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
    /// OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum
    {
        [EnumMember(Value = "Actual")]
        Actual,
        [EnumMember(Value = "Agreed")]
        Agreed,
        [EnumMember(Value = "Indicative")]
        Indicative
    }
    internal static class OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum.Actual:
                    return "Actual";
                case OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum.Agreed:
                    return "Agreed";
                case OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum.Indicative:
                    return "Indicative";
            }
            return null;
        }

        internal static OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum? ParseOBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum(this string value)
        {
            switch( value )
            {
                case "Actual":
                    return OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum.Actual;
                case "Agreed":
                    return OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum.Agreed;
                case "Indicative":
                    return OBWriteInternationalResponse5DataExchangeRateInformationRateTypeEnum.Indicative;
            }
            return null;
        }
    }
}