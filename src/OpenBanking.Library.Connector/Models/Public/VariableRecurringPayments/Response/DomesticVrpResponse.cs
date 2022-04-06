// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response
{
    /// <summary>
    ///     Response to Read and Create requests
    /// </summary>
    public class DomesticVrpResponse
    {
        public DomesticVrpResponse(
            Guid id,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse externalApiResponse)
        {
            Id = id;
            ExternalApiResponse = externalApiResponse;
        }

        public Guid Id { get; }


        public VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse ExternalApiResponse { get; }
    }
}
