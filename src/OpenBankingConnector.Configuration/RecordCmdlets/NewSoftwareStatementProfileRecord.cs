// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.NetGenericHost.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBankingConnector.Configuration.RecordCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "SoftwareStatementProfileRecord")]
    [OutputType(typeof(BankClientProfileResponse))]
    public class NewSoftwareStatementProfileRecord : RecordBaseCmdlet
    {
        public NewSoftwareStatementProfileRecord() : base(verbName: "New", nounName: "SoftwareStatementProfileRecord")
        {
            _host.Services.CheckDbExists();
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public SoftwareStatementProfile? SoftwareStatementProfile { get; set; }

        protected override void ProcessRecordInner(IServiceProvider services)
        {
            List<FluentResponseMessage> messages = new List<FluentResponseMessage>();
            try
            {
                ICreateSoftwareStatementProfile createSoftwareStatementProfile =
                    services.GetService<ICreateSoftwareStatementProfile>();
                SoftwareStatementProfileResponse response = createSoftwareStatementProfile
                    .CreateAsync(SoftwareStatementProfile).GetAwaiter()
                    .GetResult();
                OpenBankingSoftwareStatementResponse response2 =
                    new OpenBankingSoftwareStatementResponse(messages: messages, data: response);
                WriteObject(response2);
            }
            catch (Exception ex)
            {
                //context.Context.Instrumentation.Exception(ex);
                OpenBankingSoftwareStatementResponse response =
                    new OpenBankingSoftwareStatementResponse(message: ex.CreateErrorMessage(), data: null);
            }
        }
    }
}
