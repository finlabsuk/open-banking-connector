// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> The OBBeneficiary5Detail. </summary>
    internal partial class OBBeneficiary5Detail
    {
        /// <summary> Initializes a new instance of OBBeneficiary5Detail. </summary>
        /// <param name="creditorAccount"> Provides the details to identify the beneficiary account. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="creditorAccount"/> is null. </exception>
        internal OBBeneficiary5Detail(OBCashAccount50 creditorAccount)
        {
            if (creditorAccount == null)
            {
                throw new ArgumentNullException(nameof(creditorAccount));
            }

            SupplementaryData = new ChangeTrackingDictionary<string, object>();
            CreditorAccount = creditorAccount;
        }

        /// <summary> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </summary>
        public string AccountId { get; }
        /// <summary> A unique and immutable identifier used to identify the beneficiary resource. This identifier has no meaning to the account owner. </summary>
        public string BeneficiaryId { get; }
        /// <summary> Specifies the Beneficiary Type. </summary>
        public OBBeneficiaryType1CodeEnum? BeneficiaryType { get; }
        /// <summary>
        /// Unique reference, as assigned by the creditor, to unambiguously refer to the payment transaction.
        /// Usage: If available, the initiating party should provide this reference in the structured remittance information, to enable reconciliation by the creditor upon receipt of the amount of money.
        /// If the business context requires the use of a creditor reference or a payment remit identification, and only one identifier can be passed through the end-to-end chain, the creditor&apos;s reference or payment remittance identification should be quoted in the end-to-end transaction identification.
        /// </summary>
        public string Reference { get; }
        /// <summary> Additional information that can not be captured in the structured fields and/or any other specific block. </summary>
        public IReadOnlyDictionary<string, object> SupplementaryData { get; }
        /// <summary>
        /// Party that manages the account on behalf of the account owner, that is manages the registration and booking of entries on the account, calculates balances on the account and provides information about the account.
        /// This is the servicer of the beneficiary account.
        /// </summary>
        public OBBranchAndFinancialInstitutionIdentification60 CreditorAgent { get; }
        /// <summary> Provides the details to identify the beneficiary account. </summary>
        public OBCashAccount50 CreditorAccount { get; }
    }
}
