// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Bank = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Bank;
using BankProfile = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankProfile;
using BankRegistration = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankRegistration;
using SoftwareStatementProfile =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class PostAuthorisationRedirectObject
    {
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<BankProfile> _bankProfileRepo;
        private readonly IDbEntityRepository<BankRegistration> _bankRegistrationRepo;
        private readonly IDbEntityRepository<Bank> _bankRepo;
        private readonly IDbMultiEntityMethods _dbContextService;
        private readonly IDbEntityRepository<DomesticPaymentConsent> _domesticConsentRepo;
        private readonly ISoftwareStatementProfileService _softwareStatementProfileService;

        public PostAuthorisationRedirectObject(
            IApiClient apiClient,
            IDbEntityRepository<Bank> bankRepo,
            IDbEntityRepository<BankProfile> bankProfileRepo,
            IDbMultiEntityMethods dbContextService,
            IDbEntityRepository<DomesticPaymentConsent> domesticConsentRepo,
            IDbEntityRepository<BankRegistration> bankRegistrationRepo,
            ISoftwareStatementProfileService softwareStatementProfileService)
        {
            _apiClient = apiClient;
            _bankRepo = bankRepo;
            _bankProfileRepo = bankProfileRepo;
            _dbContextService = dbContextService;
            _domesticConsentRepo = domesticConsentRepo;
            _bankRegistrationRepo = bankRegistrationRepo;
            _softwareStatementProfileService = softwareStatementProfileService;
        }

        public async Task<AuthorisationRedirectObjectResponse> PostAsync(AuthorisationRedirectObject request)
        {
            request.ArgNotNull(nameof(request));

            // Load relevant data objects
            DomesticPaymentConsent paymentConsent =
                (await _domesticConsentRepo.GetAsync(dc => dc.State.Data == request.Response.State))
                .FirstOrDefault() ?? throw new KeyNotFoundException(
                    $"Consent with redirect state '{request.Response.State}' not found.");
            BankProfile apiProfile = await _bankProfileRepo.GetAsync(paymentConsent.BankProfileId) ??
                                     throw new KeyNotFoundException("API profile cannot be found.");
            BankRegistration bankClientProfile =
                await _bankRegistrationRepo.GetAsync(apiProfile.BankRegistrationId) ??
                throw new KeyNotFoundException("Bank client profile cannot be found.");
            Bank bank = await _bankRepo.GetAsync(apiProfile.BankId)
                        ?? throw new KeyNotFoundException("No record found for BankId in BankProfile.");
            SoftwareStatementProfile softwareStatementProfile =
                _softwareStatementProfileService.GetSoftwareStatementProfile(
                    bankClientProfile.SoftwareStatementProfileId);

            // Obtain token for consent
            string redirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
            TokenEndpointResponse tokenEndpointResponse =
                await PostAuthCodeGrant(
                    authCode: request.Response.Code,
                    redirectUrl: redirectUrl,
                    client: bankClientProfile,
                    orgId: bank.XFapiFinancialId);

            // Update consent with token
            paymentConsent.TokenEndpointResponse = new ReadWriteProperty<TokenEndpointResponse?>(
                data: tokenEndpointResponse,
                timeProvider: new TimeProvider(),
                modifiedBy: null);
            await _dbContextService.SaveChangesAsync();

            return new AuthorisationRedirectObjectResponse();
        }

        private async Task<TokenEndpointResponse> PostAuthCodeGrant(
            string authCode,
            string redirectUrl,
            BankRegistration client,
            string orgId)
        {
            UriBuilder ub = new UriBuilder(new Uri(client.OpenIdConfiguration.TokenEndpoint));

            // Assemble URL-encoded form data
            string authHeader = null;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl },
                { "code", authCode }
            };
            if (client.BankClientRegistrationRequest.TokenEndpointAuthMethod ==
                TokenEndpointAuthMethodEnum.TlsClientAuth)
            {
                keyValuePairs["client_id"] = client.BankClientRegistration.ClientId;
            }
            else if (client.BankClientRegistrationRequest.TokenEndpointAuthMethod ==
                     TokenEndpointAuthMethodEnum.ClientSecretBasic)
            {
                client.BankClientRegistration.ClientSecret.ArgNotNull("No client secret available.");
                string authString = client.BankClientRegistration.ClientId + ":" +
                                    client.BankClientRegistration.ClientSecret;
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(authString);
                authHeader = "Basic " + Convert.ToBase64String(plainTextBytes);
            }
            else
            {
                if (client.BankClientRegistrationRequest.TokenEndpointAuthMethod ==
                    TokenEndpointAuthMethodEnum.TlsClientAuth)
                {
                    throw new InvalidOperationException("Found unsupported TokenEndpointAuthMethod");
                }
            }

            string content = keyValuePairs.ToUrlEncoded();

            // Assemble headers
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader(name: "x-fapi-financial-id", value: orgId)
            };
            if (authHeader != null)
            {
                headers.Add(new HttpHeader(name: "Authorization", value: authHeader));
            }

            TokenEndpointResponse resp = await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/x-www-form-urlencoded")
                .SetContent(content)
                .Create()
                .RequestJsonAsync<TokenEndpointResponse>(client: _apiClient, requestContentIsJson: false);

            return resp;
        }
    }
}
