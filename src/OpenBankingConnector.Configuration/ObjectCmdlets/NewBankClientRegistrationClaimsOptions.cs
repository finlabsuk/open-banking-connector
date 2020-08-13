// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Management.Automation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;

namespace OpenBankingConnector.Configuration.ObjectCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "BankClientRegistrationClaimsOptions")]
    [OutputType(typeof(BankClientRegistrationClaimsOverrides))]
    public class NewBankClientRegistrationClaimsOptions : BaseCmdlet
    {
        public NewBankClientRegistrationClaimsOptions() : base("New", "BankClientRegistrationClaimsOptions")
        {
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Aud { get; set; } = "";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public bool ScopeUseStringArray { get; set; }

        protected override void ProcessRecord()
        {
            BankClientRegistrationClaimsOverrides output = new BankClientRegistrationClaimsOverrides
            {
                SsaIssuer = null,
                RequestAudience = Aud,
                TokenEndpointAuthMethod = null,
                GrantTypes = null,
                ScopeUseStringArray = ScopeUseStringArray,
                TokenEndpointAuthSigningAlgorithm = null
            };
            WriteObject(output);
        }
    }
}
