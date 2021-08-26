// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.UkDcrApi.V3p1.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for OBRegistrationProperties1responseTypesItemEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBRegistrationProperties1responseTypesItemEnum
    {
        [EnumMember(Value = "code")]
        Code,
        [EnumMember(Value = "code id_token")]
        CodeidToken
    }
    internal static class OBRegistrationProperties1responseTypesItemEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBRegistrationProperties1responseTypesItemEnum? value)
        {
            return value == null ? null : ((OBRegistrationProperties1responseTypesItemEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBRegistrationProperties1responseTypesItemEnum value)
        {
            switch( value )
            {
                case OBRegistrationProperties1responseTypesItemEnum.Code:
                    return "code";
                case OBRegistrationProperties1responseTypesItemEnum.CodeidToken:
                    return "code id_token";
            }
            return null;
        }

        internal static OBRegistrationProperties1responseTypesItemEnum? ParseOBRegistrationProperties1responseTypesItemEnum(this string value)
        {
            switch( value )
            {
                case "code":
                    return OBRegistrationProperties1responseTypesItemEnum.Code;
                case "code id_token":
                    return OBRegistrationProperties1responseTypesItemEnum.CodeidToken;
            }
            return null;
        }
    }
}
