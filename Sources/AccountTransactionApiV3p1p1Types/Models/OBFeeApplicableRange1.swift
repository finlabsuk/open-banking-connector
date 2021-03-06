//
// OBFeeApplicableRange1.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation


/** Range or amounts or rates for which the fee/charge applies */

public struct OBFeeApplicableRange1: Codable {

    /** Minimum Amount on which fee/charge is applicable (where it is expressed as an amount) */
    public var minimumAmount: Double?
    /** Minimum Amount on which fee/charge is applicable (where it is expressed as an amount) */
    public var maximumAmount: Double?
    /** Minimum Amount on which fee/charge is applicable (where it is expressed as an amount) */
    public var minimumRate: Double?
    /** Minimum Amount on which fee/charge is applicable (where it is expressed as an amount) */
    public var maximumRate: Double?

    public init(minimumAmount: Double?, maximumAmount: Double?, minimumRate: Double?, maximumRate: Double?) {
        self.minimumAmount = minimumAmount
        self.maximumAmount = maximumAmount
        self.minimumRate = minimumRate
        self.maximumRate = maximumRate
    }

    public enum CodingKeys: String, CodingKey { 
        case minimumAmount = "MinimumAmount"
        case maximumAmount = "MaximumAmount"
        case minimumRate = "MinimumRate"
        case maximumRate = "MaximumRate"
    }


}

