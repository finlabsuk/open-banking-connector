// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class
        DomesticPaymentConsentGet : ReadWriteGet<DomesticPaymentConsentPersisted,
            IDomesticPaymentConsentPublicQuery,
            DomesticPaymentConsentResponse,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>
    {
        private readonly IDbReadOnlyEntityMethods<BankApiInformation> _bankApiInformationMethods;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationMethods;

        public DomesticPaymentConsentGet(
            IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticPaymentConsentPersisted>
                domesticPaymentConsentMethods,
            IReadOnlyRepository<SoftwareStatementProfile> softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IDbReadOnlyEntityMethods<BankApiInformation> bankApiInformationMethods,
            IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationMethods) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            domesticPaymentConsentMethods,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper)
        {
            _bankApiInformationMethods = bankApiInformationMethods;
            _bankRegistrationMethods = bankRegistrationMethods;
        }

        protected override string RelativePathBeforeId => "/domestic-payment-consents";

        protected override async Task<(string bankApiId, DomesticPaymentConsentPersisted
            persistedObject, BankApiInformation bankApiInformation, BankRegistration bankRegistration,
            string bankFinancialId, TokenEndpointResponse? userTokenEndpointResponse,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGetRequestData(Guid id)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            DomesticPaymentConsentPersisted persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(o => o.DomesticPaymentConsentAuthContextsNavigation)
                    .Include(o => o.BankApiInformationNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {id}.");
            BankApiInformation bankApiInformation = persistedObject.BankApiInformationNavigation;
            BankRegistration bankRegistration = persistedObject.BankRegistrationNavigation;
            string bankFinancialId = persistedObject.BankRegistrationNavigation.BankNavigation.FinancialId;

            string bankApiId = persistedObject.BankApiId;

            return (bankApiId, persistedObject, bankApiInformation, bankRegistration, bankFinancialId, null,
                nonErrorMessages);
        }
    }
}
