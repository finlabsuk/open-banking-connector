// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Web;

public class TestingAuthResult
{
    public required string State { get; init; }
    public required IEnumerable<KeyValuePair<string, string?>> RedirectParameters { get; init; }
}
