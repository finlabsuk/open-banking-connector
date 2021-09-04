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
    /// OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum
    {
        [EnumMember(Value = "BillPayment")]
        BillPayment,
        [EnumMember(Value = "ContactlessTravel")]
        ContactlessTravel,
        [EnumMember(Value = "EcommerceGoods")]
        EcommerceGoods,
        [EnumMember(Value = "EcommerceServices")]
        EcommerceServices,
        [EnumMember(Value = "Kiosk")]
        Kiosk,
        [EnumMember(Value = "Parking")]
        Parking,
        [EnumMember(Value = "PartyToParty")]
        PartyToParty
    }
    internal static class OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.BillPayment:
                    return "BillPayment";
                case OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.ContactlessTravel:
                    return "ContactlessTravel";
                case OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.EcommerceGoods:
                    return "EcommerceGoods";
                case OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.EcommerceServices:
                    return "EcommerceServices";
                case OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.Kiosk:
                    return "Kiosk";
                case OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.Parking:
                    return "Parking";
                case OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.PartyToParty:
                    return "PartyToParty";
            }
            return null;
        }

        internal static OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum? ParseOBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum(this string value)
        {
            switch( value )
            {
                case "BillPayment":
                    return OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.BillPayment;
                case "ContactlessTravel":
                    return OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.ContactlessTravel;
                case "EcommerceGoods":
                    return OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.EcommerceGoods;
                case "EcommerceServices":
                    return OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.EcommerceServices;
                case "Kiosk":
                    return OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.Kiosk;
                case "Parking":
                    return OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.Parking;
                case "PartyToParty":
                    return OBWriteInternationalStandingOrderConsent6DataSCASupportDataRequestedSCAExemptionTypeEnum.PartyToParty;
            }
            return null;
        }
    }
}