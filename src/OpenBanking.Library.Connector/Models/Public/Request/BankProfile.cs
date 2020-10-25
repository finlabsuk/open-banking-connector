// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class BankProfile
    {
        /// <summary>
        ///     Bank for which this profile is to be created.
        /// </summary>
        public Guid BankId { get; set; }

        /// <summary>
        ///     If profile is successfully created, replace <see cref="Persistent.Bank.DefaultBankProfileId" />
        ///     with reference to this profile.
        /// </summary>
        public bool ReplaceDefaultBankProfile { get; set; } = false;

        /// <summary>
        ///     If profile is successfully created, replace <see cref="Persistent.Bank.StagingBankProfileId" />
        ///     with reference to this profile.
        /// </summary>
        public bool ReplaceStagingBankProfile { get; set; } = false;

        /// <summary>
        ///     Specifies UK Open Banking Payment Initiation API associated with profile.
        ///     Null means profile is not used with such an API.
        /// </summary>
        public PaymentInitiationApi? PaymentInitiationApi { get; set; }
    }
}
