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
    /// OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum
    {
        [EnumMember(Value = "Authorised")]
        Authorised,
        [EnumMember(Value = "AwaitingFurtherAuthorisation")]
        AwaitingFurtherAuthorisation,
        [EnumMember(Value = "Rejected")]
        Rejected
    }
    internal static class OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum? value)
        {
            return value == null ? null : ((OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum value)
        {
            switch( value )
            {
                case OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum.Authorised:
                    return "Authorised";
                case OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum.AwaitingFurtherAuthorisation:
                    return "AwaitingFurtherAuthorisation";
                case OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum.Rejected:
                    return "Rejected";
            }
            return null;
        }

        internal static OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum? ParseOBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum(this string value)
        {
            switch( value )
            {
                case "Authorised":
                    return OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum.Authorised;
                case "AwaitingFurtherAuthorisation":
                    return OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum.AwaitingFurtherAuthorisation;
                case "Rejected":
                    return OBWriteDomesticScheduledResponse4DataMultiAuthorisationStatusEnum.Rejected;
            }
            return null;
        }
    }
}
