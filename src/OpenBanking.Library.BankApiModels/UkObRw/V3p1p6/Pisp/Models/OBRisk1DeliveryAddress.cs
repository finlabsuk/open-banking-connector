// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Information that locates and identifies a specific address, as defined
    /// by postal services or in free format text.
    /// </summary>
    [SourceApiEquivalent(typeof(V3p1p4.Pisp.Models.OBRisk1DeliveryAddress),
        ValueMappingSourceMembers = new string[]
        {
            "CountrySubDivision",
        },
        ValueMappingDestinationMembers = new []
        {
            "CountrySubDivision"
        },
        ValueMappings = new []
        {
            ValueMapping.CommaDelimitedStringToIEnumerableReverse
        })
    ]
    [TargetApiEquivalent(typeof(V3p1p4.Pisp.Models.OBRisk1DeliveryAddress),
        ValueMappingSourceMembers = new string[]
        {
            "CountrySubDivision",
        },
        ValueMappingDestinationMembers = new []
        {
            "CountrySubDivision"
        },
        ValueMappings = new []
        {
            ValueMapping.CommaDelimitedStringToIEnumerable
        })
    ]
    public partial class OBRisk1DeliveryAddress
    {
        /// <summary>
        /// Initializes a new instance of the OBRisk1DeliveryAddress class.
        /// </summary>
        public OBRisk1DeliveryAddress()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBRisk1DeliveryAddress class.
        /// </summary>
        /// <param name="country">Nation with its own government, occupying a
        /// particular territory.</param>
        public OBRisk1DeliveryAddress(string townName, string country, IList<string> addressLine = default(IList<string>), string streetName = default(string), string buildingNumber = default(string), string postCode = default(string), string countrySubDivision = default(string))
        {
            AddressLine = addressLine;
            StreetName = streetName;
            BuildingNumber = buildingNumber;
            PostCode = postCode;
            TownName = townName;
            CountrySubDivision = countrySubDivision;
            Country = country;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "AddressLine")]
        public IList<string> AddressLine { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "StreetName")]
        public string StreetName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "BuildingNumber")]
        public string BuildingNumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "PostCode")]
        public string PostCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "TownName")]
        public string TownName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "CountrySubDivision")]
        public string CountrySubDivision { get; set; }

        /// <summary>
        /// Gets or sets nation with its own government, occupying a particular
        /// territory.
        /// </summary>
        [JsonProperty(PropertyName = "Country")]
        public string Country { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (TownName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "TownName");
            }
            if (Country == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Country");
            }
        }
    }
}
