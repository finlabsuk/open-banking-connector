// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p3.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for OBRegistrationProperties1applicationTypeEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBRegistrationProperties1applicationTypeEnum
    {
        [EnumMember(Value = "web")]
        Web,
        [EnumMember(Value = "mobile")]
        Mobile
    }
    internal static class OBRegistrationProperties1applicationTypeEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBRegistrationProperties1applicationTypeEnum? value)
        {
            return value == null ? null : ((OBRegistrationProperties1applicationTypeEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBRegistrationProperties1applicationTypeEnum value)
        {
            switch( value )
            {
                case OBRegistrationProperties1applicationTypeEnum.Web:
                    return "web";
                case OBRegistrationProperties1applicationTypeEnum.Mobile:
                    return "mobile";
            }
            return null;
        }

        internal static OBRegistrationProperties1applicationTypeEnum? ParseOBRegistrationProperties1applicationTypeEnum(this string value)
        {
            switch( value )
            {
                case "web":
                    return OBRegistrationProperties1applicationTypeEnum.Web;
                case "mobile":
                    return OBRegistrationProperties1applicationTypeEnum.Mobile;
            }
            return null;
        }
    }
}
