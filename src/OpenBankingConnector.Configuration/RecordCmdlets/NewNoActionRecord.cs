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
    [Cmdlet(verbName: VerbsCommon.New, nounName: "NoActionRecord")]
    [OutputType(typeof(BankClientProfileResponse))]
    public class NewNoActionRecord : RecordBaseCmdlet
    {
        public NewNoActionRecord() : base(verbName: "New", nounName: "NoActionRecord")
        {
            _host.Services.CheckDbExists();
        }

        protected override void ProcessRecordInner(IServiceProvider services)
        {
            List<FluentResponseMessage> messages = new List<FluentResponseMessage>();
            try
            {
                SoftwareStatementProfile SoftwareStatementProfile = new SoftwareStatementProfile();
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
