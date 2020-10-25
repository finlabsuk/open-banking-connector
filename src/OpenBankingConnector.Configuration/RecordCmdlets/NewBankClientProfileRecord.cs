// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace OpenBankingConnector.Configuration.RecordCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "BankClientProfileRecord")]
    [OutputType(typeof(FluentResponse<BankRegistrationResponse>))]
    public class NewBankClientProfileRecord : RecordBaseCmdlet
    {
        public NewBankClientProfileRecord() : base(
            verbName: "New",
            nounName: "BankClientProfileRecord",
            deleteAndRecreateDb: false) { }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public BankRegistration? BankClientProfile { get; set; }

        protected override void ProcessRecordInner(IServiceProvider services)
        {
            List<FluentResponseMessage> messages = new List<FluentResponseMessage>();
            try
            {
                // ICreateBankClientProfile createBankClientProfile = services.GetService<ICreateBankClientProfile>();
                // BankRegistrationResponse response = createBankClientProfile
                //     .CreateAsync(BankClientProfile)
                //     .GetAwaiter()
                //     .GetResult();
                // FluentResponse<BankRegistrationResponse> response2 =
                //     new FluentResponse<BankRegistrationResponse>(messages: messages, data: response);
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
