//
// OBWriteInternationalScheduledResponse3Data.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation



public struct OBWriteInternationalScheduledResponse3Data: Codable {

    public enum Status: String, Codable { 
        case cancelled = "Cancelled"
        case initiationCompleted = "InitiationCompleted"
        case initiationFailed = "InitiationFailed"
        case initiationPending = "InitiationPending"
    }
    /** OB: Unique identification as assigned by the ASPSP to uniquely identify the international scheduled payment resource. */
    public var internationalScheduledPaymentId: String
    /** OB: Unique identification as assigned by the ASPSP to uniquely identify the consent resource. */
    public var consentId: String
    /** Date and time at which the message was created.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00 */
    public var creationDateTime: Date
    /** Specifies the status of the payment order resource. */
    public var status: Status
    /** Date and time at which the resource status was updated.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00 */
    public var statusUpdateDateTime: Date
    /** Expected execution date and time for the payment resource.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00 */
    public var expectedExecutionDateTime: Date?
    /** Expected settlement date and time for the payment resource.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00 */
    public var expectedSettlementDateTime: Date?
    public var charges: [OBWriteDomesticConsentResponse3DataCharges]?
    public var exchangeRateInformation: OBWriteInternationalConsentResponse3DataExchangeRateInformation?
    public var initiation: OBWriteInternationalScheduled2DataInitiation
    public var multiAuthorisation: OBWriteDomesticResponse3DataMultiAuthorisation?

    public init(internationalScheduledPaymentId: String, consentId: String, creationDateTime: Date, status: Status, statusUpdateDateTime: Date, expectedExecutionDateTime: Date?, expectedSettlementDateTime: Date?, charges: [OBWriteDomesticConsentResponse3DataCharges]?, exchangeRateInformation: OBWriteInternationalConsentResponse3DataExchangeRateInformation?, initiation: OBWriteInternationalScheduled2DataInitiation, multiAuthorisation: OBWriteDomesticResponse3DataMultiAuthorisation?) {
        self.internationalScheduledPaymentId = internationalScheduledPaymentId
        self.consentId = consentId
        self.creationDateTime = creationDateTime
        self.status = status
        self.statusUpdateDateTime = statusUpdateDateTime
        self.expectedExecutionDateTime = expectedExecutionDateTime
        self.expectedSettlementDateTime = expectedSettlementDateTime
        self.charges = charges
        self.exchangeRateInformation = exchangeRateInformation
        self.initiation = initiation
        self.multiAuthorisation = multiAuthorisation
    }

    public enum CodingKeys: String, CodingKey { 
        case internationalScheduledPaymentId = "InternationalScheduledPaymentId"
        case consentId = "ConsentId"
        case creationDateTime = "CreationDateTime"
        case status = "Status"
        case statusUpdateDateTime = "StatusUpdateDateTime"
        case expectedExecutionDateTime = "ExpectedExecutionDateTime"
        case expectedSettlementDateTime = "ExpectedSettlementDateTime"
        case charges = "Charges"
        case exchangeRateInformation = "ExchangeRateInformation"
        case initiation = "Initiation"
        case multiAuthorisation = "MultiAuthorisation"
    }


}

