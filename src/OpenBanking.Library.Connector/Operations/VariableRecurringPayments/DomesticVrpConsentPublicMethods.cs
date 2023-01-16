// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
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
    }
}
