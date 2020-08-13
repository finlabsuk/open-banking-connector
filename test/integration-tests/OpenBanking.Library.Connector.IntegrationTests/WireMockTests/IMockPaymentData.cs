using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using System;
using System.Collections.Generic;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;

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
        GrantTypesItemEnum[] GetGrantTypes();
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
        string GetOBWriteDomesticResponse2();
        string GetAuthtoriseResponse();
        string[] GetRedirectUris();
    }
}
