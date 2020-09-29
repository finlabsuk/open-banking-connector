// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBankingConnector.Configuration.RecordCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "PaymentInitiationApiProfileRecord")]
    [OutputType(typeof(FluentResponse<BankProfileResponse>))]
    public class NewPaymentInitiationApiProfileRecord : RecordBaseCmdlet
    {
        public NewPaymentInitiationApiProfileRecord() : base(
            verbName: "New",
            nounName: "PaymentInitiationApiProfileRecord",
            deleteAndRecreateDb: false) { }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public BankProfile? PaymentInitiationApiProfile { get; set; }

        protected override void ProcessRecordInner(IServiceProvider services)
        {
            List<FluentResponseMessage> messages = new List<FluentResponseMessage>();
            try
            {
                // ICreateBankProfile createApiProfile =
                //     services.GetService<ICreateBankProfile>();
                // BankProfileResponse response = createApiProfile
                //     .CreateAsync(PaymentInitiationApiProfile)
                //     .GetAwaiter()
                //     .GetResult();
                // FluentResponse<BankProfileResponse> response2 =
                //     new FluentResponse<BankProfileResponse>(messages: messages, data: response);
                // WriteObject(response2);
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
