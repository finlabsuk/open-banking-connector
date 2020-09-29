// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FluentValidation.Results;
using DomesticPaymentConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPaymentConsent;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal class DomesticPaymentConsent : IEntityWithPublicInterface<DomesticPaymentConsent,
            DomesticPaymentConsentRequest,
            DomesticPaymentConsentResponse,
            IDomesticPaymentConsentPublicQuery>,
        IDomesticPaymentConsentPublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access static methods in generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public DomesticPaymentConsent() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        public DomesticPaymentConsent(
            ITimeProvider timeProvider,
            string state,
            string bankProfileId,
            OBWriteDomesticConsent4 obWriteDomesticConsent,
            OBWriteDomesticConsentResponse4 obWriteDomesticConsentResponse,
            string? createdBy)
        {
            Created = timeProvider.GetUtcNow();
            CreatedBy = createdBy;
            IsDeleted = new ReadWriteProperty<bool>(data: false, timeProvider: timeProvider, modifiedBy: CreatedBy);
            State = new ReadWriteProperty<string>(data: state, timeProvider: timeProvider, modifiedBy: createdBy);
            TokenEndpointResponse = new ReadWriteProperty<TokenEndpointResponse?>(
                data: new TokenEndpointResponse(),
                timeProvider: timeProvider,
                modifiedBy: createdBy);
            BankProfileId = bankProfileId;
            OBWriteDomesticConsent = obWriteDomesticConsent;
            OBWriteDomesticConsentResponse = obWriteDomesticConsentResponse;
            Id = Guid.NewGuid().ToString();
        }

        public ReadWriteProperty<string> State { get; set; } = null!;

        public string BankProfileId { get; set; } = null!;

        public OBWriteDomesticConsent4 OBWriteDomesticConsent { get; set; } = null!;

        public ReadWriteProperty<TokenEndpointResponse?> TokenEndpointResponse { get; set; } = null!;

        public string OBId => OBWriteDomesticConsentResponse.Data.ConsentId;

        public OBWriteDomesticConsentResponse4 OBWriteDomesticConsentResponse { get; set; } = null!;

        public string Id { get; set; } = null!;

        public ReadWriteProperty<bool> IsDeleted { get; set; } = null!;
        public DateTimeOffset Created { get; }
        public string? CreatedBy { get; }

        public Func<DomesticPaymentConsentRequest, ValidationResult> ValidatePublicRequestWrapper =>
            ValidatePublicRequest;

        public Func<ISharedContext, DomesticPaymentConsentRequest, string?, Task<DomesticPaymentConsentResponse>>
            PostEntityAsyncWrapper =>
            PostEntityAsync;

        public DomesticPaymentConsentResponse PublicResponse =>
            new DomesticPaymentConsentResponse(
                authUrl: null,
                id: Id,
                obWriteDomesticConsentResponse: OBWriteDomesticConsentResponse);

        public Task BankApiDeleteAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DomesticPaymentConsentResponse> BankApiGetAsync(
            ITimeProvider timeProvider,
            string? modifiedBy)
        {
            throw new NotImplementedException();
        }

        public static OBWriteDomesticConsent4 GetOBWriteDomesticConsent(DomesticPaymentConsentRequest request) =>
            new OBWriteDomesticConsent4
            {
                Data = new OBWriteDomesticConsent4Data
                {
                    ReadRefundAccount = null,
                    Initiation = new OBWriteDomestic2DataInitiation
                    {
                        InstructionIdentification = request.InstructionIdentification,
                        EndToEndIdentification = request.EndToEndIdentification,
                        LocalInstrument = null,
                        InstructedAmount = request.InstructedAmount,
                        DebtorAccount = null,
                        CreditorAccount = request.CreditorAccount,
                        CreditorPostalAddress = null,
                        RemittanceInformation = request.RemittanceInformation,
                        SupplementaryData = null
                    },
                    Authorisation = null,
                    SCASupportData = null
                },
                Risk = request.Merchant
            };

        public static ValidationResult ValidatePublicRequest(DomesticPaymentConsentRequest request)
        {
            return new DomesticPaymentConsentValidator()
                .Validate(request);
        }

        public static async Task<DomesticPaymentConsentResponse> PostEntityAsync(
            ISharedContext context,
            DomesticPaymentConsentRequest request,
            string? createdBy)
        {
            CreateDomesticPaymentConsent i = new CreateDomesticPaymentConsent(
                apiClient: context.ApiClient,
                bankProfileRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<BankProfile>(),
                bankClientProfileRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<BankRegistration>(),
                bankRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<Bank>(),
                dbMultiEntityMethods: context.DbContextService,
                domesticConsentRepo: context.DbEntityRepositoryFactory
                    .CreateDbEntityRepository<DomesticPaymentConsent>(),
                mapper: context.EntityMapper,
                softwareStatementProfileService: context.SoftwareStatementProfileService,
                timeProvider: context.TimeProvider);

            DomesticPaymentConsentResponse resp = await i.CreateAsync(
                request: request,
                createdBy: createdBy);
            return resp;
        }
    }
}
