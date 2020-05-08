// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBankingConnector.Configuration.RecordCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "PaymentInitiationApiProfileRecord")]
    [OutputType(typeof(BankClientProfileResponse))]
    public class NewPaymentInitiationApiProfileRecord : RecordBaseCmdlet
    {
        private readonly ICreateApiProfile _createApiProfile;

        public NewPaymentInitiationApiProfileRecord() : base(
            verbName: "New",
            nounName: "PaymentInitiationApiProfileRecord")
        {
            _createApiProfile = _serviceProvider.GetService<ICreateApiProfile>();
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public PaymentInitiationApiProfile? PaymentInitiationApiProfile { get; set; }

        protected override void ProcessRecord()
        {
            List<FluentResponseMessage> messages = new List<FluentResponseMessage>();
            try
            {
                PaymentInitiationApiProfileResponse response = _createApiProfile
                    .CreateAsync(PaymentInitiationApiProfile)
                    .GetAwaiter()
                    .GetResult();
                PaymentInitiationApiProfileFluentResponse response2 =
                    new PaymentInitiationApiProfileFluentResponse(messages: messages, data: response);
                WriteObject(response2);
            }
            catch (Exception ex)
            {
                //context.Context.Instrumentation.Exception(ex);
                PaymentInitiationApiProfileFluentResponse response =
                    new PaymentInitiationApiProfileFluentResponse(message: ex.CreateErrorMessage(), data: null);
            }

            _streamWriter.Flush();
            string outputString = Encoding.UTF8.GetString(_memoryStream.ToArray());
            WriteVerbose(outputString);
        }
    }
}
