// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.AccountAndTransaction;
using Newtonsoft.Json;
using AccountAndTransactionModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request
{
    /// <summary>
    ///     Request object used when creating an AccountAccessConsent object. Includes a UK Open Banking request object
    ///     plus information on the bank registration and bank functional API to associate with the consent.
    /// </summary>
    public class AccountAccessConsent : ConsentRequestBase, ISupportsValidation
    {
        /// <summary>
        ///     BankProfile used to supply default values for unspecified properties and apply transformations to external API
        ///     requests.
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
        ///     Request object OBReadConsent1 from UK Open Banking Read-Write Account and Transaction API spec
        ///     <a
        ///         href="https://github.com/OpenBankingUK/read-write-api-specs/blob/v3.1.8r5/dist/openapi/account-info-openapi.yaml" />
        ///     v3.1.9r5 <a />. Open Banking Connector will automatically
        ///     translate <i>from</i> this to an older format for banks supporting an earlier spec version.
        /// </summary>
        public AccountAndTransactionModelsPublic.OBReadConsent1? ExternalApiRequest { get; set; } = null!;

        public async Task<ValidationResult> ValidateAsync() =>
            await new AccountAccessConsentValidator()
                .ValidateAsync(this)!;
    }
}
