// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class BankRegistration
    {
        [JsonProperty("softwareStatementProfileId")]
        public string SoftwareStatementProfileId { get; set; } = null!;

        [JsonProperty("openIdOverrides")]
        public OpenIdConfigurationOverrides OpenIdConfigurationOverrides { get; set; } = null!;

        [JsonProperty("httpMtlsOverrides")]
        public HttpMtlsConfigurationOverrides HttpMtlsConfigurationOverrides { get; set; } = null!;

        [JsonProperty("bankClientRegistrationClaimsOverrides")]
        public BankClientRegistrationClaimsOverrides? BankClientRegistrationClaimsOverrides { get; set; }

        [JsonProperty("registrationResponseOptions")]
        public RegistrationResponseJsonOptions? RegistrationResponseJsonOptions { get; set; }

        /// <summary>
        ///     Bank this registration is with.
        /// </summary>
        public string BankName { get; set; } = null!;

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
        /// If registration already exists for bank, allow creation of additional one. NB this may
        /// disrupt existing registration depending on bank support for multiple registrations.
        /// </summary>
        public bool AllowMultipleRegistrations { get; set; } = false;
    }
}
