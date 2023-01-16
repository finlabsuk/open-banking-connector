// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

public static class DomesticPaymentPublicMethods
{
    public static OBWriteDomestic2 ResolveExternalApiRequest(
        OBWriteDomestic2? externalApiRequest,
        DomesticPaymentTemplateRequest? templateRequest,
        string externalApiConsentId,
        BankProfile? bankProfile)
    {
        // Resolve external API request
        OBWriteDomestic2 resolvedExternalApiRequest =
            externalApiRequest ??
            DomesticPaymentTemplates.DomesticPaymentExternalApiRequest(
                templateRequest ??
                throw new InvalidOperationException(
                    "Both ExternalApiRequest and TemplateRequest specified as null so not possible to create external API request."),
                externalApiConsentId);

        // Customise external API request using bank profile
        if (bankProfile is not null)
        {
            resolvedExternalApiRequest = bankProfile.PaymentInitiationApiSettings
                .DomesticPaymentExternalApiRequestAdjustments(resolvedExternalApiRequest);
        }

        return resolvedExternalApiRequest;
    }
}
