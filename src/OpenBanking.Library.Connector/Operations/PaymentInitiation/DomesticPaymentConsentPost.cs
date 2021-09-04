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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
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
        DomesticPaymentConsentPost : ReadWritePost<DomesticPaymentConsentPersisted,
            DomesticPaymentConsent,
            DomesticPaymentConsentResponse,
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>
    {
        private readonly IDbReadOnlyEntityMethods<BankApiInformation> _bankApiInformationMethods;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationMethods;

        public DomesticPaymentConsentPost(
            IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticPaymentConsentPersisted>
                domesticPaymentConsentMethods,
            IReadOnlyRepository<ProcessedSoftwareStatementProfile> softwareStatementProfileRepo,
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

        protected override string RelativePath => "/domestic-payment-consents";

        protected override async
            Task<(PaymentInitiationModelsPublic.OBWriteDomesticConsent4 apiRequest, BankApiInformation
                bankApiInformation, BankRegistration bankRegistration, string bankFinancialId,
                TokenEndpointResponse? userTokenEndpointResponse, List<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)> ApiPostRequestData(DomesticPaymentConsent request)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data and checks
            Guid bankRegistrationId = request.BankRegistrationId;
            BankRegistration bankRegistration =
                await _bankRegistrationMethods
                    .DbSetNoTracking
                    .Include(o => o.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == bankRegistrationId) ??
                throw new KeyNotFoundException(
                    $"No record found for BankRegistrationId {bankRegistrationId} specified by request.");
            Guid bankApiInformationId = request.BankApiInformationId;
            BankApiInformation bankApiInformation =
                await _bankApiInformationMethods
                    .DbSetNoTracking
                    .SingleOrDefaultAsync(x => x.Id == bankApiInformationId) ??
                throw new KeyNotFoundException(
                    $"No record found for BankApiInformation {bankApiInformationId} specified by request.");
            if (bankApiInformation.BankId != bankRegistration.BankId)
            {
                throw new ArgumentException("BankRegistrationId and BankProfileId objects do not share same BankId.");
            }

            string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

            // Create request
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4 apiRequest = request.WriteDomesticConsent;

            return (apiRequest, bankApiInformation, bankRegistration, bankFinancialId, null,
                nonErrorMessages);
        }
    }
}
