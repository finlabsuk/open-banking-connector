// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for BankRegistration.
    ///     Can't make internal due to use in OpenBanking.Library.Connector.GenericHost but at least
    ///     constructors can be internal.
    /// </summary>
    public class BankRegistration : IEntity
    {
        internal BankRegistration() { }

        internal BankRegistration(
            string softwareStatementProfileId,
            OpenIdConfiguration openIdConfiguration,
            OBClientRegistration1 bankClientRegistrationRequestData,
            OBClientRegistration1 bankClientRegistrationData,
            string bankId)
        {
            SoftwareStatementProfileId = softwareStatementProfileId;
            OpenIdConfiguration = openIdConfiguration;
            BankClientRegistrationRequestData = bankClientRegistrationRequestData;
            BankClientRegistrationData = bankClientRegistrationData;
            Id = Guid.NewGuid().ToString();
            BankId = bankId;
        }

        public string SoftwareStatementProfileId { get; set; } = null!;

        public OpenIdConfiguration OpenIdConfiguration { get; set; } = null!;

        // TODO: Add MTLS configuration

        public OBClientRegistration1 BankClientRegistrationRequestData { get; set; } = null!;

        public OBClientRegistration1 BankClientRegistrationData { get; set; } = null!;

        /// <summary>
        ///     Bank this registration is with.
        /// </summary>
        public string BankId { get; set; } = null!;

        public string Id { get; set; } = null!;
    }
}
