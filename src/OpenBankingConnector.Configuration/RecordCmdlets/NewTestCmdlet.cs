// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBankingConnector.Configuration.RecordCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "TestCmdlet")]
    [OutputType(typeof(BankClientProfileResponse))]
    public class NewTestCmdlet : RecordBaseCmdlet
    {
        public NewTestCmdlet() : base(
            verbName: "New",
            nounName: "NoActionRecord",
            deleteAndRecreateDb: true,
            setUpSoftwareStatementProfileService: false,
            loadSecretsFromConfig: false) { }

        protected override void ProcessRecordInner(IServiceProvider services)
        {
            List<FluentResponseMessage> messages = new List<FluentResponseMessage>();
            PaymentInitiationApiProfile apiProfile = new PaymentInitiationApiProfile(
                id: "id",
                bankClientProfileId: "",
                apiVersion: ApiVersion.V3P1P1,
                baseUrl: "https://www.google.com/");
            try
            {
                ICreatePaymentInitiationApiProfile createApiProfile =
                    services.GetService<ICreatePaymentInitiationApiProfile>();
                PaymentInitiationApiProfileResponse response = createApiProfile
                    .CreateAsync(apiProfile)
                    .GetAwaiter()
                    .GetResult();
                PaymentInitiationApiProfileFluentResponse response2 =
                    new PaymentInitiationApiProfileFluentResponse(messages: messages, data: response);
                WriteObject(response2);
            }
            catch (Exception ex)
            {
                WriteError(
                    new ErrorRecord(
                        exception: ex,
                        errorId: "Could not create record",
                        errorCategory: ErrorCategory.InvalidOperation,
                        targetObject: null));
            }
        }
    }
}
