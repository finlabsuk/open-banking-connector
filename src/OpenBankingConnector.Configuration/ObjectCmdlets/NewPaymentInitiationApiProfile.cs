// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Management.Automation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace OpenBankingConnector.Configuration.ObjectCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "PaymentInitiationApiProfile")]
    [OutputType(typeof(BankRegistrationClaimsOverrides))]
    public class NewPaymentInitiationApiProfile : BaseCmdlet
    {
        public NewPaymentInitiationApiProfile() : base(verbName: "New", nounName: "PaymentInitiationApiProfile") { }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public Guid BankClientProfileId { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public ApiVersion ApiVersion { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string BaseUrl { get; set; }


        protected override void ProcessRecord()
        {
            BankApiInformation output = new BankApiInformation
            {
                //BankRegistrationId = BankClientProfileId,
                PaymentInitiationApi = new PaymentInitiationApi
                {
                    ApiVersion = ApiVersion,
                    BaseUrl = BaseUrl
                }
                //Id = Id
            };
            WriteObject(output);
        }
    }
}
