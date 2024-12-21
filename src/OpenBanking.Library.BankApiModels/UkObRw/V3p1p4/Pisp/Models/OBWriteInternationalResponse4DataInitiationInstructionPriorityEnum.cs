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
    /// OBWriteInternationalResponse4DataInitiationInstructionPriorityEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalResponse4DataInitiationInstructionPriorityEnum
    {
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Urgent")]
        Urgent
    }
    internal static class OBWriteInternationalResponse4DataInitiationInstructionPriorityEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalResponse4DataInitiationInstructionPriorityEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalResponse4DataInitiationInstructionPriorityEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalResponse4DataInitiationInstructionPriorityEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalResponse4DataInitiationInstructionPriorityEnum.Normal:
                    return "Normal";
                case OBWriteInternationalResponse4DataInitiationInstructionPriorityEnum.Urgent:
                    return "Urgent";
            }
            return null;
        }

        internal static OBWriteInternationalResponse4DataInitiationInstructionPriorityEnum? ParseOBWriteInternationalResponse4DataInitiationInstructionPriorityEnum(this string value)
        {
            switch( value )
            {
                case "Normal":
                    return OBWriteInternationalResponse4DataInitiationInstructionPriorityEnum.Normal;
                case "Urgent":
                    return OBWriteInternationalResponse4DataInitiationInstructionPriorityEnum.Urgent;
            }
            return null;
        }
    }
}