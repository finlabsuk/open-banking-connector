// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using BankApiInformationRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankApiInformation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for BankProfile.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    internal partial class BankApiInformation :
        EntityBase,
        ISupportsFluentDeleteLocal<BankApiInformation>,
        IBankApiInformationPublicQuery
    {
        [ForeignKey("BankId")]
        public Bank BankNavigation { get; set; } = null!;

        public List<DomesticPaymentConsent> DomesticPaymentConsentsNavigation { get; set; } = null!;

        public PaymentInitiationApi? PaymentInitiationApi { get; set; }

        public Guid BankId { get; set; }
    }

    internal partial class BankApiInformation :
        ISupportsFluentLocalEntityPost<BankApiInformationRequest, BankApiInformationResponse>
    {
        public BankApiInformationResponse PublicGetResponse => new BankApiInformationResponse(
            Id,
            Name,
            Created,
            CreatedBy,
            PaymentInitiationApi,
            BankId);

        public void Initialise(
            BankApiInformationRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            base.Initialise(Guid.NewGuid(), request.Name, createdBy, timeProvider);
            PaymentInitiationApi = request.PaymentInitiationApi;
            BankId = request.BankId;
        }

        public BankApiInformationResponse PublicPostResponse => PublicGetResponse;
    }

    internal partial class BankApiInformation :
        ISupportsFluentLocalEntityGet<BankApiInformationResponse> { }
}
