// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FluentValidation.Results;
using BankRegistrationRequest = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankRegistration;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal class BankRegistration : IEntityWithPublicInterface<BankRegistration, BankRegistrationRequest,
            BankRegistrationResponse,
            IBankRegistrationPublicQuery>,
        IBankRegistrationPublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access static methods in generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public BankRegistration() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        public BankRegistration(
            ITimeProvider timeProvider,
            string softwareStatementProfileId,
            OpenIdConfiguration openIdConfiguration,
            OBClientRegistration1 bankClientRegistration,
            string bankId,
            OBClientRegistration1 bankClientRegistrationRequest,
            string? createdBy)
        {
            Created = timeProvider.GetUtcNow();
            CreatedBy = createdBy;
            IsDeleted = new ReadWriteProperty<bool>(data: false, timeProvider: timeProvider, modifiedBy: CreatedBy);
            SoftwareStatementProfileId = softwareStatementProfileId;
            OpenIdConfiguration = openIdConfiguration;
            BankClientRegistrationRequest = bankClientRegistrationRequest;
            BankClientRegistration = bankClientRegistration;
            Id = Guid.NewGuid().ToString();
            BankId = bankId;
        }

        public string SoftwareStatementProfileId { get; set; } = null!;

        public OpenIdConfiguration OpenIdConfiguration { get; set; } = null!;

        public OBClientRegistration1 BankClientRegistration { get; set; } = null!;

        /// <summary>
        ///     Bank this registration is with.
        /// </summary>
        public string BankId { get; set; } = null!;

        // TODO: Add MTLS configuration

        public OBClientRegistration1 BankClientRegistrationRequest { get; set; } = null!;

        public string Id { get; set; } = null!;

        public ReadWriteProperty<bool> IsDeleted { get; set; } = null!;
        public DateTimeOffset Created { get; }
        public string? CreatedBy { get; }
        public Func<BankRegistrationRequest, ValidationResult> ValidatePublicRequestWrapper => ValidatePublicRequest;

        public BankRegistrationResponse PublicResponse =>
            new BankRegistrationResponse(id: Id, bankClientRegistrationRequest: BankClientRegistrationRequest);

        public Func<ISharedContext, BankRegistrationRequest, string?, Task<BankRegistrationResponse>>
            PostEntityAsyncWrapper =>
            PostEntityAsync;

        public Task BankApiDeleteAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BankRegistrationResponse> BankApiGetAsync(ITimeProvider timeProvider, string? modifiedBy)
        {
            throw new NotImplementedException();
        }

        public static ValidationResult ValidatePublicRequest(BankRegistrationRequest request)
        {
            return new BankRegistrationValidator()
                .Validate(request);
        }

        public static async Task<BankRegistrationResponse> PostEntityAsync(
            ISharedContext context,
            BankRegistrationRequest request,
            string? createdBy)
        {
            CreateBankRegistration i = new CreateBankRegistration(
                apiClient: context.ApiClient,
                bankRegistrationRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<BankRegistration>(),
                bankRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<Bank>(),
                dbMultiEntityMethods: context.DbContextService,
                softwareStatementProfileService: context.SoftwareStatementProfileService,
                timeProvider: context.TimeProvider);

            BankRegistrationResponse resp = await i.CreateAsync(
                requestBankRegistration: request,
                createdBy: createdBy);
            return resp;
        }
    }
}
