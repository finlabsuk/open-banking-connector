// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class AuthContextsApiClient(WebAppClient client)
{
    public async Task<AuthContextUpdateAuthResultResponse> RedirectDelegate(
        IEnumerable<KeyValuePair<string, string?>> formCollection)
    {
        // Read object
        var uriPath = "/auth/redirect-delegate";
        var response =
            await client.CreateFromFormAsync<AuthContextUpdateAuthResultResponse>(
                uriPath,
                formCollection);

        // Checks
        response.Warnings.Should().BeNull();

        return response;
    }
}
