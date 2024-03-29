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
    /// OBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnum
    {
        [EnumMember(Value = "No")]
        No,
        [EnumMember(Value = "Yes")]
        Yes
    }
    internal static class OBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnum.No:
                    return "No";
                case OBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnum.Yes:
                    return "Yes";
            }
            return null;
        }

        internal static OBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnum? ParseOBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnum(this string value)
        {
            switch( value )
            {
                case "No":
                    return OBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnum.No;
                case "Yes":
                    return OBWriteInternationalScheduledConsentResponse6DataReadRefundAccountEnum.Yes;
            }
            return null;
        }
    }
}
