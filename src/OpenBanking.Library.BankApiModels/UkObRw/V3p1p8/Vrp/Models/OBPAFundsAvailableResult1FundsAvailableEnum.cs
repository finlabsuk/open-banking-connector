// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for OBPAFundsAvailableResult1FundsAvailableEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBPAFundsAvailableResult1FundsAvailableEnum
    {
        [EnumMember(Value = "Available")]
        Available,
        [EnumMember(Value = "NotAvailable")]
        NotAvailable
    }
    internal static class OBPAFundsAvailableResult1FundsAvailableEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBPAFundsAvailableResult1FundsAvailableEnum? value)
        {
            return value == null ? null : ((OBPAFundsAvailableResult1FundsAvailableEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBPAFundsAvailableResult1FundsAvailableEnum value)
        {
            switch( value )
            {
                case OBPAFundsAvailableResult1FundsAvailableEnum.Available:
                    return "Available";
                case OBPAFundsAvailableResult1FundsAvailableEnum.NotAvailable:
                    return "NotAvailable";
            }
            return null;
        }

        internal static OBPAFundsAvailableResult1FundsAvailableEnum? ParseOBPAFundsAvailableResult1FundsAvailableEnum(this string value)
        {
            switch( value )
            {
                case "Available":
                    return OBPAFundsAvailableResult1FundsAvailableEnum.Available;
                case "NotAvailable":
                    return OBPAFundsAvailableResult1FundsAvailableEnum.NotAvailable;
            }
            return null;
        }
    }
}
