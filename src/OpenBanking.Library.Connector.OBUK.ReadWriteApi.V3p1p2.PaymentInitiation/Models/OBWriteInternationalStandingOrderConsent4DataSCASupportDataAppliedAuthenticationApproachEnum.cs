// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p2.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum
    {
        [EnumMember(Value = "CA")]
        CA,
        [EnumMember(Value = "SCA")]
        SCA
    }
    internal static class OBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.CA:
                    return "CA";
                case OBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.SCA:
                    return "SCA";
            }
            return null;
        }

        internal static OBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum? ParseOBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum(this string value)
        {
            switch( value )
            {
                case "CA":
                    return OBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.CA;
                case "SCA":
                    return OBWriteInternationalStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.SCA;
            }
            return null;
        }
    }
}
