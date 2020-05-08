// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Management.Automation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace OpenBankingConnector.Configuration.ObjectCmdlets
{
    [Cmdlet(verbName: VerbsCommon.New, nounName: "SoftwareStatementProfile")]
    [OutputType(typeof(SoftwareStatementProfile))]
    public class NewSoftwareStatementProfile : BaseCmdlet
    {
        public NewSoftwareStatementProfile() : base("New", "SoftwareStatementProfile")
        {
        }

        /// Software statement profile ID as string, e.g. "DevPispSoftwareStatement"
        /// This is your choice; a meaningful name should help debugging throughout OBC.
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; } = "";

        /// Software statement as string, e.g. "A.B.C"
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string SoftwareStatement { get; set; } = "";

        /// Open Banking Signing Key ID as string, e.g. "ABC"
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string SigningKeyId { get; set; } = "";

        /// Open Banking Signing Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        /// TODO: This will be replaced by a secret name
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string SigningKeySecretName { get; set; } = "";

        /// Open Banking Signing Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string SigningCertificate { get; set; } = "";

        /// Open Banking Transport Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        /// TODO: This will be replaced by a secret name
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string TransportKeySecretName { get; set; } = "";

        /// Open Banking Transport Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string TransportCertificate { get; set; } = "";

        /// Default redirect URL for OAuth clients with response_mode == fragment.
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string DefaultFragmentRedirectUrl { get; set; } = "";

        protected override void ProcessRecord()
        {
            SoftwareStatementProfile output = new SoftwareStatementProfile
            {
                Id = Id,
                SoftwareStatement = SoftwareStatement,
                SigningKeyId = SigningKeyId,
                SigningKeySecretName = SigningKeySecretName,
                SigningCertificate = SigningCertificate,
                TransportKeySecretName = TransportKeySecretName,
                TransportCertificate = TransportCertificate,
                DefaultFragmentRedirectUrl = DefaultFragmentRedirectUrl
            };
            WriteObject(output);
        }
    }
}
