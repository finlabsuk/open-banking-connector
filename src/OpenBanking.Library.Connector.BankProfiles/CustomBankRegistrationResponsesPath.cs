// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public partial class BankProfile
    {
        partial void CustomBankRegistrationResponsesPath(ref string path)
        {
            path =
                @"C:\Repos\open-banking-connector-csharp\src\OpenBanking.Library.Connector.BankProfiles\Sandbox\RegistrationResponses";
        }
    }
}
