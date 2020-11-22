// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using OBClientRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models.OBClientRegistration1;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class BankRegistration
    {
        /// <summary>
        ///     ID of software statement profile used to create bank registration. Only
        ///     IDs which have been specified at OBC startup via <see cref="ObcConfiguration.SoftwareStatementProfileIds" />
        ///     will be accepted.
        /// </summary>
        public string SoftwareStatementProfileId { get; set; } = null!;

        /// <summary>
        ///     Allows to use existing bank registration instead of creating new one. Response
        ///     should be of type <see cref="OBClientRegistration" /> but is supplied as JSON text file
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
        ///     Bank for which this registration is to be created.
        /// </summary>
        public Guid BankId { get; set; }

        /// <summary>
        ///     If registration is successfully created, replace <see cref="Persistent.Bank.DefaultBankRegistrationId" />
        ///     with reference to this registration.
        /// </summary>
        public bool ReplaceDefaultBankRegistration { get; set; } = false;

        /// <summary>
        ///     If registration is successfully created, replace <see cref="Persistent.Bank.StagingBankRegistrationId" />
        ///     with reference to this registration.
        /// </summary>
        public bool ReplaceStagingBankRegistration { get; set; } = false;

        /// <summary>
        ///     If registration already exists for bank, allow creation of additional one. NB this may
        ///     disrupt existing registration depending on bank support for multiple registrations.
        /// </summary>
        public bool AllowMultipleRegistrations { get; set; } = false;
    }
}
