// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Management.Automation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace OpenBankingConnector.Configuration.ObjectCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "BankClientProfile")]
    [OutputType(typeof(BankClientRegistrationClaimsOverrides))]
    public class NewBankClientProfile : BaseCmdlet
    {
        public NewBankClientProfile() : base("New", "BankClientProfile")
        {
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; } = "";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string SoftwareStatementProfileId { get; set; } = "";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string IssuerUrl { get; set; } = "";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string XFapiFinancialId { get; set; } = "";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public BankClientRegistrationClaimsOverrides? BankClientRegistrationClaimsOverrides { get; set; }

        protected override void ProcessRecord()
        {
            BankClientProfile output = new BankClientProfile
            {
                Id = Id,
                SoftwareStatementProfileId = SoftwareStatementProfileId,
                IssuerUrl = IssuerUrl,
                XFapiFinancialId = XFapiFinancialId,
                OpenIdConfigurationOverrides = null,
                HttpMtlsConfigurationOverrides = null,
                BankClientRegistrationClaimsOverrides = BankClientRegistrationClaimsOverrides,
                BankClientRegistrationDataOverrides = null
            };
            WriteObject(output);
        }
    }
}
