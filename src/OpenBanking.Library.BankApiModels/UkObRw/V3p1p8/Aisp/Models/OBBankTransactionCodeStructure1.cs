// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> Set of elements used to fully identify the type of underlying transaction resulting in an entry. </summary>
    public partial class OBBankTransactionCodeStructure1
    {
        /// <summary> Initializes a new instance of OBBankTransactionCodeStructure1. </summary>
        /// <param name="code"> Specifies the family within a domain. </param>
        /// <param name="subCode"> Specifies the sub-product family within a specific family. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="code"/> or <paramref name="subCode"/> is null. </exception>
        public OBBankTransactionCodeStructure1(string code, string subCode)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }
            if (subCode == null)
            {
                throw new ArgumentNullException(nameof(subCode));
            }

            Code = code;
            SubCode = subCode;
        }

        /// <summary> Specifies the family within a domain. </summary>
        public string Code { get; }
        /// <summary> Specifies the sub-product family within a specific family. </summary>
        public string SubCode { get; }
    }
}
