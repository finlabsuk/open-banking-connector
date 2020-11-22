// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p6.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for
    /// OBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnum
    {
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Urgent")]
        Urgent
    }
    internal static class OBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnumEnumExtension
    {
        internal static string ToSerializedValue(this OBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnum? value)
        {
            return value == null ? null : ((OBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnum)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnum value)
        {
            switch( value )
            {
                case OBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnum.Normal:
                    return "Normal";
                case OBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnum.Urgent:
                    return "Urgent";
            }
            return null;
        }

        internal static OBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnum? ParseOBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnum(this string value)
        {
            switch( value )
            {
                case "Normal":
                    return OBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnum.Normal;
                case "Urgent":
                    return OBWriteInternationalScheduledResponse6DataInitiationInstructionPriorityEnum.Urgent;
            }
            return null;
        }
    }
}
