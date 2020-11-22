// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p6.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteFileResponse3DataMultiAuthorisationStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteFileResponse3DataMultiAuthorisationStatusEnum
    {
        [EnumMember(Value = "Authorised")]
        Authorised,
        [EnumMember(Value = "AwaitingFurtherAuthorisation")]
        AwaitingFurtherAuthorisation,
        [EnumMember(Value = "Rejected")]
        Rejected
    }
    internal static class OBWriteFileResponse3DataMultiAuthorisationStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteFileResponse3DataMultiAuthorisationStatusEnum? value)
        {
            return value == null ? null : ((OBWriteFileResponse3DataMultiAuthorisationStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteFileResponse3DataMultiAuthorisationStatusEnum value)
        {
            switch( value )
            {
                case OBWriteFileResponse3DataMultiAuthorisationStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteFileResponse3DataMultiAuthorisationStatusEnum.AwaitingFurtherAuthorisation:
                    return "AwaitingFurtherAuthorisation";
                case OBWriteFileResponse3DataMultiAuthorisationStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteFileResponse3DataMultiAuthorisationStatusEnum? ParseOBWriteFileResponse3DataMultiAuthorisationStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteFileResponse3DataMultiAuthorisationStatusEnum.Authorised;
                case "AwaitingFurtherAuthorisation":
                    return OBWriteFileResponse3DataMultiAuthorisationStatusEnum.AwaitingFurtherAuthorisation;
                case "Rejected":
                    return OBWriteFileResponse3DataMultiAuthorisationStatusEnum.Rejected;
            }
            return null;
        }
    }
}
