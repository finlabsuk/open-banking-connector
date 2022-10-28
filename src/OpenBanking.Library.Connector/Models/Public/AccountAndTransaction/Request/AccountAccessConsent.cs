// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.AccountAndTransaction;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request
{
    public class TemplateRequest
    {
        /// <summary>
        ///     Template type to use.
        /// </summary>
        public AccountAccessConsentTemplateType Type { get; set; }
    }

    /// <summary>
    ///     Request object used to create an AccountAccessConsent. Includes a UK Open Banking request object
    ///     plus information on the bank registration and bank functional API to use for the consent.
    /// </summary>
    public class AccountAccessConsentRequest : ConsentRequestBase, ISupportsValidation
    {
        /// <summary>
        ///     BankProfile used to apply transformations to external API requests.
        /// </summary>
        public BankProfileEnum? BankProfile { get; set; }

        /// <summary>
        ///     Specifies BankRegistration object to use when creating the consent.
        ///     Both AccountAndTransactionApiId and BankRegistrationId properties must refer
        ///     to objects with the same parent Bank object.
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.Always)]
        public Guid BankRegistrationId { get; set; }

        /// <summary>
        ///     Specifies AccountAndTransactionApi object (bank functional API info) to use when creating the consent.
        ///     Both AccountAndTransactionApiId and BankRegistrationId properties must refer
        ///     to objects with the same parent Bank object.
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.Always)]
        public Guid AccountAndTransactionApiId { get; set; }

        /// <summary>
        ///     Use external API request object created from template.
        ///     The first non-null of ExternalApiObject, ExternalApiRequest, and TemplateRequest (in that order) is used
        ///     and the others are ignored. At least one of these three must be non-null.
        ///     Specifies template used to create external API request object.
        /// </summary>
        public TemplateRequest? TemplateRequest { get; set; }

        /// <summary>
        ///     Use directly-specified external API request object.
        ///     The first non-null of ExternalApiObject, ExternalApiRequest, and TemplateRequest (in that order) is used
        ///     and the others are ignored. At least one of these three must be non-null.
        ///     Specifies OBReadConsent1 from UK Open Banking Read-Write Account and Transaction API spec
        ///     <a
        ///         href="https://github.com/OpenBankingUK/read-write-api-specs/blob/v3.1.8r5/dist/openapi/account-info-openapi.yaml" />
        ///     v3.1.9r5 <a />. Open Banking Connector will automatically
        ///     translate <i>from</i> this to an older format for banks supporting an earlier spec version.
        /// </summary>
        public AccountAndTransactionModelsPublic.OBReadConsent1? ExternalApiRequest { get; set; }

        public async Task<ValidationResult> ValidateAsync() =>
            await new AccountAccessConsentValidator()
                .ValidateAsync(this)!;
    }
}
