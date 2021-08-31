// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Information that locates and identifies a specific address, as defined
    /// by postal services.
    /// </summary>
    [TargetApiEquivalent(typeof(V3p1p4.Pisp.Models.OBPostalAddress6))]
    [SourceApiEquivalent(typeof(V3p1p4.Pisp.Models.OBPostalAddress6))]
    public partial class OBPostalAddress6
    {
        /// <summary>
        /// Initializes a new instance of the OBPostalAddress6 class.
        /// </summary>
        public OBPostalAddress6()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBPostalAddress6 class.
        /// </summary>
        /// <param name="addressType">Possible values include: 'Business',
        /// 'Correspondence', 'DeliveryTo', 'MailTo', 'POBox', 'Postal',
        /// 'Residential', 'Statement'</param>
        public OBPostalAddress6(OBAddressTypeCodeEnum? addressType = default(OBAddressTypeCodeEnum?), string department = default(string), string subDepartment = default(string), string streetName = default(string), string buildingNumber = default(string), string postCode = default(string), string townName = default(string), string countrySubDivision = default(string), string country = default(string), IList<string> addressLine = default(IList<string>))
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
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'Business', 'Correspondence',
        /// 'DeliveryTo', 'MailTo', 'POBox', 'Postal', 'Residential',
        /// 'Statement'
        /// </summary>
        [JsonProperty(PropertyName = "AddressType")]
        public OBAddressTypeCodeEnum? AddressType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Department")]
        public string Department { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SubDepartment")]
        public string SubDepartment { get; set; }

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
        /// </summary>
        [JsonProperty(PropertyName = "Country")]
        public string Country { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "AddressLine")]
        public IList<string> AddressLine { get; set; }

    }
}
