//
// CreditInterest1TierBand.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation


/** Tier Band Details */

public struct CreditInterest1TierBand: Codable {

    public enum CalculationFrequency: String, Codable { 
        case perAcademicTerm = "PerAcademicTerm"
        case daily = "Daily"
        case halfYearly = "HalfYearly"
        case monthly = "Monthly"
        case other = "Other"
        case quarterly = "Quarterly"
        case perStatementDate = "PerStatementDate"
        case weekly = "Weekly"
        case yearly = "Yearly"
    }
    public enum ApplicationFrequency: String, Codable { 
        case perAcademicTerm = "PerAcademicTerm"
        case daily = "Daily"
        case halfYearly = "HalfYearly"
        case monthly = "Monthly"
        case other = "Other"
        case quarterly = "Quarterly"
        case perStatementDate = "PerStatementDate"
        case weekly = "Weekly"
        case yearly = "Yearly"
    }
    public enum DepositInterestAppliedCoverage: String, Codable { 
        case tiered = "Tiered"
        case whole = "Whole"
    }
    public enum FixedVariableInterestRateType: String, Codable { 
        case fixed = "Fixed"
        case variable = "Variable"
    }
    public enum BankInterestRateType: String, Codable { 
        case linkedBaseRate = "LinkedBaseRate"
        case gross = "Gross"
        case net = "Net"
        case other = "Other"
    }
    /** Unique and unambiguous identification of a  Tier Band for a PCA. */
    public var identification: String?
    /** Minimum deposit value for which the credit interest tier applies. */
    public var tierValueMinimum: String
    /** Maximum deposit value for which the credit interest tier applies. */
    public var tierValueMaximum: String?
    /** How often is credit interest calculated for the account. */
    public var calculationFrequency: CalculationFrequency?
    /** How often is interest applied to the PCA for this tier/band i.e. how often the financial institution pays accumulated interest to the customer&#39;s PCA. */
    public var applicationFrequency: ApplicationFrequency
    /** Amount on which Interest applied. */
    public var depositInterestAppliedCoverage: DepositInterestAppliedCoverage?
    /** Type of interest rate, Fixed or Variable */
    public var fixedVariableInterestRateType: FixedVariableInterestRateType
    /** The annual equivalent rate (AER) is interest that is calculated under the assumption that any interest paid is combined with the original balance and the next interest payment will be based on the slightly higher account balance. Overall, this means that interest can be compounded several times in a year depending on the number of times that interest payments are made.   Read more: Annual Equivalent Rate (AER) http://www.investopedia.com/terms/a/aer.asp#ixzz4gfR7IO1A */
    public var AER: String
    /** Interest rate types, other than AER, which financial institutions may use to describe the annual interest rate payable to the PCA. */
    public var bankInterestRateType: BankInterestRateType?
    /** Bank Interest for the PCA product */
    public var bankInterestRate: String?
    /** Optional additional notes to supplement the Tier Band details */
    public var notes: [String]?
    public var otherBankInterestType: OtherBankInterestType?
    public var otherApplicationFrequency: OtherApplicationFrequency?
    public var otherCalculationFrequency: OtherCalculationFrequency?

    public init(identification: String?, tierValueMinimum: String, tierValueMaximum: String?, calculationFrequency: CalculationFrequency?, applicationFrequency: ApplicationFrequency, depositInterestAppliedCoverage: DepositInterestAppliedCoverage?, fixedVariableInterestRateType: FixedVariableInterestRateType, AER: String, bankInterestRateType: BankInterestRateType?, bankInterestRate: String?, notes: [String]?, otherBankInterestType: OtherBankInterestType?, otherApplicationFrequency: OtherApplicationFrequency?, otherCalculationFrequency: OtherCalculationFrequency?) {
        self.identification = identification
        self.tierValueMinimum = tierValueMinimum
        self.tierValueMaximum = tierValueMaximum
        self.calculationFrequency = calculationFrequency
        self.applicationFrequency = applicationFrequency
        self.depositInterestAppliedCoverage = depositInterestAppliedCoverage
        self.fixedVariableInterestRateType = fixedVariableInterestRateType
        self.AER = AER
        self.bankInterestRateType = bankInterestRateType
        self.bankInterestRate = bankInterestRate
        self.notes = notes
        self.otherBankInterestType = otherBankInterestType
        self.otherApplicationFrequency = otherApplicationFrequency
        self.otherCalculationFrequency = otherCalculationFrequency
    }

    public enum CodingKeys: String, CodingKey { 
        case identification = "Identification"
        case tierValueMinimum = "TierValueMinimum"
        case tierValueMaximum = "TierValueMaximum"
        case calculationFrequency = "CalculationFrequency"
        case applicationFrequency = "ApplicationFrequency"
        case depositInterestAppliedCoverage = "DepositInterestAppliedCoverage"
        case fixedVariableInterestRateType = "FixedVariableInterestRateType"
        case AER
        case bankInterestRateType = "BankInterestRateType"
        case bankInterestRate = "BankInterestRate"
        case notes = "Notes"
        case otherBankInterestType = "OtherBankInterestType"
        case otherApplicationFrequency = "OtherApplicationFrequency"
        case otherCalculationFrequency = "OtherCalculationFrequency"
    }


}

