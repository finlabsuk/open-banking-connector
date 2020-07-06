using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
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
        [EnumMember(Value = "business")] Business = 1,

        /// <summary>
        ///     Enum Correspondence for value: Correspondence
        /// </summary>
        [EnumMember(Value = "correspondence")] Correspondence = 2,

        /// <summary>
        ///     Enum DeliveryTo for value: DeliveryTo
        /// </summary>
        [EnumMember(Value = "deliveryTo")] DeliveryTo = 3,

        /// <summary>
        ///     Enum MailTo for value: MailTo
        /// </summary>
        [EnumMember(Value = "mailTo")] MailTo = 4,

        /// <summary>
        ///     Enum POBox for value: POBox
        /// </summary>
        [EnumMember(Value = "poBox")] POBox = 5,

        /// <summary>
        ///     Enum Postal for value: Postal
        /// </summary>
        [EnumMember(Value = "postal")] Postal = 6,

        /// <summary>
        ///     Enum Residential for value: Residential
        /// </summary>
        [EnumMember(Value = "residential")] Residential = 7,

        /// <summary>
        ///     Enum Statement for value: Statement
        /// </summary>
        [EnumMember(Value = "statement")] Statement = 8
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
        [EnumMember(Value = "any")] Any = 1,

        /// <summary>
        ///     Enum Single for value: Single
        /// </summary>
        [EnumMember(Value = "single")] Single = 2
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
        [EnumMember(Value = "billPayment")] BillPayment = 1,

        /// <summary>
        ///     Enum ContactlessTravel for value: ContactlessTravel
        /// </summary>
        [EnumMember(Value = "contactlessTravel")]
        ContactlessTravel = 2,

        /// <summary>
        ///     Enum EcommerceGoods for value: EcommerceGoods
        /// </summary>
        [EnumMember(Value = "ecommerceGoods")]
        EcommerceGoods = 3,

        /// <summary>
        ///     Enum EcommerceServices for value: EcommerceServices
        /// </summary>
        [EnumMember(Value = "ecommerceServices")]
        EcommerceServices = 4,

        /// <summary>
        ///     Enum Kiosk for value: Kiosk
        /// </summary>
        [EnumMember(Value = "kiosk")]
        Kiosk = 5,

        /// <summary>
        ///     Enum Parking for value: Parking
        /// </summary>
        [EnumMember(Value = "parking")]
        Parking = 6,

        /// <summary>
        ///     Enum PartyToParty for value: PartyToParty
        /// </summary>
        [EnumMember(Value = "partyToParty")]
        PartyToParty = 7
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
        [EnumMember(Value = "ca")] CA = 1,

        /// <summary>
        ///     Enum SCA for value: SCA
        /// </summary>
        [EnumMember(Value = "sca")] SCA = 2
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWritePaymentResponseDataApiStatus
    {
        [EnumMember(Value = "acceptedCreditSettlementCompleted")]
        AcceptedCreditSettlementCompleted,
        [EnumMember(Value = "acceptedSettlementCompleted")]
        AcceptedSettlementCompleted,
        [EnumMember(Value = "acceptedSettlementInProcess")]
        AcceptedSettlementInProcess,
        [EnumMember(Value = "acceptedWithoutPosting")]
        AcceptedWithoutPosting,
        [EnumMember(Value = "pending")]
        Pending,
        [EnumMember(Value = "rejected")]
        Rejected
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBWritePaymentConsentResponseDataApiStatus
    {
        [EnumMember(Value = "authorised")]
        Authorised,
        [EnumMember(Value = "awaitingAuthorisation")]
        AwaitingAuthorisation,
        [EnumMember(Value = "consumed")]
        Consumed,
        [EnumMember(Value = "rejected")]
        Rejected
    }

    [JsonConverter(typeof(StringEnumConverter))]

    public enum OBChargeBearerType1Code
    {
        /// <summary>
        /// Enum BorneByCreditor for value: BorneByCreditor
        /// </summary>
        [EnumMember(Value = "borneByCreditor")]
        BorneByCreditor = 1,

        /// <summary>
        /// Enum BorneByDebtor for value: BorneByDebtor
        /// </summary>
        [EnumMember(Value = "borneByDebtor")]
        BorneByDebtor = 2,

        /// <summary>
        /// Enum FollowingServiceLevel for value: FollowingServiceLevel
        /// </summary>
        [EnumMember(Value = "followingServiceLevel")]
        FollowingServiceLevel = 3,

        /// <summary>
        /// Enum Shared for value: Shared
        /// </summary>
        [EnumMember(Value = "shared")]
        Shared = 4

    }

    [JsonConverter(typeof(StringEnumConverter))]

    public enum OBTransactionIndividualStatus1Code
    {
        /// <summary>
        /// Enum AcceptedSettlementCompleted for value: AcceptedSettlementCompleted
        /// </summary>
        [EnumMember(Value = "acceptedSettlementCompleted")]
        AcceptedSettlementCompleted = 1,

        /// <summary>
        /// Enum AcceptedSettlementInProcess for value: AcceptedSettlementInProcess
        /// </summary>
        [EnumMember(Value = "acceptedSettlementInProcess")]
        AcceptedSettlementInProcess = 2,

        /// <summary>
        /// Enum Pending for value: Pending
        /// </summary>
        [EnumMember(Value = "pending")]
        Pending = 3,

        /// <summary>
        /// Enum Rejected for value: Rejected
        /// </summary>
        [EnumMember(Value = "rejected")]
        Rejected = 4

    }

    [JsonConverter(typeof(StringEnumConverter))]

    public enum OBExternalStatusCode
    {
        /// <summary>
        /// Enum Authorised for value: Authorised
        /// </summary>
        [EnumMember(Value = "authorised")]
        Authorised = 1,

        /// <summary>
        /// Enum AwaitingFurtherAuthorisation for value: AwaitingFurtherAuthorisation
        /// </summary>
        [EnumMember(Value = "awaitingFurtherAuthorisation")]
        AwaitingFurtherAuthorisation = 2,

        /// <summary>
        /// Enum Rejected for value: Rejected
        /// </summary>
        [EnumMember(Value = "rejected")]
        Rejected = 3

    }
}
