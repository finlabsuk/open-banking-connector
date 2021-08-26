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
    /// Defines values for RegistrationErrorerrorEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RegistrationErrorerrorEnum
    {
        [EnumMember(Value = "invalid_redirect_uri")]
        InvalidRedirectUri,
        [EnumMember(Value = "invalid_client_metadata")]
        InvalidClientMetadata,
        [EnumMember(Value = "invalid_software_statement")]
        InvalidSoftwareStatement,
        [EnumMember(Value = "unapproved_software_statement")]
        UnapprovedSoftwareStatement
    }
    internal static class RegistrationErrorerrorEnumEnumExtension
    {
        internal static string ToSerializedValue(this RegistrationErrorerrorEnum? value)
        {
            return value == null ? null : ((RegistrationErrorerrorEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this RegistrationErrorerrorEnum value)
        {
            switch( value )
            {
                case RegistrationErrorerrorEnum.InvalidRedirectUri:
                    return "invalid_redirect_uri";
                case RegistrationErrorerrorEnum.InvalidClientMetadata:
                    return "invalid_client_metadata";
                case RegistrationErrorerrorEnum.InvalidSoftwareStatement:
                    return "invalid_software_statement";
                case RegistrationErrorerrorEnum.UnapprovedSoftwareStatement:
                    return "unapproved_software_statement";
            }
            return null;
        }

        internal static RegistrationErrorerrorEnum? ParseRegistrationErrorerrorEnum(this string value)
        {
            switch( value )
            {
                case "invalid_redirect_uri":
                    return RegistrationErrorerrorEnum.InvalidRedirectUri;
                case "invalid_client_metadata":
                    return RegistrationErrorerrorEnum.InvalidClientMetadata;
                case "invalid_software_statement":
                    return RegistrationErrorerrorEnum.InvalidSoftwareStatement;
                case "unapproved_software_statement":
                    return RegistrationErrorerrorEnum.UnapprovedSoftwareStatement;
            }
            return null;
        }
    }
}
