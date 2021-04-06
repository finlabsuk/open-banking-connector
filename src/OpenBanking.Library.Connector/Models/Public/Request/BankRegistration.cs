// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentValidation.Results;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class BankRegistration : ISupportsValidation
    {
        /// <summary>
        ///     Bank this registration is with.
        /// </summary>
        public Guid BankId { get; set; }

        /// <summary>
        ///     ID of software statement profile used to create bank registration. Only
        ///     IDs which have been specified at OBC startup via
        ///     <see cref="OpenBankingConnectorSettings.SoftwareStatementProfileIds" />
        ///     will be accepted.
        /// </summary>
        public string SoftwareStatementProfileId { get; set; } = null!;

        /// <summary>
        ///     Functional APIs used for bank registration.
        ///     If not supplied, registration scope implied by software statement profile will be used.
        /// </summary>
        public RegistrationScope? RegistrationScope { get; set; }

        /// <summary>
        ///     Version of Open Banking Dynamic Client Registration API to use
        ///     for bank registration.
        /// </summary>
        public ClientRegistrationApiVersion ClientRegistrationApi { get; set; }

        /// <summary>
        ///     Allows to use existing bank registration instead of creating new one. Response
        ///     should be of type <see cref="ClientRegistrationModelsPublic.OBClientRegistration1" /> but is supplied as JSON text
        ///     file
        ///     so can be de-serialised and validated in same way as response from bank.
        ///     Settings in <see cref="BankRegistrationResponseJsonOptions" /> will be used.
        ///     This argument is intended for use in testing scenarios where you want to test
        ///     creation of a new registration but cannot re-POST one to bank for whatever reason.
        /// </summary>
        public string? BankRegistrationResponseFileName { get; set; }

        public OpenIdConfigurationOverrides? OpenIdConfigurationOverrides { get; set; }

        public HttpMtlsConfigurationOverrides? HttpMtlsConfigurationOverrides { get; set; }

        public BankRegistrationClaimsOverrides? BankRegistrationClaimsOverrides { get; set; }

        public BankRegistrationResponseJsonOptions? BankRegistrationResponseJsonOptions { get; set; }

        public BankRegistrationResponseOverrides? BankRegistrationResponseOverrides { get; set; }

        public OAuth2RequestObjectClaimsOverrides? OAuth2RequestObjectClaimsOverrides { get; set; }

        /// <summary>
        ///     Friendly name to support debugging etc. (must be unique i.e. not already in use).
        ///     This is optional.
        /// </summary>
        public string? Name { get; set; }

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
