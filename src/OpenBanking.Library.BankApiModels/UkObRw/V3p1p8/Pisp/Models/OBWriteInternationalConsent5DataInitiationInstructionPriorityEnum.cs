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
    /// OBWriteInternationalConsent5DataInitiationInstructionPriorityEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalConsent5DataInitiationInstructionPriorityEnum
    {
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Urgent")]
        Urgent
    }
    internal static class OBWriteInternationalConsent5DataInitiationInstructionPriorityEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalConsent5DataInitiationInstructionPriorityEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalConsent5DataInitiationInstructionPriorityEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalConsent5DataInitiationInstructionPriorityEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalConsent5DataInitiationInstructionPriorityEnum.Normal:
                    return "Normal";
                case OBWriteInternationalConsent5DataInitiationInstructionPriorityEnum.Urgent:
                    return "Urgent";
            }
            return null;
        }

        internal static OBWriteInternationalConsent5DataInitiationInstructionPriorityEnum? ParseOBWriteInternationalConsent5DataInitiationInstructionPriorityEnum(this string value)
        {
            switch( value )
            {
                case "Normal":
                    return OBWriteInternationalConsent5DataInitiationInstructionPriorityEnum.Normal;
                case "Urgent":
                    return OBWriteInternationalConsent5DataInitiationInstructionPriorityEnum.Urgent;
            }
            return null;
        }
    }
}
