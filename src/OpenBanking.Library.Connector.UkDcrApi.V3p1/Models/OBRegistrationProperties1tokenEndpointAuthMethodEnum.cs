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
    /// Defines values for
    /// OBRegistrationProperties1tokenEndpointAuthMethodEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBRegistrationProperties1tokenEndpointAuthMethodEnum
    {
        [EnumMember(Value = "private_key_jwt")]
        PrivateKeyJwt,
        [EnumMember(Value = "tls_client_auth")]
        TlsClientAuth
    }
    internal static class OBRegistrationProperties1tokenEndpointAuthMethodEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBRegistrationProperties1tokenEndpointAuthMethodEnum? value)
        {
            return value == null ? null : ((OBRegistrationProperties1tokenEndpointAuthMethodEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBRegistrationProperties1tokenEndpointAuthMethodEnum value)
        {
            switch( value )
            {
                case OBRegistrationProperties1tokenEndpointAuthMethodEnum.PrivateKeyJwt:
                    return "private_key_jwt";
                case OBRegistrationProperties1tokenEndpointAuthMethodEnum.TlsClientAuth:
                    return "tls_client_auth";
            }
            return null;
        }

        internal static OBRegistrationProperties1tokenEndpointAuthMethodEnum? ParseOBRegistrationProperties1tokenEndpointAuthMethodEnum(this string value)
        {
            switch( value )
            {
                case "private_key_jwt":
                    return OBRegistrationProperties1tokenEndpointAuthMethodEnum.PrivateKeyJwt;
                case "tls_client_auth":
                    return OBRegistrationProperties1tokenEndpointAuthMethodEnum.TlsClientAuth;
            }
            return null;
        }
    }
}
