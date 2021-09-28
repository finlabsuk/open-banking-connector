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
    /// Defines values for OBRisk1PaymentContextCodeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBRisk1PaymentContextCodeEnum
    {
        [EnumMember(Value = "BillPayment")]
        BillPayment,
        [EnumMember(Value = "EcommerceGoods")]
        EcommerceGoods,
        [EnumMember(Value = "EcommerceServices")]
        EcommerceServices,
        [EnumMember(Value = "Other")]
        Other,
        [EnumMember(Value = "PartyToParty")]
        PartyToParty
    }
    internal static class OBRisk1PaymentContextCodeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBRisk1PaymentContextCodeEnum? value)
        {
            return value == null ? null : ((OBRisk1PaymentContextCodeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBRisk1PaymentContextCodeEnum value)
        {
            switch( value )
            {
                case OBRisk1PaymentContextCodeEnum.BillPayment:
                    return "BillPayment";
                case OBRisk1PaymentContextCodeEnum.EcommerceGoods:
                    return "EcommerceGoods";
                case OBRisk1PaymentContextCodeEnum.EcommerceServices:
                    return "EcommerceServices";
                case OBRisk1PaymentContextCodeEnum.Other:
                    return "Other";
                case OBRisk1PaymentContextCodeEnum.PartyToParty:
                    return "PartyToParty";
            }
            return null;
        }

        internal static OBRisk1PaymentContextCodeEnum? ParseOBRisk1PaymentContextCodeEnum(this string value)
        {
            switch( value )
            {
                case "BillPayment":
                    return OBRisk1PaymentContextCodeEnum.BillPayment;
                case "EcommerceGoods":
                    return OBRisk1PaymentContextCodeEnum.EcommerceGoods;
                case "EcommerceServices":
                    return OBRisk1PaymentContextCodeEnum.EcommerceServices;
                case "Other":
                    return OBRisk1PaymentContextCodeEnum.Other;
                case "PartyToParty":
                    return OBRisk1PaymentContextCodeEnum.PartyToParty;
            }
            return null;
        }
    }
}