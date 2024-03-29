// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> Details of the merchant involved in the transaction. </summary>
    [SourceApiEquivalent(typeof(V3p1p7.Aisp.Models.OBMerchantDetails1))]
    public partial class OBMerchantDetails1
    {
        /// <summary> Initializes a new instance of OBMerchantDetails1. </summary>
        public OBMerchantDetails1()
        {
        }

        /// <summary> Initializes a new instance of OBMerchantDetails1. </summary>
        /// <param name="merchantName"> Name by which the merchant is known. </param>
        /// <param name="merchantCategoryCode"> Category code conform to ISO 18245, related to the type of services or goods the merchant provides for the transaction. </param>
        [JsonConstructor]
        public OBMerchantDetails1(string merchantName, string merchantCategoryCode)
        {
            MerchantName = merchantName;
            MerchantCategoryCode = merchantCategoryCode;
        }

        /// <summary> Name by which the merchant is known. </summary>
        public string MerchantName { get; }
        /// <summary> Category code conform to ISO 18245, related to the type of services or goods the merchant provides for the transaction. </summary>
        public string MerchantCategoryCode { get; }
    }
}
