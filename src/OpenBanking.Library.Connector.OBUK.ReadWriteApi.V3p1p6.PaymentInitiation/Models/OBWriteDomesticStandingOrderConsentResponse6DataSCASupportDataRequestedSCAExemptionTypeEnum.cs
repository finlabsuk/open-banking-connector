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
    /// OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum
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
    internal static class OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.BillPayment:
                    return "BillPayment";
                case OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.ContactlessTravel:
                    return "ContactlessTravel";
                case OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.EcommerceGoods:
                    return "EcommerceGoods";
                case OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.EcommerceServices:
                    return "EcommerceServices";
                case OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.Kiosk:
                    return "Kiosk";
                case OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.Parking:
                    return "Parking";
                case OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.PartyToParty:
                    return "PartyToParty";
            }
            return null;
        }

        internal static OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum? ParseOBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum(this string value)
        {
            switch( value )
            {
                case "BillPayment":
                    return OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.BillPayment;
                case "ContactlessTravel":
                    return OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.ContactlessTravel;
                case "EcommerceGoods":
                    return OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.EcommerceGoods;
                case "EcommerceServices":
                    return OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.EcommerceServices;
                case "Kiosk":
                    return OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.Kiosk;
                case "Parking":
                    return OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.Parking;
                case "PartyToParty":
                    return OBWriteDomesticStandingOrderConsentResponse6DataSCASupportDataRequestedSCAExemptionTypeEnum.PartyToParty;
            }
            return null;
        }
    }
}
