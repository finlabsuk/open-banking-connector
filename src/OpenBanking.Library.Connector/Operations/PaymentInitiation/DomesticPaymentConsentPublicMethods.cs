// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

public static class DomesticPaymentConsentPublicMethods
{
    public static PaymentInitiationModelsPublic.OBWriteDomesticConsent4 ResolveExternalApiRequest(
        PaymentInitiationModelsPublic.OBWriteDomesticConsent4? externalApiRequest,
        DomesticPaymentTemplateRequest? templateRequest,
        BankProfile? bankProfile)
    {
        // Resolve external API request
        PaymentInitiationModelsPublic.OBWriteDomesticConsent4 resolvedExternalApiRequest =
            externalApiRequest ??
            DomesticPaymentTemplates.DomesticPaymentConsentExternalApiRequest(
                templateRequest ??
                throw new InvalidOperationException(
                    "Both ExternalApiRequest and TemplateRequest specified as null so not possible to create external API request."));

        // Customise external API request using bank profile
        if (bankProfile is not null)
        {
            resolvedExternalApiRequest = bankProfile.PaymentInitiationApiSettings
                .DomesticPaymentConsentExternalApiRequestAdjustments(resolvedExternalApiRequest);
        }

        return resolvedExternalApiRequest;
    }
}
