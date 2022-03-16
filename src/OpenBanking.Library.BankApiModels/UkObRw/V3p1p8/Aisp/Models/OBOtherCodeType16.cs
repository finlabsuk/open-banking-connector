// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> Other application frequencies not covered in the standard code list. </summary>
    internal partial class OBOtherCodeType16
    {
        /// <summary> Initializes a new instance of OBOtherCodeType16. </summary>
        /// <param name="name"> Long name associated with the code. </param>
        /// <param name="description"> Description to describe the purpose of the code. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name"/> or <paramref name="description"/> is null. </exception>
        internal OBOtherCodeType16(string name, string description)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            Name = name;
            Description = description;
        }

        /// <summary> The four letter Mnemonic used within an XML file to identify a code. </summary>
        public string Code { get; }
        /// <summary> Long name associated with the code. </summary>
        public string Name { get; }
        /// <summary> Description to describe the purpose of the code. </summary>
        public string Description { get; }
    }
}
