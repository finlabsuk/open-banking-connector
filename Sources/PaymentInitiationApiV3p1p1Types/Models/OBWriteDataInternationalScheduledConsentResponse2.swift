//
// OBWriteDataInternationalScheduledConsentResponse2.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation



public struct OBWriteDataInternationalScheduledConsentResponse2: Codable {

    /** OB: Unique identification as assigned by the ASPSP to uniquely identify the consent resource. */
    public var consentId: String
    /** Date and time at which the resource was created. All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00 */
    public var creationDateTime: Date
    public var status: OBExternalConsentStatus1Code
    /** Date and time at which the resource status was updated. All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00 */
    public var statusUpdateDateTime: Date
    public var permission: OBExternalPermissions2Code
    /** Specified cut-off date and time for the payment consent. All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00 */
    public var cutOffDateTime: Date?
    /** Expected execution date and time for the payment resource. All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00 */
    public var expectedExecutionDateTime: Date?
    /** Expected settlement date and time for the payment resource. All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00 */
    public var expectedSettlementDateTime: Date?
    /** Set of elements used to provide details of a charge for the payment initiation. */
    public var charges: [OBCharge2]?
    public var exchangeRateInformation: OBExchangeRate2?
    public var initiation: OBInternationalScheduled2
    public var authorisation: OBAuthorisation1?

    public init(consentId: String, creationDateTime: Date, status: OBExternalConsentStatus1Code, statusUpdateDateTime: Date, permission: OBExternalPermissions2Code, cutOffDateTime: Date?, expectedExecutionDateTime: Date?, expectedSettlementDateTime: Date?, charges: [OBCharge2]?, exchangeRateInformation: OBExchangeRate2?, initiation: OBInternationalScheduled2, authorisation: OBAuthorisation1?) {
        self.consentId = consentId
        self.creationDateTime = creationDateTime
        self.status = status
        self.statusUpdateDateTime = statusUpdateDateTime
        self.permission = permission
        self.cutOffDateTime = cutOffDateTime
        self.expectedExecutionDateTime = expectedExecutionDateTime
        self.expectedSettlementDateTime = expectedSettlementDateTime
        self.charges = charges
        self.exchangeRateInformation = exchangeRateInformation
        self.initiation = initiation
        self.authorisation = authorisation
    }

    public enum CodingKeys: String, CodingKey { 
        case consentId = "ConsentId"
        case creationDateTime = "CreationDateTime"
        case status = "Status"
        case statusUpdateDateTime = "StatusUpdateDateTime"
        case permission = "Permission"
        case cutOffDateTime = "CutOffDateTime"
        case expectedExecutionDateTime = "ExpectedExecutionDateTime"
        case expectedSettlementDateTime = "ExpectedSettlementDateTime"
        case charges = "Charges"
        case exchangeRateInformation = "ExchangeRateInformation"
        case initiation = "Initiation"
        case authorisation = "Authorisation"
    }


}

