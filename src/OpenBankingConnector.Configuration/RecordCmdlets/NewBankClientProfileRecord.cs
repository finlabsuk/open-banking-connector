// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBankingConnector.Configuration.RecordCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "BankClientProfileRecord")]
    [OutputType(typeof(BankClientProfileFluentResponse))]
    public class NewBankClientProfileRecord : RecordBaseCmdlet
    {
        public NewBankClientProfileRecord() : base(
            verbName: "New",
            nounName: "BankClientProfileRecord",
            deleteAndRecreateDb: false) { }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public BankClientProfile? BankClientProfile { get; set; }

        protected override void ProcessRecordInner(IServiceProvider services)
        {
            List<FluentResponseMessage> messages = new List<FluentResponseMessage>();
            try
            {
                ICreateBankClientProfile createBankClientProfile = services.GetService<ICreateBankClientProfile>();
                BankClientProfileResponse response = createBankClientProfile
                    .CreateAsync(BankClientProfile)
                    .GetAwaiter()
                    .GetResult();
                BankClientProfileFluentResponse response2 =
                    new BankClientProfileFluentResponse(messages: messages, data: response);
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
