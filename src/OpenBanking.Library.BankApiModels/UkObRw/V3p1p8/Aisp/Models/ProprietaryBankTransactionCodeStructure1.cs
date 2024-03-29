// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> Set of elements to fully identify a proprietary bank transaction code. </summary>
    public partial class ProprietaryBankTransactionCodeStructure1
    {
        /// <summary> Initializes a new instance of ProprietaryBankTransactionCodeStructure1. </summary>
        /// <param name="code"> Proprietary bank transaction code to identify the underlying transaction. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="code"/> is null. </exception>
        public ProprietaryBankTransactionCodeStructure1(string code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            Code = code;
        }

        /// <summary> Initializes a new instance of ProprietaryBankTransactionCodeStructure1. </summary>
        /// <param name="code"> Proprietary bank transaction code to identify the underlying transaction. </param>
        /// <param name="issuer"> Identification of the issuer of the proprietary bank transaction code. </param>
        public ProprietaryBankTransactionCodeStructure1(string code, string issuer)
        {
            Code = code;
            Issuer = issuer;
        }

        /// <summary> Proprietary bank transaction code to identify the underlying transaction. </summary>
        public string Code { get; }
        /// <summary> Identification of the issuer of the proprietary bank transaction code. </summary>
        public string Issuer { get; }
    }
}
