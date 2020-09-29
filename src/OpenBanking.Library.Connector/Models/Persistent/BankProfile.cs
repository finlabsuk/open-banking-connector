// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FluentValidation.Results;
using BankProfileRequest = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for BankProfile.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal class BankProfile : IEntityWithPublicInterface<BankProfile, BankProfileRequest, BankProfileResponse,
            IBankProfilePublicQuery>,
        IBankProfilePublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access customisation points defined by IEntityWithPublicInterface in
        ///     generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public BankProfile() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        public BankProfile(
            ITimeProvider timeProvider,
            string bankRegistrationId,
            PaymentInitiationApi? paymentInitiationApi,
            string bankId,
            string? createdBy)
        {
            Created = timeProvider.GetUtcNow();
            CreatedBy = createdBy;
            IsDeleted = new ReadWriteProperty<bool>(data: false, timeProvider: timeProvider, modifiedBy: CreatedBy);
            BankRegistrationId = bankRegistrationId;
            PaymentInitiationApi = paymentInitiationApi;
            Id = Guid.NewGuid().ToString();
            BankId = bankId;
        }

        public string BankId { get; set; } = null!;

        public string BankRegistrationId { get; set; } = null!;
        public PaymentInitiationApi? PaymentInitiationApi { get; set; }
        public string Id { get; set; } = null!;
        public ReadWriteProperty<bool> IsDeleted { get; set; } = null!;
        public DateTimeOffset Created { get; }
        public string? CreatedBy { get; }
        public Func<BankProfileRequest, ValidationResult> ValidatePublicRequestWrapper => ValidatePublicRequest;

        public BankProfileResponse PublicResponse => new BankProfileResponse(
            bankRegistrationId: Id,
            paymentInitiationApi: PaymentInitiationApi,
            id: Id);

        public Func<ISharedContext, BankProfileRequest, string?, Task<BankProfileResponse>> PostEntityAsyncWrapper =>
            PostEntityAsync;

        public Task BankApiDeleteAsync()
        {
            throw new InvalidOperationException("This class is local to OBC and cannot be deleted at bank API.");
        }

        public Task<BankProfileResponse> BankApiGetAsync(ITimeProvider timeProvider, string? modifiedBy)
        {
            throw new InvalidOperationException("This class is local to OBC and cannot be gotten from bank API.");
        }

        public static ValidationResult ValidatePublicRequest(BankProfileRequest request)
        {
            return new BankProfileValidator()
                .Validate(request);
        }

        public static async Task<BankProfileResponse> PostEntityAsync(
            ISharedContext context,
            BankProfileRequest request,
            string? createdBy)
        {
            CreateBankProfile i = new CreateBankProfile(
                bankProfileRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<BankProfile>(),
                bankRegistrationRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<BankRegistration>(),
                bankRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<Bank>(),
                dbMultiEntityMethods: context.DbContextService,
                timeProvider: context.TimeProvider);

            BankProfileResponse resp = await i.CreateAsync(requestBankProfile: request, createdBy: createdBy);
            return resp;
        }
    }
}
