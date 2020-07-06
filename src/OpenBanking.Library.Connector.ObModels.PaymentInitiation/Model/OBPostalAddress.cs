// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    /// <summary>
    ///     Information that locates and identifies a specific address, as defined by postal services.
    /// </summary>
    [OpenBankingEquivalent(typeof(OBPostalAddress6))]
    [OpenBankingEquivalent(typeof(ObModels.PaymentInitiation.V3p1p2.Model.OBPostalAddress6))]
    [SourceApiEquivalent(typeof(OBPostalAddress6))]
    [SourceApiEquivalent(typeof(ObModels.PaymentInitiation.V3p1p2.Model.OBPostalAddress6))]
    public class OBPostalAddress
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OBPostalAddress" /> class.
        /// </summary>
        public OBPostalAddress()
        {
        }

        /// <summary>
        ///     Gets or Sets AddressType
        /// </summary>
        [JsonProperty("addressType")]
        public OBAddressTypeCode? AddressType { get; set; }

        /// <summary>
        ///     Identification of a division of a large organisation or building.
        /// </summary>
        /// <value>Identification of a division of a large organisation or building.</value>
        [JsonProperty("department")]
        public string Department { get; set; }

        /// <summary>
        ///     Identification of a sub-division of a large organisation or building.
        /// </summary>
        /// <value>Identification of a sub-division of a large organisation or building.</value>
        [JsonProperty("subDepartment")]
        public string SubDepartment { get; set; }

        /// <summary>
        ///     Name of a street or thoroughfare.
        /// </summary>
        /// <value>Name of a street or thoroughfare.</value>
        [JsonProperty("streetName")]
        public string StreetName { get; set; }

        /// <summary>
        ///     Number that identifies the position of a building on a street.
        /// </summary>
        /// <value>Number that identifies the position of a building on a street.</value>
        [JsonProperty("buildingNumber")]
        public string BuildingNumber { get; set; }

        /// <summary>
        ///     Identifier consisting of a group of letters and/or numbers that is added to a postal address to assist the sorting
        ///     of mail.
        /// </summary>
        /// <value>
        ///     Identifier consisting of a group of letters and/or numbers that is added to a postal address to assist the
        ///     sorting of mail.
        /// </value>
        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        /// <summary>
        ///     Name of a built-up area, with defined boundaries, and a local government.
        /// </summary>
        /// <value>Name of a built-up area, with defined boundaries, and a local government.</value>
        [JsonProperty("townName")]
        public string TownName { get; set; }

        /// <summary>
        ///     Identifies a subdivision of a country such as state, region, county.
        /// </summary>
        /// <value>Identifies a subdivision of a country such as state, region, county.</value>
        [JsonProperty("countrySubDivision")]
        public string CountrySubDivision { get; set; }

        /// <summary>
        ///     Nation with its own government.
        /// </summary>
        /// <value>Nation with its own government.</value>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        ///     Gets or Sets AddressLine
        /// </summary>
        [JsonProperty("addressLine")]
        public List<string> AddressLine { get; set; }
    }
}
