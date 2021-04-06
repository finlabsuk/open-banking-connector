// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Repository.SoftwareStatementProfile;
using DomesticPaymentConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.
    DomesticPaymentConsentAuthContext;


namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class PostDomesticPaymentConsentAuthContext
    {
        private readonly IDbReadWriteEntityMethods<DomesticPaymentConsentAuthContext> _authContextRepo;
        private readonly IDbReadOnlyEntityMethods<BankApiInformation> _bankApiInformationRepo;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationRepo;
        private readonly IDbReadOnlyEntityMethods<Bank> _bankRepo;
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly IDbReadOnlyEntityMethods<DomesticPaymentConsent> _domesticConsentRepo;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IReadOnlyRepository<SoftwareStatementProfileCached> _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public PostDomesticPaymentConsentAuthContext(
            IReadOnlyRepository<SoftwareStatementProfileCached> softwareStatementProfileRepo,
            IDbReadOnlyEntityMethods<Bank> bankRepo,
            IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationRepo,
            IDbReadOnlyEntityMethods<BankApiInformation> bankApiInformationRepo,
            IDbReadOnlyEntityMethods<DomesticPaymentConsent> domesticConsentRepo,
            IDbReadWriteEntityMethods<DomesticPaymentConsentAuthContext> authContextRepo,
            IDbSaveChangesMethod dbSaveChangesMethod,
            IInstrumentationClient instrumentationClient,
            ITimeProvider timeProvider)
        {
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _bankRepo = bankRepo;
            _bankRegistrationRepo = bankRegistrationRepo;
            _bankApiInformationRepo = bankApiInformationRepo;
            _domesticConsentRepo = domesticConsentRepo;
            _authContextRepo = authContextRepo;
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _instrumentationClient = instrumentationClient;
            _timeProvider = timeProvider;
        }

        // Load bank and bankProfile
        public async Task<(DomesticPaymentConsentAuthContextPostResponse response,
                IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            PostAsync(
                DomesticPaymentConsentAuthContextRequest request,
                string? createdBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data objects
            DomesticPaymentConsent domesticPaymentConsent =
                await _domesticConsentRepo.GetNoTrackingAsync(request.DomesticPaymentConsentId)
                ?? throw new KeyNotFoundException(
                    $"No record found for Domestic Payment Consent with ID {request.DomesticPaymentConsentId}.");
            BankApiInformation bankApiInformation =
                await _bankApiInformationRepo.GetNoTrackingAsync(domesticPaymentConsent.BankApiInformationId)
                ?? throw new KeyNotFoundException(
                    $"No record found for Bank API Information with ID {domesticPaymentConsent.BankApiInformationId}.");
            BankRegistration bankRegistration =
                await _bankRegistrationRepo.GetNoTrackingAsync(domesticPaymentConsent.BankRegistrationId)
                ?? throw new KeyNotFoundException(
                    $"No record found for Bank Registration with ID {domesticPaymentConsent.BankRegistrationId}.");

            // Checks
            if (bankRegistration.BankId != bankApiInformation.BankId)
            {
                throw new ArgumentException(
                    "Bank Registration and Bank API Information records are not associated with the same Bank");
            }

            Guid bankId = bankApiInformation.BankId;

            // Load relevant data objects
            Bank bank = await _bankRepo.GetNoTrackingAsync(bankId)
                        ?? throw new KeyNotFoundException($"No record found for Bank with ID {bankId}.");
            SoftwareStatementProfileCached softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankRegistration.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException(
                    $"No record found for SoftwareStatementProfile with ID {bankRegistration.SoftwareStatementProfileId}.");

            // Create auth context
            var persistedAuthContext = new DomesticPaymentConsentAuthContext(
                request.DomesticPaymentConsentId,
                Guid.NewGuid(),
                request.Name,
                createdBy,
                _timeProvider);

            // Create auth URL
            var state = persistedAuthContext.Id.ToString();
            JwtFactory jwtFactory = new JwtFactory();
            string authUrl = CreateAuthUrl.Create(
                domesticPaymentConsent.OBWriteDomesticConsentResponse.Data.ConsentId,
                softwareStatementProfile,
                bankRegistration,
                bank.IssuerUrl,
                state,
                jwtFactory,
                _instrumentationClient);

            // Store auth context
            await _authContextRepo.AddAsync(persistedAuthContext);
            await _dbSaveChangesMethod.SaveChangesAsync();

            DomesticPaymentConsentAuthContextPostResponse response = persistedAuthContext.PublicPostResponse(authUrl);

            return (response, nonErrorMessages);
        }
    }
}
