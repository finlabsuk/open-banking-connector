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
    /// OBWriteInternationalStandingOrderConsentResponse6DataPermissionEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalStandingOrderConsentResponse6DataPermissionEnum
    {
        [EnumMember(Value = "Create")]
        Create
    }
    internal static class OBWriteInternationalStandingOrderConsentResponse6DataPermissionEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsentResponse6DataPermissionEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalStandingOrderConsentResponse6DataPermissionEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderConsentResponse6DataPermissionEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalStandingOrderConsentResponse6DataPermissionEnum.Create:
                    return "Create";
            }
            return null;
        }

        internal static OBWriteInternationalStandingOrderConsentResponse6DataPermissionEnum? ParseOBWriteInternationalStandingOrderConsentResponse6DataPermissionEnum(this string value)
        {
            switch( value )
            {
                case "Create":
                    return OBWriteInternationalStandingOrderConsentResponse6DataPermissionEnum.Create;
            }
            return null;
        }
    }
}
