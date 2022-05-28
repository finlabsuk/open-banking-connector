// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request
{
    public class BankRegistration : Base, ISupportsValidation
    {
        public BankProfileEnum? BankProfile { get; set; }

        /// <summary>
        ///     Bank this registration is with.
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.Always)]
        public Guid BankId { get; set; }

        /// <summary>
        ///     ID of software statement profile used to create bank registration. Only
        ///     IDs which have been specified via configuration
        ///     will be accepted.
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.Always)]
        public string SoftwareStatementProfileId { get; set; } = null!;

        /// <summary>
        ///     Optional override case to use with software statement and certificate profiles. Override cases
        ///     can be used for bank-specific customisations to profiles, e.g. different transport certificate DN string.
        ///     When null no override case is specified.
        /// </summary>
        public string? SoftwareStatementAndCertificateProfileOverrideCase { get; set; }

        /// <summary>
        ///     Token endpoint authorisation method. Specify null for "most preferred" method to be selected based on
        ///     supported methods in Issuer URL OpenID Provider Configuration.
        /// </summary>
        public TokenEndpointAuthMethod? TokenEndpointAuthMethod { get; set; }


        /// <summary>
        ///     Functional APIs used for bank registration.
        ///     If not supplied, registration scope implied by software statement profile will be used.
        /// </summary>
        public RegistrationScopeEnum? RegistrationScope { get; set; }

        /// <summary>
        ///     Information about external BankRegistration (OAuth2 client) created using external (bank) API.
        ///     When non-null, this will be referenced
        ///     instead of
        ///     creating a new external BankRegistration object at the external API via DCR.
        /// </summary>
        public ExternalApiBankRegistration? ExternalApiObject { get; set; }

        /// <summary>
        ///     If registration already exists for bank, allow creation of additional one. NB this may
        ///     disrupt existing registration depending on bank support for multiple registrations.
        /// </summary>
        public bool AllowMultipleRegistrations { get; set; } = false;

        public async Task<ValidationResult> ValidateAsync() =>
            await new BankRegistrationValidator()
                .ValidateAsync(this)!;
    }
}
