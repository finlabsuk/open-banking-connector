// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum
    {
        [EnumMember(Value = "CA")]
        CA,
        [EnumMember(Value = "SCA")]
        SCA
    }
    internal static class OBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum.CA:
                    return "CA";
                case OBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum.SCA:
                    return "SCA";
            }
            return null;
        }

        internal static OBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum? ParseOBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum(this string value)
        {
            switch( value )
            {
                case "CA":
                    return OBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum.CA;
                case "SCA":
                    return OBWriteDomesticConsentResponse4DataSCASupportDataAppliedAuthenticationApproachEnum.SCA;
            }
            return null;
        }
    }
}
