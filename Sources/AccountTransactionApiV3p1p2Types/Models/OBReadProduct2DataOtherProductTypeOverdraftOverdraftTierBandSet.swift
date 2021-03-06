//
// OBReadProduct2DataOtherProductTypeOverdraftOverdraftTierBandSet.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation


/** Tier band set details */

public struct OBReadProduct2DataOtherProductTypeOverdraftOverdraftTierBandSet: Codable {

    public enum TierBandMethod: String, Codable { 
        case inba = "INBA"
        case inti = "INTI"
        case inwh = "INWH"
    }
    public enum OverdraftType: String, Codable { 
        case ovco = "OVCO"
        case ovod = "OVOD"
        case ovot = "OVOT"
    }
    /** The methodology of how overdraft is charged. It can be: &#39;Whole&#39;  Where the same charge/rate is applied to the entirety of the overdraft balance (where charges are applicable).  &#39;Tiered&#39; Where different charges/rates are applied dependent on overdraft maximum and minimum balance amount tiers defined by the lending financial organisation &#39;Banded&#39; Where different charges/rates are applied dependent on overdraft maximum and minimum balance amount bands defined by a government organisation. */
    public var tierBandMethod: TierBandMethod
    /** An overdraft can either be &#39;committed&#39; which means that the facility cannot be withdrawn without reasonable notification before it&#39;s agreed end date, or &#39;on demand&#39; which means that the financial institution can demand repayment at any point in time. */
    public var overdraftType: OverdraftType?
    /** Unique and unambiguous identification of a  Tier Band for a overdraft product. */
    public var identification: String?
    /** Indicates if the Overdraft is authorised (Y) or unauthorised (N) */
    public var authorisedIndicator: Bool?
    /** When a customer exceeds their credit limit, a financial institution will not charge the customer unauthorised overdraft charges if they do not exceed by more than the buffer amount. Note: Authorised overdraft charges may still apply. */
    public var bufferAmount: String?
    public var notes: [String]?
    public var overdraftTierBand: [OBReadProduct2DataOtherProductTypeOverdraftOverdraftTierBand]
    public var overdraftFeesCharges: [OBReadProduct2DataOtherProductTypeOverdraftOverdraftFeesCharges1]?

    public init(tierBandMethod: TierBandMethod, overdraftType: OverdraftType?, identification: String?, authorisedIndicator: Bool?, bufferAmount: String?, notes: [String]?, overdraftTierBand: [OBReadProduct2DataOtherProductTypeOverdraftOverdraftTierBand], overdraftFeesCharges: [OBReadProduct2DataOtherProductTypeOverdraftOverdraftFeesCharges1]?) {
        self.tierBandMethod = tierBandMethod
        self.overdraftType = overdraftType
        self.identification = identification
        self.authorisedIndicator = authorisedIndicator
        self.bufferAmount = bufferAmount
        self.notes = notes
        self.overdraftTierBand = overdraftTierBand
        self.overdraftFeesCharges = overdraftFeesCharges
    }

    public enum CodingKeys: String, CodingKey { 
        case tierBandMethod = "TierBandMethod"
        case overdraftType = "OverdraftType"
        case identification = "Identification"
        case authorisedIndicator = "AuthorisedIndicator"
        case bufferAmount = "BufferAmount"
        case notes = "Notes"
        case overdraftTierBand = "OverdraftTierBand"
        case overdraftFeesCharges = "OverdraftFeesCharges"
    }


}

