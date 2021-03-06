//
// OBReadProduct2DataOtherProductTypeLoanInterestOtherLoanProviderInterestRateType.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation


/** Other loan interest rate types which are not available in the standard code list */

public struct OBReadProduct2DataOtherProductTypeLoanInterestOtherLoanProviderInterestRateType: Codable {

    public var code: OBCodeMnemonic?
    public var name: Name3
    public var _description: Description3

    public init(code: OBCodeMnemonic?, name: Name3, _description: Description3) {
        self.code = code
        self.name = name
        self._description = _description
    }

    public enum CodingKeys: String, CodingKey { 
        case code = "Code"
        case name = "Name"
        case _description = "Description"
    }


}

