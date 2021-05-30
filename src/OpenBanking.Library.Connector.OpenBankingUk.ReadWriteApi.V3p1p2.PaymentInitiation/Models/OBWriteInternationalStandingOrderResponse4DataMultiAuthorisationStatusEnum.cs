// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p2.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum
    {
        [EnumMember(Value = "Authorised")]
        Authorised,
        [EnumMember(Value = "AwaitingFurtherAuthorisation")]
        AwaitingFurtherAuthorisation,
        [EnumMember(Value = "Rejected")]
        Rejected
    }
    internal static class OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum.AwaitingFurtherAuthorisation:
                    return "AwaitingFurtherAuthorisation";
                case OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum? ParseOBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum.Authorised;
                case "AwaitingFurtherAuthorisation":
                    return OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum.AwaitingFurtherAuthorisation;
                case "Rejected":
                    return OBWriteInternationalStandingOrderResponse4DataMultiAuthorisationStatusEnum.Rejected;
            }
            return null;
        }
    }
}