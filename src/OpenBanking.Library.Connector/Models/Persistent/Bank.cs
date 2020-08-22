// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for Bank.
    ///     Can't make internal due to use in OpenBanking.Library.Connector.GenericHost but at least
    ///     constructors can be internal.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    public class Bank : IEntity
    {
        internal Bank() { }

        internal Bank(Public.Request.Bank bank)
        {
            IssuerUrl = bank.IssuerUrl;
            XFapiFinancialId = bank.XFapiFinancialId;
            Name = bank.Name;
            Id = Guid.NewGuid().ToString();
        }

        public string IssuerUrl { get; set; } = null!;

        public string XFapiFinancialId { get; set; } = null!;

        public string Name { get; set; } = null!;
        
        /// <summary>
        /// Specifies default Bank Registration associated with Bank.
        /// This may be used when creating a BankProfile.
        /// </summary>
        public string? DefaultBankRegistrationId { get; set; }

        /// <summary>
        /// Specifies staging Bank Registration associated with Bank.
        /// This may be used when creating a staging BankProfile.
        /// </summary>
        public string? StagingBankRegistrationId { get; set; }
        
        public string? DefaultBankProfileId { get; set; }

        public string? StagingBankProfileId { get; set; }
        
        public string Id { get; set; } = null!;
    }
}
