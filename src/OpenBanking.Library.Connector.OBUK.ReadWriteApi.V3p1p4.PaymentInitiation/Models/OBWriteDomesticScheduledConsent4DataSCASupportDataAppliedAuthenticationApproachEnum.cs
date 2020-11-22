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
    /// OBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnum
    {
        [EnumMember(Value = "CA")]
        CA,
        [EnumMember(Value = "SCA")]
        SCA
    }
    internal static class OBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.CA:
                    return "CA";
                case OBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.SCA:
                    return "SCA";
            }
            return null;
        }

        internal static OBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnum? ParseOBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnum(this string value)
        {
            switch( value )
            {
                case "CA":
                    return OBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.CA;
                case "SCA":
                    return OBWriteDomesticScheduledConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.SCA;
            }
            return null;
        }
    }
}
