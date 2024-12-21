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
    /// OBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnum
    {
        [EnumMember(Value = "CA")]
        CA,
        [EnumMember(Value = "SCA")]
        SCA
    }
    internal static class OBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnum.CA:
                    return "CA";
                case OBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnum.SCA:
                    return "SCA";
            }
            return null;
        }

        internal static OBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnum? ParseOBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnum(this string value)
        {
            switch( value )
            {
                case "CA":
                    return OBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnum.CA;
                case "SCA":
                    return OBWriteDomesticConsentResponse3DataSCASupportDataAppliedAuthenticationApproachEnum.SCA;
            }
            return null;
        }
    }
}