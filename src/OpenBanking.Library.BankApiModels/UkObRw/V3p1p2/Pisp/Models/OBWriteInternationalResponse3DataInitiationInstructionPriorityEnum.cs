// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p2.Pisp.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteInternationalResponse3DataInitiationInstructionPriorityEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalResponse3DataInitiationInstructionPriorityEnum
    {
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Urgent")]
        Urgent
    }
    internal static class OBWriteInternationalResponse3DataInitiationInstructionPriorityEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalResponse3DataInitiationInstructionPriorityEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalResponse3DataInitiationInstructionPriorityEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalResponse3DataInitiationInstructionPriorityEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalResponse3DataInitiationInstructionPriorityEnum.Normal:
                    return "Normal";
                case OBWriteInternationalResponse3DataInitiationInstructionPriorityEnum.Urgent:
                    return "Urgent";
            }
            return null;
        }

        internal static OBWriteInternationalResponse3DataInitiationInstructionPriorityEnum? ParseOBWriteInternationalResponse3DataInitiationInstructionPriorityEnum(this string value)
        {
            switch( value )
            {
                case "Normal":
                    return OBWriteInternationalResponse3DataInitiationInstructionPriorityEnum.Normal;
                case "Urgent":
                    return OBWriteInternationalResponse3DataInitiationInstructionPriorityEnum.Urgent;
            }
            return null;
        }
    }
}