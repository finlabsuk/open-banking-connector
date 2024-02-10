// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;

public static class DomesticVrpConsentPublicMethods
{
    public static VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest ResolveExternalApiRequest(
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest? externalApiRequest,
        DomesticVrpTemplateRequest? templateRequest,
        BankProfile? bankProfile)
    {
        // Resolve external API request
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest resolvedExternalApiRequest =
            externalApiRequest ??
            DomesticVrpTemplates.DomesticVrpConsentExternalApiRequest(
                templateRequest ??
                throw new InvalidOperationException(
                    "Both ExternalApiRequest and TemplateRequest specified as null so not possible to create external API request."));

        // Customise external API request using bank profile
        if (bankProfile is not null)
        {
            resolvedExternalApiRequest = bankProfile.VariableRecurringPaymentsApiSettings
                .DomesticVrpConsentExternalApiRequestAdjustments(resolvedExternalApiRequest);
        }

        return resolvedExternalApiRequest;
    }

    public static VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationRequest
        ResolveExternalApiFundsConfirmationRequest(
            VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationRequest? externalApiRequest,
            DomesticVrpTemplateRequest? templateRequest,
            string externalApiConsentId,
            BankProfile? bankProfile)
    {
        // Resolve external API request
        VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationRequest resolvedExternalApiRequest =
            externalApiRequest ??
            DomesticVrpTemplates.DomesticVrpExternalApiFundsConfirmationRequest(
                templateRequest ??
                throw new InvalidOperationException(
                    "Both ExternalApiRequest and TemplateRequest specified as null so not possible to create external API request."),
                externalApiConsentId);

        // Customise external API request using bank profile
        if (bankProfile is not null)
        {
            resolvedExternalApiRequest = bankProfile.VariableRecurringPaymentsApiSettings
                .DomesticVrpConsentExternalApiFundsConfirmationRequestAdjustments(resolvedExternalApiRequest);
        }

        return resolvedExternalApiRequest;
    }
}
