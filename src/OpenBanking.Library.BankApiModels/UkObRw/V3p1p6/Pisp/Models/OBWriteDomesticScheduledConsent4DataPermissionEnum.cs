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
    /// Defines values for OBWriteDomesticScheduledConsent4DataPermissionEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticScheduledConsent4DataPermissionEnum
    {
        [EnumMember(Value = "Create")]
        Create
    }
    internal static class OBWriteDomesticScheduledConsent4DataPermissionEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticScheduledConsent4DataPermissionEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticScheduledConsent4DataPermissionEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticScheduledConsent4DataPermissionEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticScheduledConsent4DataPermissionEnum.Create:
                    return "Create";
            }
            return null;
        }

        internal static OBWriteDomesticScheduledConsent4DataPermissionEnum? ParseOBWriteDomesticScheduledConsent4DataPermissionEnum(this string value)
        {
            switch( value )
            {
                case "Create":
                    return OBWriteDomesticScheduledConsent4DataPermissionEnum.Create;
            }
            return null;
        }
    }
}
