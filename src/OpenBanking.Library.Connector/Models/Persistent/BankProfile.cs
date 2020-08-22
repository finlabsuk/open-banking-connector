// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for BankProfile.
    ///     Can't make internal due to use in OpenBanking.Library.Connector.GenericHost but at least
    ///     constructors can be internal.
    /// </summary>
    public class BankProfile : IEntity
    {
        internal BankProfile() { }

        internal BankProfile(
            string bankRegistrationId,
            PaymentInitiationApi? paymentInitiationApi,
            string bankId)
        {
            BankRegistrationId = bankRegistrationId;
            PaymentInitiationApi = paymentInitiationApi;
            Id = Guid.NewGuid().ToString();
            BankId = bankId;
        }

        public string BankRegistrationId { get; set; } = null!;
        public PaymentInitiationApi? PaymentInitiationApi { get; set; }

        public string BankId { get; set; } = null!;
        public string Id { get; set; } = null!;
    }
}
