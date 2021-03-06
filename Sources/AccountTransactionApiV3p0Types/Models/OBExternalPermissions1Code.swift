//
// OBExternalPermissions1Code.swift
//
// Generated by swagger-codegen
// https://github.com/swagger-api/swagger-codegen
//

import Foundation


/** Specifies the Open Banking account access data types. This is a list of the data clusters being consented by the PSU, and requested for authorisation with the ASPSP. */
public enum OBExternalPermissions1Code: String, Codable {
    case readAccountsBasic = "ReadAccountsBasic"
    case readAccountsDetail = "ReadAccountsDetail"
    case readBalances = "ReadBalances"
    case readBeneficiariesBasic = "ReadBeneficiariesBasic"
    case readBeneficiariesDetail = "ReadBeneficiariesDetail"
    case readDirectDebits = "ReadDirectDebits"
    case readOffers = "ReadOffers"
    case readPAN = "ReadPAN"
    case readParty = "ReadParty"
    case readPartyPSU = "ReadPartyPSU"
    case readProducts = "ReadProducts"
    case readScheduledPaymentsBasic = "ReadScheduledPaymentsBasic"
    case readScheduledPaymentsDetail = "ReadScheduledPaymentsDetail"
    case readStandingOrdersBasic = "ReadStandingOrdersBasic"
    case readStandingOrdersDetail = "ReadStandingOrdersDetail"
    case readStatementsBasic = "ReadStatementsBasic"
    case readStatementsDetail = "ReadStatementsDetail"
    case readTransactionsBasic = "ReadTransactionsBasic"
    case readTransactionsCredits = "ReadTransactionsCredits"
    case readTransactionsDebits = "ReadTransactionsDebits"
    case readTransactionsDetail = "ReadTransactionsDetail"

}
