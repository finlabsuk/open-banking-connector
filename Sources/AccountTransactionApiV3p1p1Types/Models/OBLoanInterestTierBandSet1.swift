//
// OBLoanInterestTierBandSet1.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation


/** The group of tiers or bands for which debit interest can be applied. */

public struct OBLoanInterestTierBandSet1: Codable {

    public var tierBandMethod: OBTierBandType1Code
    /** Loan interest tierbandset identification. Used by  loan providers for internal use purpose. */
    public var identification: String?
    public var calculationMethod: OBInterestCalculationMethod1Code
    /** Optional additional notes to supplement the Tier Band Set details */
    public var notes: [String]?
    public var otherCalculationMethod: OBOtherCodeType1?
    /** Tier Band Details */
    public var loanInterestTierBand: [OBLoanInterestTierBand1]
    /** Contains details of fees and charges which are not associated with either LoanRepayment or features/benefits */
    public var loanInterestFeesCharges: [OBLoanInterestFeesCharges1]?

    public init(tierBandMethod: OBTierBandType1Code, identification: String?, calculationMethod: OBInterestCalculationMethod1Code, notes: [String]?, otherCalculationMethod: OBOtherCodeType1?, loanInterestTierBand: [OBLoanInterestTierBand1], loanInterestFeesCharges: [OBLoanInterestFeesCharges1]?) {
        self.tierBandMethod = tierBandMethod
        self.identification = identification
        self.calculationMethod = calculationMethod
        self.notes = notes
        self.otherCalculationMethod = otherCalculationMethod
        self.loanInterestTierBand = loanInterestTierBand
        self.loanInterestFeesCharges = loanInterestFeesCharges
    }

    public enum CodingKeys: String, CodingKey { 
        case tierBandMethod = "TierBandMethod"
        case identification = "Identification"
        case calculationMethod = "CalculationMethod"
        case notes = "Notes"
        case otherCalculationMethod = "OtherCalculationMethod"
        case loanInterestTierBand = "LoanInterestTierBand"
        case loanInterestFeesCharges = "LoanInterestFeesCharges"
    }


}

