// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p4.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnum
    {
        [EnumMember(Value = "No")]
        No,
        [EnumMember(Value = "Yes")]
        Yes
    }
    internal static class OBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnum.No:
                    return "No";
                case OBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnum.Yes:
                    return "Yes";
            }
            return null;
        }

        internal static OBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnum? ParseOBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnum(this string value)
        {
            switch( value )
            {
                case "No":
                    return OBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnum.No;
                case "Yes":
                    return OBWriteDomesticStandingOrderConsentResponse5DataReadRefundAccountEnum.Yes;
            }
            return null;
        }
    }
}
