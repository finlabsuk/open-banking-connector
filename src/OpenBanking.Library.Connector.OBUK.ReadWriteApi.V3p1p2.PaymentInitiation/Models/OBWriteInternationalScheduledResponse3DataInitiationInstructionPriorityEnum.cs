// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p2.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnum
    {
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Urgent")]
        Urgent
    }
    internal static class OBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnum.Normal:
                    return "Normal";
                case OBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnum.Urgent:
                    return "Urgent";
            }
            return null;
        }

        internal static OBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnum? ParseOBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnum(this string value)
        {
            switch( value )
            {
                case "Normal":
                    return OBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnum.Normal;
                case "Urgent":
                    return OBWriteInternationalScheduledResponse3DataInitiationInstructionPriorityEnum.Urgent;
            }
            return null;
        }
    }
}
