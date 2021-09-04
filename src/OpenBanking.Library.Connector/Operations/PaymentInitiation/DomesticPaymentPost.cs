// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;
using DomesticPaymentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPayment;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class
        DomesticPaymentPost : ReadWritePost<DomesticPaymentPersisted, DomesticPaymentRequest,
            DomesticPaymentResponse,
            PaymentInitiationModelsPublic.OBWriteDomestic2, PaymentInitiationModelsPublic.OBWriteDomesticResponse5>
    {
        public DomesticPaymentPost(
            IDbReadWriteEntityMethods<DomesticPaymentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticPaymentConsent>
                domesticPaymentConsentMethods,
            IReadOnlyRepository<SoftwareStatementProfile> softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            domesticPaymentConsentMethods,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper) { }

        protected override string RelativePath => "/domestic-payments";

        protected override async Task<(PaymentInitiationModelsPublic.OBWriteDomestic2 apiRequest, BankApiInformation
            bankApiInformation, BankRegistration bankRegistration, string bankFinancialId,
            TokenEndpointResponse? userTokenEndpointResponse, List<IFluentResponseInfoOrWarningMessage> nonErrorMessages
            )> ApiPostRequestData(DomesticPaymentRequest request)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data
            Guid domesticPaymentConsentId = request.DomesticPaymentConsentId;
            DomesticPaymentConsent domesticPaymentConsent =
                await _domesticPaymentConsentMethods
                    .DbSetNoTracking
                    .Include(o => o.DomesticPaymentConsentAuthContextsNavigation)
                    .Include(o => o.BankApiInformationNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == domesticPaymentConsentId) ??
                throw new KeyNotFoundException(
                    $"No record found for Domestic Payment Consent with ID {domesticPaymentConsentId}.");
            BankApiInformation bankApiInformation = domesticPaymentConsent.BankApiInformationNavigation;
            BankRegistration bankRegistration = domesticPaymentConsent.BankRegistrationNavigation;
            string bankFinancialId = domesticPaymentConsent.BankRegistrationNavigation.BankNavigation.FinancialId;

            // Create request
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4
                consentRequest = domesticPaymentConsent.BankApiRequest;
            _mapper.Map(
                consentRequest.Data.Initiation,
                out PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiation initiation);
            PaymentInitiationModelsPublic.OBWriteDomestic2 apiRequest =
                new PaymentInitiationModelsPublic.OBWriteDomestic2
                {
                    Data = new PaymentInitiationModelsPublic.OBWriteDomestic2Data
                    {
                        ConsentId = domesticPaymentConsent.BankApiId,
                        Initiation = initiation
                    },
                    Risk = consentRequest.Risk
                };

            // Get token
            List<DomesticPaymentConsentAuthContextPersisted> authContextsWithToken =
                domesticPaymentConsent.DomesticPaymentConsentAuthContextsNavigation
                    .Where(x => x.TokenEndpointResponse.Data != null)
                    .ToList();

            TokenEndpointResponse userTokenEndpointResponse =
                authContextsWithToken.Any()
                    ? authContextsWithToken
                        .OrderByDescending(x => x.TokenEndpointResponse.Modified)
                        .Select(x => x.TokenEndpointResponse.Data)
                        .First()! // We already filtered out null entries above
                    : throw new InvalidOperationException("No token is available for Domestic Payment Consent.");

            return (apiRequest, bankApiInformation, bankRegistration, bankFinancialId,
                userTokenEndpointResponse, nonErrorMessages);
        }
    }
}