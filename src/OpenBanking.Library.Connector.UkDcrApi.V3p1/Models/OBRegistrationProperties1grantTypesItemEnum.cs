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
    /// Defines values for OBRegistrationProperties1grantTypesItemEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBRegistrationProperties1grantTypesItemEnum
    {
        [EnumMember(Value = "client_credentials")]
        ClientCredentials,
        [EnumMember(Value = "authorization_code")]
        AuthorizationCode,
        [EnumMember(Value = "refresh_token")]
        RefreshToken
    }
    internal static class OBRegistrationProperties1grantTypesItemEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBRegistrationProperties1grantTypesItemEnum? value)
        {
            return value == null ? null : ((OBRegistrationProperties1grantTypesItemEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBRegistrationProperties1grantTypesItemEnum value)
        {
            switch( value )
            {
                case OBRegistrationProperties1grantTypesItemEnum.ClientCredentials:
                    return "client_credentials";
                case OBRegistrationProperties1grantTypesItemEnum.AuthorizationCode:
                    return "authorization_code";
                case OBRegistrationProperties1grantTypesItemEnum.RefreshToken:
                    return "refresh_token";
            }
            return null;
        }

        internal static OBRegistrationProperties1grantTypesItemEnum? ParseOBRegistrationProperties1grantTypesItemEnum(this string value)
        {
            switch( value )
            {
                case "client_credentials":
                    return OBRegistrationProperties1grantTypesItemEnum.ClientCredentials;
                case "authorization_code":
                    return OBRegistrationProperties1grantTypesItemEnum.AuthorizationCode;
                case "refresh_token":
                    return OBRegistrationProperties1grantTypesItemEnum.RefreshToken;
            }
            return null;
        }
    }
}
