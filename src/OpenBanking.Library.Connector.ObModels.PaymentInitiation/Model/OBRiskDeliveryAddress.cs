// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    /// <summary>
    ///     Information that locates and identifies a specific address, as defined by postal services or in free format text.
    /// </summary>
    [OpenBankingEquivalent(typeof(OBRisk1DeliveryAddress))]
    [OpenBankingEquivalent(typeof(ObModels.PaymentInitiation.V3p1p2.Model.OBRisk1DeliveryAddress))]
    public class OBRiskDeliveryAddress
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OBRiskDeliveryAddress" /> class.
        /// </summary>
        [JsonConstructor]
        public OBRiskDeliveryAddress() { }

        /// <summary>
        ///     Gets or Sets AddressLine
        /// </summary>
        [JsonProperty("addressLine")]
        public List<string> AddressLine { get; set; }

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
        ///     Gets or Sets CountrySubDivision
        /// </summary>
        [JsonProperty("countrySubDivision")]
        public List<string> CountrySubDivision { get; set; }

        /// <summary>
        ///     Nation with its own government, occupying a particular territory.
        /// </summary>
        /// <value>Nation with its own government, occupying a particular territory.</value>
        [JsonProperty("country")]
        public string Country { get; set; }
    }
}
