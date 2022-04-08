// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
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
        DomesticPaymentConsentAuthContextPost : LocalEntityPostBase<
            DomesticPaymentConsentAuthContextPersisted,
            DomesticPaymentConsentAuthContextRequest,
            DomesticPaymentConsentAuthContextCreateLocalResponse>
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

        protected override DomesticPaymentConsentAuthContextPersisted Create(
            DomesticPaymentConsentAuthContextRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            var output = new DomesticPaymentConsentAuthContextPersisted(
                Guid.NewGuid(),
                request.Name,
                createdBy,
                timeProvider,
                null,
                0,
                null,
                request.DomesticPaymentConsentId);

            return output;
        }

        protected override async Task<DomesticPaymentConsentAuthContextCreateLocalResponse> CreateResponse(
            DomesticPaymentConsentAuthContextPersisted persistedObject)
        {
            // Load relevant data objects
            DomesticPaymentConsentPersisted domesticPaymentConsent =
                _domesticPaymentConsentMethods
                    .DbSetNoTracking
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefault(x => x.Id == persistedObject.DomesticPaymentConsentId) ??
                throw new KeyNotFoundException(
                    $"No record found for Domestic Payment Consent with ID {persistedObject.DomesticPaymentConsentId}.");

            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    domesticPaymentConsent.BankRegistrationNavigation.SoftwareStatementProfileId,
                    domesticPaymentConsent.BankRegistrationNavigation
                        .SoftwareStatementAndCertificateProfileOverrideCase);

            // Create auth URL
            var state = persistedObject.Id.ToString();
            string authUrl = CreateAuthUrl.Create(
                domesticPaymentConsent.ExternalApiId,
                processedSoftwareStatementProfile,
                domesticPaymentConsent.BankRegistrationNavigation,
                domesticPaymentConsent.BankRegistrationNavigation.BankNavigation.IssuerUrl,
                state,
                "payments",
                _instrumentationClient);
            var response =
                new DomesticPaymentConsentAuthContextCreateLocalResponse(
                    persistedObject.Id,
                    persistedObject.Name,
                    persistedObject.Created,
                    persistedObject.CreatedBy,
                    persistedObject.DomesticPaymentConsentId,
                    authUrl);

            return response;
        }
    }
}
