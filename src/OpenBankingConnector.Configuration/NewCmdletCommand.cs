using System.Management.Automation;
//using BankClientProfile = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankClientProfile;
using BankClientProfileResponse2 = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response.BankClientProfileResponse;

namespace FinnovationLabs.OpenBanking.PsModule.Connector
{
    [Cmdlet(VerbsDiagnostic.Test, "NewCmdlet")]
    [OutputType(typeof(BankClientProfileResponse2))]
    public class NewCmdletCommand : PSCmdlet
    {

        /*        [Parameter(
                    Mandatory = true,
                    Position = 0,
                    ValueFromPipeline = true,
                    ValueFromPipelineByPropertyName = true)]
                public BankClientProfile info { get; set; } = new BankClientProfile();

        */
        [Parameter(
          Position = 0,
          ValueFromPipelineByPropertyName = true)]
        public string AnyString { get; set; } = "MyString";

        protected override void BeginProcessing()
        {
            WriteVerbose("A");
        }

        protected override void ProcessRecord()
        {
            WriteObject(new BankClientProfileResponse2());
        }

        protected override void EndProcessing()
        {
            WriteVerbose("C");
        }
    }
}
