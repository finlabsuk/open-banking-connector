// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

public static class AccountAccessConsentPostHelper
{
    public static AccountAccessConsentRequest ResolveToLowLevelRequest(
        BankProfile? bankProfile,
        AccountAccessConsentRequest request)
    {
        // Resolve external API request
        if (request.ExternalApiObject is null)
        {
            OBReadConsent1 externalApiRequest =
                request.ExternalApiRequest ??
                TemplateRequests.AccountAccessConsentExternalApiRequest(
                    request.TemplateRequest?.Type ??
                    throw new InvalidOperationException(
                        "Both ExternalApiRequest and TemplateRequest specified as null so not possible to create external API request."));
            if (bankProfile is not null)
            {
                externalApiRequest = bankProfile.AccountAndTransactionApiSettings
                    .ExternalApiRequestAdjustments(externalApiRequest);
            }

            request.ExternalApiRequest = externalApiRequest;
        }

        return request;
    }
}
