// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteInternationalScheduled3DataInitiationInstructionPriorityEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalScheduled3DataInitiationInstructionPriorityEnum
    {
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Urgent")]
        Urgent
    }
    internal static class OBWriteInternationalScheduled3DataInitiationInstructionPriorityEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalScheduled3DataInitiationInstructionPriorityEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalScheduled3DataInitiationInstructionPriorityEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalScheduled3DataInitiationInstructionPriorityEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalScheduled3DataInitiationInstructionPriorityEnum.Normal:
                    return "Normal";
                case OBWriteInternationalScheduled3DataInitiationInstructionPriorityEnum.Urgent:
                    return "Urgent";
            }
            return null;
        }

        internal static OBWriteInternationalScheduled3DataInitiationInstructionPriorityEnum? ParseOBWriteInternationalScheduled3DataInitiationInstructionPriorityEnum(this string value)
        {
            switch( value )
            {
                case "Normal":
                    return OBWriteInternationalScheduled3DataInitiationInstructionPriorityEnum.Normal;
                case "Urgent":
                    return OBWriteInternationalScheduled3DataInitiationInstructionPriorityEnum.Urgent;
            }
            return null;
        }
    }
}
