// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using Azure.Core;

namespace AccountAndTransactionAPISpecification.Models
{
    /// <summary> Postal address of a party. </summary>
    public partial class OBParty2AddressItem
    {
        /// <summary> Initializes a new instance of OBParty2AddressItem. </summary>
        /// <param name="country"> Nation with its own government, occupying a particular territory. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="country"/> is null. </exception>
        internal OBParty2AddressItem(string country)
        {
            if (country == null)
            {
                throw new ArgumentNullException(nameof(country));
            }

            AddressLine = new ChangeTrackingList<string>();
            Country = country;
        }

        /// <summary> Initializes a new instance of OBParty2AddressItem. </summary>
        /// <param name="addressType"> Identifies the nature of the postal address. </param>
        /// <param name="addressLine"></param>
        /// <param name="streetName"> Name of a street or thoroughfare. </param>
        /// <param name="buildingNumber"> Number that identifies the position of a building on a street. </param>
        /// <param name="postCode"> Identifier consisting of a group of letters and/or numbers that is added to a postal address to assist the sorting of mail. </param>
        /// <param name="townName"> Name of a built-up area, with defined boundaries, and a local government. </param>
        /// <param name="countrySubDivision"> Identifies a subdivision of a country eg, state, region, county. </param>
        /// <param name="country"> Nation with its own government, occupying a particular territory. </param>
        internal OBParty2AddressItem(OBAddressTypeCodeEnum? addressType, IReadOnlyList<string> addressLine, string streetName, string buildingNumber, string postCode, string townName, string countrySubDivision, string country)
        {
            AddressType = addressType;
            AddressLine = addressLine;
            StreetName = streetName;
            BuildingNumber = buildingNumber;
            PostCode = postCode;
            TownName = townName;
            CountrySubDivision = countrySubDivision;
            Country = country;
        }

        /// <summary> Identifies the nature of the postal address. </summary>
        public OBAddressTypeCodeEnum? AddressType { get; }
        /// <summary> Gets the address line. </summary>
        public IReadOnlyList<string> AddressLine { get; }
        /// <summary> Name of a street or thoroughfare. </summary>
        public string StreetName { get; }
        /// <summary> Number that identifies the position of a building on a street. </summary>
        public string BuildingNumber { get; }
        /// <summary> Identifier consisting of a group of letters and/or numbers that is added to a postal address to assist the sorting of mail. </summary>
        public string PostCode { get; }
        /// <summary> Name of a built-up area, with defined boundaries, and a local government. </summary>
        public string TownName { get; }
        /// <summary> Identifies a subdivision of a country eg, state, region, county. </summary>
        public string CountrySubDivision { get; }
        /// <summary> Nation with its own government, occupying a particular territory. </summary>
        public string Country { get; }
    }
}
