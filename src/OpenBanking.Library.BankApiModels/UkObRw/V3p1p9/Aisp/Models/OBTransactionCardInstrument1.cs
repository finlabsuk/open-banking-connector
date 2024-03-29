// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary> Set of elements to describe the card instrument used in the transaction. </summary>
    [SourceApiEquivalent(typeof(V3p1p7.Aisp.Models.OBTransactionCardInstrument1))]
    public partial class OBTransactionCardInstrument1
    {
        /// <summary> Initializes a new instance of OBTransactionCardInstrument1. </summary>
        /// <param name="cardSchemeName"> Name of the card scheme. </param>
        public OBTransactionCardInstrument1(OBTransactionCardInstrument1CardSchemeNameEnum cardSchemeName)
        {
            CardSchemeName = cardSchemeName;
        }

        /// <summary> Initializes a new instance of OBTransactionCardInstrument1. </summary>
        /// <param name="cardSchemeName"> Name of the card scheme. </param>
        /// <param name="authorisationType"> The card authorisation type. </param>
        /// <param name="name"> Name of the cardholder using the card instrument. </param>
        /// <param name="identification"> Identification assigned by an institution to identify the card instrument used in the transaction. This identification is known by the account owner, and may be masked. </param>
        public OBTransactionCardInstrument1(OBTransactionCardInstrument1CardSchemeNameEnum cardSchemeName, OBTransactionCardInstrument1AuthorisationTypeEnum? authorisationType, string name, string identification)
        {
            CardSchemeName = cardSchemeName;
            AuthorisationType = authorisationType;
            Name = name;
            Identification = identification;
        }

        /// <summary> Name of the card scheme. </summary>
        public OBTransactionCardInstrument1CardSchemeNameEnum CardSchemeName { get; }
        /// <summary> The card authorisation type. </summary>
        public OBTransactionCardInstrument1AuthorisationTypeEnum? AuthorisationType { get; }
        /// <summary> Name of the cardholder using the card instrument. </summary>
        public string Name { get; }
        /// <summary> Identification assigned by an institution to identify the card instrument used in the transaction. This identification is known by the account owner, and may be masked. </summary>
        public string Identification { get; }
    }
}
