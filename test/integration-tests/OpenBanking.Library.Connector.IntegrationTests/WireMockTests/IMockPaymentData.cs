using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests
{
    public interface IMockPaymentData
    {
        IOpenBankingRequestBuilder CreateMockRequestBuilder();
        string GetAccessToken();
        string GetBase64TokenString();
        string GetClientId();
        string GetClientSecret();
        string GetFapiHeader();
        string[] GetGrantTypes();
        string GetIdempotencyKey();
        string GetJwsSignature();
        string GetMockCertificate();
        string GetMockPrivateKey();
        string GetOBWriteDomesticConsent2();
        string GetOBWriteDomesticConsentResponse2();
        string GetOpenBankingClientRegistrationResponseJson();
        string GetOpenIdConfigJson();
        string GetOpenIdTokenEndpointResponseJson();
        string GetPaymentConsentId();
        string[] GetRedirectUris();
    }
}
