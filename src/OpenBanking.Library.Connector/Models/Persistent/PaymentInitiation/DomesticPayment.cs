// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FluentValidation.Results;
using OBWriteDomesticResponse =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.OBWriteDomesticResponse4;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal class DomesticPayment : IEntityWithPublicInterface<DomesticPayment,
            Public.PaymentInitiation.Request.DomesticPayment,
            DomesticPaymentResponse,
            IDomesticPaymentPublicQuery>,
        IDomesticPaymentPublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access static methods in generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public DomesticPayment() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        public DomesticPayment(
            ITimeProvider timeProvider,
            Guid domesticPaymentConsentId,
            OBWriteDomesticResponse obWriteDomesticResponse,
            string? createdBy)
        {
            Created = timeProvider.GetUtcNow();
            CreatedBy = createdBy;
            IsDeleted = new ReadWriteProperty<bool>(data: false, timeProvider: timeProvider, modifiedBy: CreatedBy);
            DomesticPaymentConsentId = domesticPaymentConsentId;
            OBWriteDomesticResponse = obWriteDomesticResponse;
            Id = Guid.NewGuid();
        }

        public Guid DomesticPaymentConsentId { get; set; }

        public OBWriteDomesticResponse OBWriteDomesticResponse { get; set; } = null!;

        public Guid Id { get; set; }

        public ReadWriteProperty<bool> IsDeleted { get; set; } = null!;
        public DateTimeOffset Created { get; }
        public string? CreatedBy { get; }

        public Func<Public.PaymentInitiation.Request.DomesticPayment, ValidationResult> ValidatePublicRequestWrapper =>
            ValidatePublicRequest;

        public Func<ISharedContext, Public.PaymentInitiation.Request.DomesticPayment, string?,
                Task<DomesticPaymentResponse>>
            PostEntityAsyncWrapper =>
            PostEntityAsync;

        public DomesticPaymentResponse PublicResponse =>
            new DomesticPaymentResponse(OBWriteDomesticResponse);

        public Task BankApiDeleteAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DomesticPaymentResponse> BankApiGetAsync(
            ITimeProvider timeProvider,
            string? modifiedBy)
        {
            throw new NotImplementedException();
        }

        public static ValidationResult ValidatePublicRequest(Public.PaymentInitiation.Request.DomesticPayment request)
        {
            return new PaymentRequestValidator()
                .Validate(request);
        }

        public static async Task<DomesticPaymentResponse> PostEntityAsync(
            ISharedContext context,
            Public.PaymentInitiation.Request.DomesticPayment request,
            string? createdBy)
        {
            CreateDomesticPayment i = new CreateDomesticPayment(
                bankRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<Bank>(),
                bankProfileRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<BankApiInformation>(),
                domesticConsentRepo: context.DbEntityRepositoryFactory
                    .CreateDbEntityRepository<DomesticPaymentConsent>(),
                mapper: context.EntityMapper,
                bankRegistrationRepo: context.DbEntityRepositoryFactory
                    .CreateDbEntityRepository<BankRegistration>(),
                domesticPaymentRepo: context.DbEntityRepositoryFactory
                    .CreateDbEntityRepository<DomesticPayment>(),
                timeProvider: context.TimeProvider,
                dbMultiEntityMethods: context.DbContextService,
                softwareStatementProfileRepo: context.SoftwareStatementProfileCachedRepo);

            DomesticPaymentResponse? resp = await i.CreateAsync(
                request: request,
                createdBy: createdBy);
            return resp;
        }
    }
}
