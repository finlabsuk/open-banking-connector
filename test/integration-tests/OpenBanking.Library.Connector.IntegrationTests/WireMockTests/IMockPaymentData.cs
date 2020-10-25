// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets.Cached;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests
{
    public interface IMockPaymentData
    {
        IRequestBuilder CreateMockRequestBuilder(
            IDictionary<string, SoftwareStatementProfile> softwareStatementProfileDictionary);

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
