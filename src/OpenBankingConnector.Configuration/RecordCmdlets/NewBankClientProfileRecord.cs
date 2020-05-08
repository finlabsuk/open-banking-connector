// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBankingConnector.Configuration.RecordCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "BankClientProfileRecord")]
    [OutputType(typeof(BankClientProfileResponse))]
    public class NewBankClientProfileRecord : RecordBaseCmdlet
    {
        private readonly ICreateBankClientProfile _createBankClientProfile;

        public NewBankClientProfileRecord() : base(verbName: "New", nounName: "BankClientProfileRecord")
        {
            _createBankClientProfile = _serviceProvider.GetService<ICreateBankClientProfile>();
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public BankClientProfile? BankClientProfile { get; set; }

        protected override void ProcessRecord()
        {
            List<FluentResponseMessage> messages = new List<FluentResponseMessage>();
            try
            {
                var response = _createBankClientProfile
                    .CreateAsync(BankClientProfile)
                    .GetAwaiter()
                    .GetResult();
                var response2 = new BankClientProfileFluentResponse(messages: messages, data: response);
                WriteObject(response2);
            }
            catch (Exception ex)
            {
                //context.Context.Instrumentation.Exception(ex);
                var response = new OpenBankingSoftwareStatementResponse(message: ex.CreateErrorMessage(), data: null);
            }
            _streamWriter.Flush();
            var outputString = (Encoding.UTF8).GetString(_memoryStream.ToArray());
            WriteVerbose(outputString);
        }
    }
}
