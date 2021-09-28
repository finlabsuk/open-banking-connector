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
    /// OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum
    {
        [EnumMember(Value = "Actual")]
        Actual,
        [EnumMember(Value = "Agreed")]
        Agreed,
        [EnumMember(Value = "Indicative")]
        Indicative
    }
    internal static class OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum.Actual:
                    return "Actual";
                case OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum.Agreed:
                    return "Agreed";
                case OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum.Indicative:
                    return "Indicative";
            }
            return null;
        }

        internal static OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum? ParseOBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum(this string value)
        {
            switch( value )
            {
                case "Actual":
                    return OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum.Actual;
                case "Agreed":
                    return OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum.Agreed;
                case "Indicative":
                    return OBWriteInternationalScheduledResponse6DataExchangeRateInformationRateTypeEnum.Indicative;
            }
            return null;
        }
    }
}
