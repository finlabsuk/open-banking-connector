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
    /// OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum
    {
        [EnumMember(Value = "Authorised")]
        Authorised,
        [EnumMember(Value = "AwaitingFurtherAuthorisation")]
        AwaitingFurtherAuthorisation,
        [EnumMember(Value = "Rejected")]
        Rejected
    }
    internal static class OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum.AwaitingFurtherAuthorisation:
                    return "AwaitingFurtherAuthorisation";
                case OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum? ParseOBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum.Authorised;
                case "AwaitingFurtherAuthorisation":
                    return OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum.AwaitingFurtherAuthorisation;
                case "Rejected":
                    return OBWriteInternationalStandingOrderResponse6DataMultiAuthorisationStatusEnum.Rejected;
            }
            return null;
        }
    }
}
