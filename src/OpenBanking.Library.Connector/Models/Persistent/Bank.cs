// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using BankRequest = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.Bank;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for Bank.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    internal class Bank : IEntityWithPublicInterface<Bank, BankRequest, BankResponse, IBankPublicQuery>,
        IBankPublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access static methods in generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public Bank() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        /// <param name="timeProvider"></param>
        /// <param name="requestBank"></param>
        /// <param name="createdBy"></param>
        public Bank(
            ITimeProvider timeProvider,
            string issuerUrl,
            string xFapiFinancialId,
            string name,
            string? createdBy)
        {
            Created = timeProvider.GetUtcNow();
            CreatedBy = createdBy;
            IsDeleted = new ReadWriteProperty<bool>(data: false, timeProvider: timeProvider, modifiedBy: CreatedBy);
            IssuerUrl = issuerUrl;
            XFapiFinancialId = xFapiFinancialId;
            Name = name;
            Id = Guid.NewGuid().ToString();
            DefaultBankRegistrationId = new ReadWriteProperty<string?>(
                data: null,
                timeProvider: timeProvider,
                modifiedBy: CreatedBy);
            DefaultBankProfileId = new ReadWriteProperty<string?>(
                data: null,
                timeProvider: timeProvider,
                modifiedBy: CreatedBy);
            StagingBankRegistrationId = new ReadWriteProperty<string?>(
                data: null,
                timeProvider: timeProvider,
                modifiedBy: CreatedBy);
            StagingBankProfileId = new ReadWriteProperty<string?>(
                data: null,
                timeProvider: timeProvider,
                modifiedBy: CreatedBy);
        }

        /// <summary>
        ///     Specifies default Bank Registration associated with Bank.
        ///     This may be used when creating a BankProfile.
        /// </summary>
        public ReadWriteProperty<string?> DefaultBankRegistrationId { get; set; } = null!;

        public ReadWriteProperty<string?> DefaultBankProfileId { get; set; } = null!;

        /// <summary>
        ///     Specifies staging Bank Registration associated with Bank.
        ///     This may be used when creating a staging BankProfile.
        /// </summary>
        public ReadWriteProperty<string?> StagingBankRegistrationId { get; set; } = null!;

        public ReadWriteProperty<string?> StagingBankProfileId { get; set; } = null!;

        public string IssuerUrl { get; } = null!;

        public string XFapiFinancialId { get; } = null!;

        public string Name { get; } = null!;

        public DateTimeOffset Created { get; }

        public string? CreatedBy { get; }

        public ReadWriteProperty<bool> IsDeleted { get; set; } = null!;

        public string Id { get; } = null!;

        public BankResponse PublicResponse => new BankResponse(
            issuerUrl: IssuerUrl,
            xFapiFinancialId: XFapiFinancialId,
            name: Name,
            id: Id);

        public Func<BankRequest, ValidationResult> ValidatePublicRequestWrapper => ValidatePublicRequest;

        public Func<ISharedContext, BankRequest, string?, Task<BankResponse>> PostEntityAsyncWrapper => PostEntityAsync;

        public Task BankApiDeleteAsync()
        {
            throw new InvalidOperationException("This class is local to OBC and cannot be deleted at bank API.");
        }

        public Task<BankResponse> BankApiGetAsync(ITimeProvider timeProvider, string? modifiedBy)
        {
            throw new InvalidOperationException("This class is local to OBC and cannot be gotten from bank API.");
        }

        public static ValidationResult ValidatePublicRequest(BankRequest requestBank)
        {
            return new BankValidator()
                .Validate(requestBank);
        }

        public static async Task<BankResponse> PostEntityAsync(
            ISharedContext context,
            BankRequest requestBank,
            string? createdBy)
        {
            CreateBank i = new CreateBank(
                bankRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<Bank>(),
                dbMultiEntityMethods: context.DbContextService,
                timeProvider: context.TimeProvider);

            BankResponse resp = await i.CreateAsync(requestBank: requestBank, createdBy: createdBy);
            return resp;
        }
    }
}
