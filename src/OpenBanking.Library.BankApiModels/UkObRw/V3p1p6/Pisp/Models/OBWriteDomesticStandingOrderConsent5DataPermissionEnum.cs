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
    /// OBWriteDomesticStandingOrderConsent5DataPermissionEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticStandingOrderConsent5DataPermissionEnum
    {
        [EnumMember(Value = "Create")]
        Create
    }
    internal static class OBWriteDomesticStandingOrderConsent5DataPermissionEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticStandingOrderConsent5DataPermissionEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticStandingOrderConsent5DataPermissionEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticStandingOrderConsent5DataPermissionEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticStandingOrderConsent5DataPermissionEnum.Create:
                    return "Create";
            }
            return null;
        }

        internal static OBWriteDomesticStandingOrderConsent5DataPermissionEnum? ParseOBWriteDomesticStandingOrderConsent5DataPermissionEnum(this string value)
        {
            switch( value )
            {
                case "Create":
                    return OBWriteDomesticStandingOrderConsent5DataPermissionEnum.Create;
            }
            return null;
        }
    }
}
