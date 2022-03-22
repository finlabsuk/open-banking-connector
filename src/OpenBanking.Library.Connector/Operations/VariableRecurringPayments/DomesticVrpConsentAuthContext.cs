// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
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
        DomesticVrpConsentAuthContext : LocalEntityPost<
            DomesticVrpConsentAuthContextPersisted,
            DomesticVrpConsentAuthContextRequest,
            DomesticVrpConsentAuthContextCreateLocalResponse>
    {
        protected readonly IDbReadOnlyEntityMethods<DomesticVrpConsentPersisted> _domesticPaymentConsentMethods;

        public DomesticVrpConsentAuthContext(
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

        protected override async Task<DomesticVrpConsentAuthContextCreateLocalResponse> CreateResponse(
            DomesticVrpConsentAuthContextPersisted persistedObject)
        {
            // Load relevant data objects
            DomesticVrpConsentPersisted domesticPaymentConsent =
                _domesticPaymentConsentMethods
                    .DbSetNoTracking
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefault(x => x.Id == persistedObject.DomesticVrpConsentId) ??
                throw new KeyNotFoundException(
                    $"No record found for Domestic Payment Consent with ID {persistedObject.DomesticVrpConsentId}.");

            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    domesticPaymentConsent.BankRegistrationNavigation.SoftwareStatementProfileId,
                    domesticPaymentConsent.BankRegistrationNavigation
                        .SoftwareStatementAndCertificateProfileOverrideCase);

            // Create auth URL
            var state = persistedObject.Id.ToString();
            string authUrl = CreateAuthUrl.Create(
                domesticPaymentConsent.BankApiResponse.Data.Data.ConsentId,
                processedSoftwareStatementProfile,
                domesticPaymentConsent.BankRegistrationNavigation,
                domesticPaymentConsent.BankRegistrationNavigation.BankNavigation.IssuerUrl,
                state,
                _instrumentationClient);
            DomesticVrpConsentAuthContextCreateLocalResponse response =
                persistedObject.PublicPostResponseCustomised(authUrl);

            return response;
        }
    }
}
