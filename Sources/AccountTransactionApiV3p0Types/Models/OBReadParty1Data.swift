//
// OBReadParty1Data.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation



public struct OBReadParty1Data: Codable {

    public var party: OBParty1?

    public init(party: OBParty1?) {
        self.party = party
    }

    public enum CodingKeys: String, CodingKey { 
        case party = "Party"
    }


}

