// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;

public class BankApiRequestFile
{
    public BankApiRequestFile(string requestFile)
    {
        RequestFile = requestFile;
    }

    public string RequestFile { get; }
}
