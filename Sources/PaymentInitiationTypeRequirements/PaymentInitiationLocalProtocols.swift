// ********************************************************************************
//
// This source file is part of the Open Banking Connector project
// ( https://github.com/finlabsuk/open-banking-connector ).
//
// Copyright (C) 2019 Finnovation Labs and the Open Banking Connector project authors.
//
// Licensed under Apache License v2.0. See LICENSE.txt for licence information.
// SPDX-License-Identifier: Apache-2.0
//
// ********************************************************************************

import Foundation

public protocol OBWritePaymentConsentLocalProtocol: Codable {
    associatedtype OBWritePaymentConsentApi: OBWritePaymentConsentApiProtocol
    associatedtype OBWritePaymentApi: OBWritePaymentApiProtocol
    func obWritePaymentConsentApi() throws -> OBWritePaymentConsentApi
    func obWritePaymentApi(consentId: String) throws -> OBWritePaymentApi
}
