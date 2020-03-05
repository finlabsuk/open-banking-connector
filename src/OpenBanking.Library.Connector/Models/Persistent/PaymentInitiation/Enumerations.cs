using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiVersion
    {
        [EnumMember(Value = "v3p1p1")] 
        V3P1P1,
        [EnumMember(Value = "v3p1p2")] 
        V3P1P2
    }

    [JsonConverter(typeof(StringEnumConverter))]
    
    public enum AddressTypeCode
    {
        /// <summary>
        /// Enum Business for value: Business
        /// </summary>
        [EnumMember(Value = "business")]
        Business = 1,

        /// <summary>
        /// Enum Correspondence for value: Correspondence
        /// </summary>
        [EnumMember(Value = "correspondence")]
        Correspondence = 2,

        /// <summary>
        /// Enum DeliveryTo for value: DeliveryTo
        /// </summary>
        [EnumMember(Value = "deliveryTo")]
        DeliveryTo = 3,

        /// <summary>
        /// Enum MailTo for value: MailTo
        /// </summary>
        [EnumMember(Value = "mailTo")]
        MailTo = 4,

        /// <summary>
        /// Enum POBox for value: POBox
        /// </summary>
        [EnumMember(Value = "pobox")]
        POBox = 5,

        /// <summary>
        /// Enum Postal for value: Postal
        /// </summary>
        [EnumMember(Value = "postal")]
        Postal = 6,

        /// <summary>
        /// Enum Residential for value: Residential
        /// </summary>
        [EnumMember(Value = "residential")]
        Residential = 7,

        /// <summary>
        /// Enum Statement for value: Statement
        /// </summary>
        [EnumMember(Value = "statement")]
        Statement = 8

    }

    /// <summary>
    ///     Specifies the payment context
    /// </summary>
    /// <value>Specifies the payment context</value>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentContextCode
    {
        /// <summary>
        ///     Enum BillPayment for value: BillPayment
        /// </summary>
        [EnumMember(Value = "BillPayment")] BillPayment = 1,

        /// <summary>
        ///     Enum EcommerceGoods for value: EcommerceGoods
        /// </summary>
        [EnumMember(Value = "EcommerceGoods")] EcommerceGoods = 2,

        /// <summary>
        ///     Enum EcommerceServices for value: EcommerceServices
        /// </summary>
        [EnumMember(Value = "EcommerceServices")]
        EcommerceServices = 3,

        /// <summary>
        ///     Enum Other for value: Other
        /// </summary>
        [EnumMember(Value = "Other")] Other = 4,

        /// <summary>
        ///     Enum PartyToParty for value: PartyToParty
        /// </summary>
        [EnumMember(Value = "PartyToParty")] PartyToParty = 5
    }

    /// <summary>
    ///     Identifies the nature of the postal address.
    /// </summary>
    /// <value>Identifies the nature of the postal address.</value>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBAddressTypeCode
    {
        /// <summary>
        ///     Enum Business for value: Business
        /// </summary>
        [EnumMember(Value = "Business")] Business = 1,

        /// <summary>
        ///     Enum Correspondence for value: Correspondence
        /// </summary>
        [EnumMember(Value = "Correspondence")] Correspondence = 2,

        /// <summary>
        ///     Enum DeliveryTo for value: DeliveryTo
        /// </summary>
        [EnumMember(Value = "DeliveryTo")] DeliveryTo = 3,

        /// <summary>
        ///     Enum MailTo for value: MailTo
        /// </summary>
        [EnumMember(Value = "MailTo")] MailTo = 4,

        /// <summary>
        ///     Enum POBox for value: POBox
        /// </summary>
        [EnumMember(Value = "POBox")] POBox = 5,

        /// <summary>
        ///     Enum Postal for value: Postal
        /// </summary>
        [EnumMember(Value = "Postal")] Postal = 6,

        /// <summary>
        ///     Enum Residential for value: Residential
        /// </summary>
        [EnumMember(Value = "Residential")] Residential = 7,

        /// <summary>
        ///     Enum Statement for value: Statement
        /// </summary>
        [EnumMember(Value = "Statement")] Statement = 8
    }

    /// <summary>
    ///     Type of authorisation flow requested.
    /// </summary>
    /// <value>Type of authorisation flow requested.</value>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthorisationType
    {
        /// <summary>
        ///     Enum Any for value: Any
        /// </summary>
        [EnumMember(Value = "Any")] Any = 1,

        /// <summary>
        ///     Enum Single for value: Single
        /// </summary>
        [EnumMember(Value = "Single")] Single = 2
    }
 
    /// <summary>
    ///     This field allows a PISP to request specific SCA Exemption for a Payment Initiation
    /// </summary>
    /// <value>This field allows a PISP to request specific SCA Exemption for a Payment Initiation</value>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RequestedSCAExemptionType
    {
        /// <summary>
        ///     Enum BillPayment for value: BillPayment
        /// </summary>
        [EnumMember(Value = "BillPayment")] BillPayment = 1,

        /// <summary>
        ///     Enum ContactlessTravel for value: ContactlessTravel
        /// </summary>
        [EnumMember(Value = "ContactlessTravel")]
        ContactlessTravel = 2,

        /// <summary>
        ///     Enum EcommerceGoods for value: EcommerceGoods
        /// </summary>
        [EnumMember(Value = "EcommerceGoods")] EcommerceGoods = 3,

        /// <summary>
        ///     Enum EcommerceServices for value: EcommerceServices
        /// </summary>
        [EnumMember(Value = "EcommerceServices")]
        EcommerceServices = 4,

        /// <summary>
        ///     Enum Kiosk for value: Kiosk
        /// </summary>
        [EnumMember(Value = "Kiosk")] Kiosk = 5,

        /// <summary>
        ///     Enum Parking for value: Parking
        /// </summary>
        [EnumMember(Value = "Parking")] Parking = 6,

        /// <summary>
        ///     Enum PartyToParty for value: PartyToParty
        /// </summary>
        [EnumMember(Value = "PartyToParty")] PartyToParty = 7
    }

    /// <summary>
    ///     Specifies a character string with a maximum length of 40 characters. Usage: This field indicates whether the PSU
    ///     was subject to SCA performed by the TPP
    /// </summary>
    /// <value>
    ///     Specifies a character string with a maximum length of 40 characters. Usage: This field indicates whether the PSU
    ///     was subject to SCA performed by the TPP
    /// </value>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AppliedAuthenticationApproach
    {
        /// <summary>
        ///     Enum CA for value: CA
        /// </summary>
        [EnumMember(Value = "CA")] CA = 1,

        /// <summary>
        ///     Enum SCA for value: SCA
        /// </summary>
        [EnumMember(Value = "SCA")] SCA = 2
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWritePaymentResponseDataApiStatus
    {
        [EnumMember(Value = "AcceptedCreditSettlementCompleted")]
        AcceptedCreditSettlementCompleted,
        [EnumMember(Value = "AcceptedSettlementCompleted")]
        AcceptedSettlementCompleted,
        [EnumMember(Value = "AcceptedSettlementInProcess")]
        AcceptedSettlementInProcess,
        [EnumMember(Value = "AcceptedWithoutPosting")]
        AcceptedWithoutPosting,
        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "Rejected")]
        Rejected
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWritePaymentConsentResponseDataApiStatus
    {
        [EnumMember(Value = "Authorised")]
        Authorised,
        [EnumMember(Value = "AwaitingAuthorisation")]
        AwaitingAuthorisation,
        [EnumMember(Value = "Consumed")]
        Consumed,
        [EnumMember(Value = "Rejected")]
        Rejected
    }    
}
