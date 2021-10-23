// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    internal enum ReadWriteApiType
    {
        AccountTransaction,
        PaymentInitiation,
        VariableRecurringPayments
    }

    internal interface ISupportsFluentReadWriteGet<out TPublicResponse, TApiResponse> :
        ISupportsFluentEntityGet<TPublicResponse, TApiResponse>
        where TApiResponse : class, ISupportsValidation
    {
        public ReadWriteApiType GetReadWriteApiType();

        public IApiGetRequests<TApiResponse> ApiGetRequests(
            PaymentInitiationApi? paymentInitiationApi,
            VariableRecurringPaymentsApi? variableRecurringPaymentsApi,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient);
    }
}
