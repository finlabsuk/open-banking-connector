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

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for BankProfile.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal class BankApiInformation : IEntityWithPublicInterface<BankApiInformation, Public.Request.BankApiInformation
            , BankApiInformationResponse,
            IBankApiInformationPublicQuery>,
        IBankApiInformationPublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access customisation points defined by IEntityWithPublicInterface in
        ///     generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public BankApiInformation() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        public BankApiInformation(
            ITimeProvider timeProvider,
            PaymentInitiationApi? paymentInitiationApi,
            Guid bankId,
            string? createdBy)
        {
            Created = timeProvider.GetUtcNow();
            CreatedBy = createdBy;
            IsDeleted = new ReadWriteProperty<bool>(data: false, timeProvider: timeProvider, modifiedBy: CreatedBy);
            PaymentInitiationApi = paymentInitiationApi;
            Id = Guid.NewGuid();
            BankId = bankId;
        }

        public Guid BankId { get; set; }

        public PaymentInitiationApi? PaymentInitiationApi { get; set; }
        public Guid Id { get; set; }
        public ReadWriteProperty<bool> IsDeleted { get; set; } = null!;
        public DateTimeOffset Created { get; }
        public string? CreatedBy { get; }

        public Func<Public.Request.BankApiInformation, ValidationResult> ValidatePublicRequestWrapper =>
            ValidatePublicRequest;

        public BankApiInformationResponse PublicResponse => new BankApiInformationResponse(
            paymentInitiationApi: PaymentInitiationApi,
            id: Id,
            bankId: BankId);

        public Func<ISharedContext, Public.Request.BankApiInformation, string?, Task<BankApiInformationResponse>>
            PostEntityAsyncWrapper =>
            PostEntityAsync;

        public Task BankApiDeleteAsync()
        {
            throw new InvalidOperationException("This class is local to OBC and cannot be deleted at bank API.");
        }

        public Task<BankApiInformationResponse> BankApiGetAsync(ITimeProvider timeProvider, string? modifiedBy)
        {
            throw new InvalidOperationException("This class is local to OBC and cannot be gotten from bank API.");
        }

        public static ValidationResult ValidatePublicRequest(Public.Request.BankApiInformation request)
        {
            return new BankApiInformationValidator()
                .Validate(request);
        }

        public static async Task<BankApiInformationResponse> PostEntityAsync(
            ISharedContext context,
            Public.Request.BankApiInformation request,
            string? createdBy)
        {
            CreateBankProfile i = new CreateBankProfile(
                bankProfileRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<BankApiInformation>(),
                bankRegistrationRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<BankRegistration>(),
                bankRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<Bank>(),
                dbMultiEntityMethods: context.DbContextService,
                timeProvider: context.TimeProvider);

            BankApiInformationResponse resp = await i.CreateAsync(request: request, createdBy: createdBy);
            return resp;
        }
    }
}
