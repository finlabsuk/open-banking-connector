//
// OBWriteInternationalStandingOrderConsent4.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation



public struct OBWriteInternationalStandingOrderConsent4: Codable {

    public var data: OBWriteInternationalStandingOrderConsent4Data
    public var risk: OBRisk1

    public init(data: OBWriteInternationalStandingOrderConsent4Data, risk: OBRisk1) {
        self.data = data
        self.risk = risk
    }

    public enum CodingKeys: String, CodingKey { 
        case data = "Data"
        case risk = "Risk"
    }


}

