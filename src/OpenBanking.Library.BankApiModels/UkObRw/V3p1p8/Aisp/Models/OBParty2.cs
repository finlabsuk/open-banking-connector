// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> The OBParty2. </summary>
    public partial class OBParty2
    {
        /// <summary> Initializes a new instance of OBParty2. </summary>
        /// <param name="partyId"> A unique and immutable identifier used to identify the customer resource. This identifier has no meaning to the account owner. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="partyId"/> is null. </exception>
        public OBParty2(string partyId)
        {
            if (partyId == null)
            {
                throw new ArgumentNullException(nameof(partyId));
            }

            PartyId = partyId;
            Address = new ChangeTrackingList<OBParty2AddressItem>();
        }

        /// <summary> Initializes a new instance of OBParty2. </summary>
        /// <param name="partyId"> A unique and immutable identifier used to identify the customer resource. This identifier has no meaning to the account owner. </param>
        /// <param name="partyNumber"> Number assigned by an agent to identify its customer. </param>
        /// <param name="partyType"> Party type, in a coded form. </param>
        /// <param name="name"> Name by which a party is known and which is usually used to identify that party. </param>
        /// <param name="fullLegalName"> Specifies a character string with a maximum length of 350 characters. </param>
        /// <param name="legalStructure"> Legal standing of the party. </param>
        /// <param name="beneficialOwnership"></param>
        /// <param name="accountRole"> A party’s role with respect to the related account. </param>
        /// <param name="emailAddress"> Address for electronic mail (e-mail). </param>
        /// <param name="phone"> Collection of information that identifies a phone number, as defined by telecom services. </param>
        /// <param name="mobile"> Collection of information that identifies a mobile phone number, as defined by telecom services. </param>
        /// <param name="relationships"> The Party&apos;s relationships with other resources. </param>
        /// <param name="address"></param>
        public OBParty2(string partyId, string partyNumber, OBExternalPartyType1CodeEnum? partyType, string name, string fullLegalName, string legalStructure, bool? beneficialOwnership, string accountRole, string emailAddress, string phone, string mobile, OBPartyRelationships1 relationships, IReadOnlyList<OBParty2AddressItem> address)
        {
            PartyId = partyId;
            PartyNumber = partyNumber;
            PartyType = partyType;
            Name = name;
            FullLegalName = fullLegalName;
            LegalStructure = legalStructure;
            BeneficialOwnership = beneficialOwnership;
            AccountRole = accountRole;
            EmailAddress = emailAddress;
            Phone = phone;
            Mobile = mobile;
            Relationships = relationships;
            Address = address;
        }

        /// <summary> A unique and immutable identifier used to identify the customer resource. This identifier has no meaning to the account owner. </summary>
        public string PartyId { get; }
        /// <summary> Number assigned by an agent to identify its customer. </summary>
        public string PartyNumber { get; }
        /// <summary> Party type, in a coded form. </summary>
        public OBExternalPartyType1CodeEnum? PartyType { get; }
        /// <summary> Name by which a party is known and which is usually used to identify that party. </summary>
        public string Name { get; }
        /// <summary> Specifies a character string with a maximum length of 350 characters. </summary>
        public string FullLegalName { get; }
        /// <summary> Legal standing of the party. </summary>
        public string LegalStructure { get; }
        /// <summary> Gets the beneficial ownership. </summary>
        public bool? BeneficialOwnership { get; }
        /// <summary> A party’s role with respect to the related account. </summary>
        public string AccountRole { get; }
        /// <summary> Address for electronic mail (e-mail). </summary>
        public string EmailAddress { get; }
        /// <summary> Collection of information that identifies a phone number, as defined by telecom services. </summary>
        public string Phone { get; }
        /// <summary> Collection of information that identifies a mobile phone number, as defined by telecom services. </summary>
        public string Mobile { get; }
        /// <summary> The Party&apos;s relationships with other resources. </summary>
        public OBPartyRelationships1 Relationships { get; }
        /// <summary> Gets the address. </summary>
        public IReadOnlyList<OBParty2AddressItem> Address { get; }
    }
}
