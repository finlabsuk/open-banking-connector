// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> Information that locates and identifies a specific address, as defined by postal services. </summary>
    public partial class OBPostalAddress6
    {
        /// <summary> Initializes a new instance of OBPostalAddress6. </summary>
        internal OBPostalAddress6()
        {
            AddressLine = new ChangeTrackingList<string>();
        }

        /// <summary> Initializes a new instance of OBPostalAddress6. </summary>
        /// <param name="addressType"> Identifies the nature of the postal address. </param>
        /// <param name="department"> Identification of a division of a large organisation or building. </param>
        /// <param name="subDepartment"> Identification of a sub-division of a large organisation or building. </param>
        /// <param name="streetName"> Name of a street or thoroughfare. </param>
        /// <param name="buildingNumber"> Number that identifies the position of a building on a street. </param>
        /// <param name="postCode"> Identifier consisting of a group of letters and/or numbers that is added to a postal address to assist the sorting of mail. </param>
        /// <param name="townName"> Name of a built-up area, with defined boundaries, and a local government. </param>
        /// <param name="countrySubDivision"> Identifies a subdivision of a country such as state, region, county. </param>
        /// <param name="country"> Nation with its own government. </param>
        /// <param name="addressLine"></param>
        internal OBPostalAddress6(OBAddressTypeCodeEnum? addressType, string department, string subDepartment, string streetName, string buildingNumber, string postCode, string townName, string countrySubDivision, string country, IReadOnlyList<string> addressLine)
        {
            AddressType = addressType;
            Department = department;
            SubDepartment = subDepartment;
            StreetName = streetName;
            BuildingNumber = buildingNumber;
            PostCode = postCode;
            TownName = townName;
            CountrySubDivision = countrySubDivision;
            Country = country;
            AddressLine = addressLine;
        }

        /// <summary> Identifies the nature of the postal address. </summary>
        public OBAddressTypeCodeEnum? AddressType { get; }
        /// <summary> Identification of a division of a large organisation or building. </summary>
        public string Department { get; }
        /// <summary> Identification of a sub-division of a large organisation or building. </summary>
        public string SubDepartment { get; }
        /// <summary> Name of a street or thoroughfare. </summary>
        public string StreetName { get; }
        /// <summary> Number that identifies the position of a building on a street. </summary>
        public string BuildingNumber { get; }
        /// <summary> Identifier consisting of a group of letters and/or numbers that is added to a postal address to assist the sorting of mail. </summary>
        public string PostCode { get; }
        /// <summary> Name of a built-up area, with defined boundaries, and a local government. </summary>
        public string TownName { get; }
        /// <summary> Identifies a subdivision of a country such as state, region, county. </summary>
        public string CountrySubDivision { get; }
        /// <summary> Nation with its own government. </summary>
        public string Country { get; }
        /// <summary> Gets the address line. </summary>
        public IReadOnlyList<string> AddressLine { get; }
    }
}
