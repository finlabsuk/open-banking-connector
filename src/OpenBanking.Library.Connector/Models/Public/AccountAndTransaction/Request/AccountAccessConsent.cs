// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using AccountAndTransactionModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request
{
    public class AccountAccessConsent : Base
    {
        /// <summary>
        ///     Request object from UK Open Banking spec v3.1.9. Open Banking Connector can be configured
        ///     to translate from this for banks supporting an earlier spec version.
        /// </summary>
        public AccountAndTransactionModelsPublic.OBReadConsent1 OBReadConsent { get; set; } = null!;

        /// <summary>
        ///     Specifies Bank API Set to use when creating consent.
        ///     Both <see cref="BankApiSetId" /> and <see cref="BankRegistrationId" /> must point to objects with the same parent
        ///     (Bank ID).
        /// </summary>
        public Guid BankApiSetId { get; set; }

        /// <summary>
        ///     Specifies Bank Registration to use when creating consent.
        ///     Both <see cref="BankApiSetId" /> and <see cref="BankRegistrationId" /> must point to objects with the same parent
        ///     (Bank ID).
        /// </summary>
        public Guid BankRegistrationId { get; set; }
    }
}
