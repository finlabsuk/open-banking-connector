// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

public static class AccountAccessConsentPublicMethods
{
    public static AccountAndTransactionModelsPublic.OBReadConsent1 ResolveExternalApiRequest(
        AccountAndTransactionModelsPublic.OBReadConsent1? externalApiRequest,
        AccountAccessConsentTemplateRequest? templateRequest,
        BankProfile? bankProfile)
    {
        // Resolve external API request
        AccountAndTransactionModelsPublic.OBReadConsent1 resolvedExternalApiRequest =
            externalApiRequest ??
            TemplateRequests.AccountAccessConsentExternalApiRequest(
                templateRequest?.Type ??
                throw new InvalidOperationException(
                    "Both ExternalApiRequest and TemplateRequest specified as null so not possible to create external API request."));
                
        // Customise external API request using bank profile
        if (bankProfile is not null)
        {
            resolvedExternalApiRequest = bankProfile.AccountAndTransactionApiSettings
                .AccountAccessConsentExternalApiRequestAdjustments(resolvedExternalApiRequest);
        }

        return resolvedExternalApiRequest;

    }
}
