// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Management.Automation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace OpenBankingConnector.Configuration.ObjectCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "BankClientRegistrationClaimsOptions")]
    [OutputType(typeof(BankRegistrationClaimsOverrides))]
    public class NewBankClientRegistrationClaimsOptions : BaseCmdlet
    {
        public NewBankClientRegistrationClaimsOptions() : base(
            verbName: "New",
            nounName: "BankClientRegistrationClaimsOptions") { }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Aud { get; set; } = "";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public bool ScopeUseStringArray { get; set; }

        protected override void ProcessRecord()
        {
            BankRegistrationClaimsOverrides output = new BankRegistrationClaimsOverrides
            {
                Audience = Aud,
                TokenEndpointAuthMethod = null,
                GrantTypes = null,
                TokenEndpointAuthSigningAlgorithm = null
            };
            WriteObject(output);
        }
    }
}
