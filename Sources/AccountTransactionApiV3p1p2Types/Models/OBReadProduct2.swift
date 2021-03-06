//
// OBReadProduct2.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation


/** Product details of Other Product which is not avaiable in the standard list */

public struct OBReadProduct2: Codable {

    public var data: OBReadProduct2Data
    public var links: Links?
    public var meta: Meta?

    public init(data: OBReadProduct2Data, links: Links?, meta: Meta?) {
        self.data = data
        self.links = links
        self.meta = meta
    }

    public enum CodingKeys: String, CodingKey { 
        case data = "Data"
        case links = "Links"
        case meta = "Meta"
    }


}

