// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for ResponseTypesItemsEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResponseTypesItemsEnum
    {
        [EnumMember(Value = "code")]
        Code,
        [EnumMember(Value = "code id_token")]
        CodeidToken
    }
    internal static class ResponseTypesItemsEnumEnumExtension
    {
        internal static string ToSerializedValue(this ResponseTypesItemsEnum? value)
        {
            return value == null ? null : ((ResponseTypesItemsEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this ResponseTypesItemsEnum value)
        {
            switch( value )
            {
                case ResponseTypesItemsEnum.Code:
                    return "code";
                case ResponseTypesItemsEnum.CodeidToken:
                    return "code id_token";
            }
            return null;
        }

        internal static ResponseTypesItemsEnum? ParseResponseTypesItemsEnum(this string value)
        {
            switch( value )
            {
                case "code":
                    return ResponseTypesItemsEnum.Code;
                case "code id_token":
                    return ResponseTypesItemsEnum.CodeidToken;
            }
            return null;
        }
    }
}
