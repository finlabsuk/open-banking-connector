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
    /// OBWriteInternational3DataInitiationInstructionPriorityEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternational3DataInitiationInstructionPriorityEnum
    {
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Urgent")]
        Urgent
    }
    internal static class OBWriteInternational3DataInitiationInstructionPriorityEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternational3DataInitiationInstructionPriorityEnum? value)
        {
            return value == null ? null : ((OBWriteInternational3DataInitiationInstructionPriorityEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternational3DataInitiationInstructionPriorityEnum value)
        {
            switch( value )
            {
                case OBWriteInternational3DataInitiationInstructionPriorityEnum.Normal:
                    return "Normal";
                case OBWriteInternational3DataInitiationInstructionPriorityEnum.Urgent:
                    return "Urgent";
            }
            return null;
        }

        internal static OBWriteInternational3DataInitiationInstructionPriorityEnum? ParseOBWriteInternational3DataInitiationInstructionPriorityEnum(this string value)
        {
            switch( value )
            {
                case "Normal":
                    return OBWriteInternational3DataInitiationInstructionPriorityEnum.Normal;
                case "Urgent":
                    return OBWriteInternational3DataInitiationInstructionPriorityEnum.Urgent;
            }
            return null;
        }
    }
}