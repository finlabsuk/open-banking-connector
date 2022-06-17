// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using DomesticPaymentConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.
    DomesticPaymentConsentAuthContext;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class
        DomesticPaymentConsentAuthContextPost : LocalEntityCreate<
            DomesticPaymentConsentAuthContextPersisted,
            DomesticPaymentConsentAuthContextRequest,
            DomesticPaymentConsentAuthContextCreateResponse>
    {
        protected readonly IDbReadOnlyEntityMethods<DomesticPaymentConsentPersisted> _domesticPaymentConsentMethods;

        public DomesticPaymentConsentAuthContextPost(
            IDbReadWriteEntityMethods<DomesticPaymentConsentAuthContextPersisted>
                entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticPaymentConsentPersisted> domesticPaymentConsentMethods,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient)
        {
            _domesticPaymentConsentMethods = domesticPaymentConsentMethods;
        }

        protected override async Task<DomesticPaymentConsentAuthContextCreateResponse> AddEntity(
            DomesticPaymentConsentAuthContextRequest request,
            ITimeProvider timeProvider)
        {
            // Create persisted entity
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var nonce = Guid.NewGuid().ToString();
            var entity = new DomesticPaymentConsentAuthContextPersisted(
                Guid.NewGuid(),
                request.Reference,
                false,
                utcNow,
                request.CreatedBy,
                utcNow,
                request.CreatedBy,
                nonce,
                request.DomesticPaymentConsentId);

            // Add entity
            await _entityMethods.AddAsync(entity);

            // Load relevant data objects
            DomesticPaymentConsentPersisted domesticPaymentConsent =
                _domesticPaymentConsentMethods
                    .DbSetNoTracking
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefault(x => x.Id == entity.DomesticPaymentConsentId) ??
                throw new KeyNotFoundException(
                    $"No record found for Domestic Payment Consent with ID {entity.DomesticPaymentConsentId}.");
            CustomBehaviourClass? customBehaviour =
                domesticPaymentConsent.BankRegistrationNavigation.BankNavigation.CustomBehaviour;
            string authorizationEndpoint =
                domesticPaymentConsent.BankRegistrationNavigation.BankNavigation.AuthorizationEndpoint;
            string? issuerUrl = domesticPaymentConsent.BankRegistrationNavigation.BankNavigation.IssuerUrl;
            bool supportsSca = domesticPaymentConsent.BankRegistrationNavigation.BankNavigation.SupportsSca;

            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    domesticPaymentConsent.BankRegistrationNavigation.SoftwareStatementProfileId,
                    domesticPaymentConsent.BankRegistrationNavigation
                        .SoftwareStatementProfileOverride);

            // Create auth URL
            var state = entity.Id.ToString();
            string consentAuthGetAudClaim =
                customBehaviour?.DomesticPaymentConsentAuthGet?.AudClaim ??
                issuerUrl ?? throw new ArgumentException("No Issuer URL or custom behaviour Aud claim specified.");

            string authUrl = CreateAuthUrl.Create(
                domesticPaymentConsent.ExternalApiId,
                processedSoftwareStatementProfile,
                domesticPaymentConsent.BankRegistrationNavigation.ExternalApiObject.ExternalApiId,
                customBehaviour?.DomesticPaymentConsentAuthGet,
                authorizationEndpoint,
                consentAuthGetAudClaim,
                supportsSca,
                state,
                nonce,
                "payments",
                _instrumentationClient);
            var response =
                new DomesticPaymentConsentAuthContextCreateResponse(
                    entity.Id,
                    entity.Created,
                    entity.CreatedBy,
                    entity.Reference,
                    entity.DomesticPaymentConsentId,
                    authUrl);

            return response;
        }
    }
}
