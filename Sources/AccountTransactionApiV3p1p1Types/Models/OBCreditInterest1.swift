//
// OBCreditInterest1.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation


/** Details about the interest that may be payable to the Account holders */

public struct OBCreditInterest1: Codable {

    /** The group of tiers or bands for which credit interest can be applied. */
    public var tierBandSet: [OBTierBandSet1]

    public init(tierBandSet: [OBTierBandSet1]) {
        self.tierBandSet = tierBandSet
    }

    public enum CodingKeys: String, CodingKey { 
        case tierBandSet = "TierBandSet"
    }


}

