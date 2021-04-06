// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using Bank = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Bank;
using BankApiInformation = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankApiInformation;
using BankRegistration = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankRegistration;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Repository.SoftwareStatementProfile;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class PostAuthorisationRedirectObject
    {
        private readonly IDbReadOnlyEntityMethods<BankApiInformation> _bankProfileRepo;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationRepo;
        private readonly IDbReadOnlyEntityMethods<Bank> _bankRepo;
        private readonly IDbSaveChangesMethod _dbContextService;
        private readonly IDbReadWriteEntityMethods<DomesticPaymentConsent> _domesticConsentRepo;
        private readonly IReadOnlyRepository<SoftwareStatementProfileCached> _softwareStatementProfileRepo;

        public PostAuthorisationRedirectObject(
            IDbReadOnlyEntityMethods<Bank> bankRepo,
            IDbReadOnlyEntityMethods<BankApiInformation> bankProfileRepo,
            IDbSaveChangesMethod dbContextService,
            IDbReadWriteEntityMethods<DomesticPaymentConsent> domesticConsentRepo,
            IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationRepo,
            IReadOnlyRepository<SoftwareStatementProfileCached> softwareStatementProfileRepo)
        {
            _bankRepo = bankRepo;
            _bankProfileRepo = bankProfileRepo;
            _dbContextService = dbContextService;
            _domesticConsentRepo = domesticConsentRepo;
            _bankRegistrationRepo = bankRegistrationRepo;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
        }

        public async Task<(AuthorisationRedirectObjectResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            PostAsync(
                AuthorisationRedirectObject request,
                string? createdBy)
        {
            request.ArgNotNull(nameof(request));

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data objects
            DomesticPaymentConsent paymentConsent =
                (await _domesticConsentRepo.GetAsync(dc => dc.State.Data == request.Response.State))
                .FirstOrDefault() ?? throw new KeyNotFoundException(
                    $"Consent with redirect state '{request.Response.State}' not found.");
            BankApiInformation bankApiInformation =
                await _bankProfileRepo.GetNoTrackingAsync(paymentConsent.BankApiInformationId) ??
                throw new KeyNotFoundException("API profile cannot be found.");
            BankRegistration bankRegistration =
                await _bankRegistrationRepo.GetNoTrackingAsync(paymentConsent.BankRegistrationId) ??
                throw new KeyNotFoundException("Bank client profile cannot be found.");
            Bank bank = await _bankRepo.GetNoTrackingAsync(bankApiInformation.BankId)
                        ?? throw new KeyNotFoundException("No record found for BankId in BankProfile.");
            SoftwareStatementProfileCached softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankRegistration.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException(
                    $"No record found for SoftwareStatementProfileId {bankRegistration.SoftwareStatementProfileId}");

            // Obtain token for consent
            string redirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
            JsonSerializerSettings? jsonSerializerSettings = null;
            TokenEndpointResponse tokenEndpointResponse =
                await PostTokenRequest.PostAuthCodeGrantAsync(
                    request.Response.Code,
                    redirectUrl,
                    bankRegistration,
                    jsonSerializerSettings,
                    softwareStatementProfile.ApiClient);

            // Update consent with token
            paymentConsent.TokenEndpointResponse = new ReadWriteProperty<TokenEndpointResponse?>(
                tokenEndpointResponse,
                new TimeProvider(),
                null);
            await _dbContextService.SaveChangesAsync();

            return (new AuthorisationRedirectObjectResponse(), nonErrorMessages);
        }
    }
}
