// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;

/// <summary>
///     Persisted type for Bank API Set
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class PaymentInitiationApiEntity :
    BaseEntity
{
    public PaymentInitiationApiEntity(
        string? reference,
        Guid id,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        Guid bankId,
        PaymentInitiationApiVersion apiVersion,
        string baseUrl) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy)
    {
        BankId = bankId;
        ApiVersion = apiVersion;
        BaseUrl = baseUrl;
    }

    [ForeignKey("BankId")]
    public Bank BankNavigation { get; set; } = null!;

    public Guid BankId { get; }
    public PaymentInitiationApiVersion ApiVersion { get; }

    public string BaseUrl { get; }
}
