// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        DomesticPaymentConsentAuthContext : LocalEntityPost<
            DomesticPaymentConsentAuthContextPersisted,
            DomesticPaymentConsentAuthContextRequest,
            DomesticPaymentConsentAuthContextPostResponse>
    {
        public DomesticPaymentConsentAuthContext(
            IDbReadWriteEntityMethods<DomesticPaymentConsentAuthContextPersisted>
                entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticPaymentConsentPersisted> domesticPaymentConsentMethods,
            IReadOnlyRepository<ProcessedSoftwareStatementProfile> softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            domesticPaymentConsentMethods,
            softwareStatementProfileRepo,
            instrumentationClient) { }

        protected override async Task<DomesticPaymentConsentAuthContextPostResponse> CreateResponse(
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
                    domesticPaymentConsent.BankRegistrationNavigation.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException(
                    $"No record found for SoftwareStatementProfile with ID {domesticPaymentConsent.BankRegistrationNavigation.SoftwareStatementProfileId}.");

            // Create auth URL
            var state = persistedObject.Id.ToString();
            string authUrl = CreateAuthUrl.Create(
                domesticPaymentConsent.BankApiResponse.Data.Data.ConsentId,
                processedSoftwareStatementProfile,
                domesticPaymentConsent.BankRegistrationNavigation,
                domesticPaymentConsent.BankRegistrationNavigation.BankNavigation.IssuerUrl,
                state,
                _instrumentationClient);
            DomesticPaymentConsentAuthContextPostResponse response =
                persistedObject.PublicPostResponseCustomised(authUrl);

            return response;
        }
    }
}
