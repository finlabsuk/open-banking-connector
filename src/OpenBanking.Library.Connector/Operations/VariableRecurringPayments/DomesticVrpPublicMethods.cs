// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;

public static class DomesticVrpPublicMethods
{
    public static OBDomesticVRPRequest ResolveExternalApiRequest(
        OBDomesticVRPRequest? externalApiRequest,
        DomesticVrpTemplateRequest? templateRequest,
        string externalApiConsentId,
        BankProfile? bankProfile)
    {
        // Resolve external API request
        OBDomesticVRPRequest resolvedExternalApiRequest =
            externalApiRequest ??
            DomesticVrpTemplates.DomesticVrpExternalApiRequest(
                templateRequest ??
                throw new InvalidOperationException(
                    "Both ExternalApiRequest and TemplateRequest specified as null so not possible to create external API request."),
                externalApiConsentId);

        // Customise external API request using bank profile
        if (bankProfile is not null)
        {
            resolvedExternalApiRequest = bankProfile.VariableRecurringPaymentsApiSettings
                .DomesticVrpExternalApiRequestAdjustments(resolvedExternalApiRequest);
        }

        return resolvedExternalApiRequest;
    }
}
