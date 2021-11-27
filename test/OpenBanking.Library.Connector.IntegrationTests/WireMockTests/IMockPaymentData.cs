// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests
{
    public interface IMockPaymentData
    {
        IRequestBuilder CreateMockRequestBuilder(
            IProcessedSoftwareStatementProfileStore softwareStatementProfilesRepository);

        string GetAccessToken();
        string GetBase64TokenString();
        string GetClientId();
        string GetClientSecret();
        string GetFapiHeader();
        ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum[] GetGrantTypes();
        string GetIdempotencyKey();
        string GetJwsSignature();
        string GetMockCertificate();
        string GetMockPrivateKey();
        string GetOBWriteDomesticConsent(BankProfile bankProfile);
        string GetOBWriteDomesticConsentResponse2(BankProfile bankProfile);
        string GetOpenBankingClientRegistrationResponseJson();
        string GetOpenIdConfigJson();
        string GetOpenIdTokenEndpointResponseJson();
        string GetPaymentConsentId();
        string GetOBWriteDomesticResponse2();
        string GetAuthtoriseResponse();
        string[] GetRedirectUris();
    }
}
