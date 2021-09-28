// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Pisp.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum
    {
        [EnumMember(Value = "Authorised")]
        Authorised,
        [EnumMember(Value = "AwaitingFurtherAuthorisation")]
        AwaitingFurtherAuthorisation,
        [EnumMember(Value = "Rejected")]
        Rejected
    }
    internal static class OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum.AwaitingFurtherAuthorisation:
                    return "AwaitingFurtherAuthorisation";
                case OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum? ParseOBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum.Authorised;
                case "AwaitingFurtherAuthorisation":
                    return OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum.AwaitingFurtherAuthorisation;
                case "Rejected":
                    return OBWriteDomesticStandingOrderResponse6DataMultiAuthorisationStatusEnum.Rejected;
            }
            return null;
        }
    }
}
