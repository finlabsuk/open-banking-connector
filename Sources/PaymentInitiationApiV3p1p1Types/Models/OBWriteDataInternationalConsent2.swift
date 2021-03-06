//
// OBWriteDataInternationalConsent2.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation



public struct OBWriteDataInternationalConsent2: Codable {

    public var initiation: OBInternational2
    public var authorisation: OBAuthorisation1?

    public init(initiation: OBInternational2, authorisation: OBAuthorisation1?) {
        self.initiation = initiation
        self.authorisation = authorisation
    }

    public enum CodingKeys: String, CodingKey { 
        case initiation = "Initiation"
        case authorisation = "Authorisation"
    }


}

