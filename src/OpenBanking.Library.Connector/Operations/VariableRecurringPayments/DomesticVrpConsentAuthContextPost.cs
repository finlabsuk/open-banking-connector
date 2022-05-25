// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
    internal class
        DomesticVrpConsentAuthContextPost : LocalEntityPost<
            DomesticVrpConsentAuthContextPersisted,
            DomesticVrpConsentAuthContextRequest,
            DomesticVrpConsentAuthContextCreateLocalResponse>
    {
        protected readonly IDbReadOnlyEntityMethods<DomesticVrpConsentPersisted> _domesticPaymentConsentMethods;

        public DomesticVrpConsentAuthContextPost(
            IDbReadWriteEntityMethods<DomesticVrpConsentAuthContextPersisted>
                entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticVrpConsentPersisted> domesticPaymentConsentMethods,
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

        protected override async Task<DomesticVrpConsentAuthContextCreateLocalResponse> AddEntity(
            DomesticVrpConsentAuthContextRequest request,
            ITimeProvider timeProvider)
        {
            // Create persisted entity
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var entity = new DomesticVrpConsentAuthContextPersisted(
                Guid.NewGuid(),
                request.Reference,
                false,
                utcNow,
                request.CreatedBy,
                utcNow,
                request.CreatedBy,
                request.DomesticVrpConsentId);

            // Add entity
            await _entityMethods.AddAsync(entity);

            // Load relevant data objects
            DomesticVrpConsentPersisted domesticVrpConsent =
                _domesticPaymentConsentMethods
                    .DbSetNoTracking
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefault(x => x.Id == entity.DomesticVrpConsentId) ??
                throw new KeyNotFoundException(
                    $"No record found for Domestic Payment Consent with ID {entity.DomesticVrpConsentId}.");
            CustomBehaviourClass? customBehaviour =
                domesticVrpConsent.BankRegistrationNavigation.BankNavigation.CustomBehaviour;
            string authorizationEndpoint =
                domesticVrpConsent.BankRegistrationNavigation.BankNavigation.AuthorizationEndpoint;
            string? issuerUrl = domesticVrpConsent.BankRegistrationNavigation.BankNavigation.IssuerUrl;
            bool supportsSca = domesticVrpConsent.BankRegistrationNavigation.BankNavigation.SupportsSca;

            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    domesticVrpConsent.BankRegistrationNavigation.SoftwareStatementProfileId,
                    domesticVrpConsent.BankRegistrationNavigation
                        .SoftwareStatementProfileOverride);

            // Create auth URL
            var state = entity.Id.ToString();
            string consentAuthGetAudClaim =
                customBehaviour?.DomesticVrpConsentAuthGet?.AudClaim ??
                issuerUrl ?? throw new ArgumentException("No Issuer URL or custom behaviour Aud claim specified.");

            string authUrl = CreateAuthUrl.Create(
                domesticVrpConsent.ExternalApiId,
                processedSoftwareStatementProfile,
                domesticVrpConsent.BankRegistrationNavigation.ExternalApiObject.ExternalApiId,
                customBehaviour?.DomesticVrpConsentAuthGet,
                authorizationEndpoint,
                consentAuthGetAudClaim,
                supportsSca,
                state,
                "payments",
                _instrumentationClient);
            var response =
                new DomesticVrpConsentAuthContextCreateLocalResponse(
                    entity.Id,
                    entity.Created,
                    entity.CreatedBy,
                    entity.Reference,
                    entity.DomesticVrpConsentId,
                    authUrl);

            return response;
        }
    }
}
