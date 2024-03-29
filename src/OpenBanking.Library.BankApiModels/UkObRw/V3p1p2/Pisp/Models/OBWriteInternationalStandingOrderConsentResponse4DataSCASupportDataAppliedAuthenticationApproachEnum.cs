// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p2.Pisp.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum
    {
        [EnumMember(Value = "CA")]
        CA,
        [EnumMember(Value = "SCA")]
        SCA
    }
    internal static class OBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum.CA:
                    return "CA";
                case OBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum.SCA:
                    return "SCA";
            }
            return null;
        }

        internal static OBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum? ParseOBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum(this string value)
        {
            switch( value )
            {
                case "CA":
                    return OBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum.CA;
                case "SCA":
                    return OBWriteInternationalStandingOrderConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum.SCA;
            }
            return null;
        }
    }
}
