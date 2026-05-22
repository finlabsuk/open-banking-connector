// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal record AuthContextNotFoundServerError(string State) : ServerError
{
    public override ServerErrorType ServerErrorType => ServerErrorType.AuthContextNotFound;
    public override string Title => "authContextNotFound";
    public override string Detail => "No record found for Auth Context with specified state.";
    public override int StatusCode => 400;

    public override IReadOnlyDictionary<string, object?> Extensions { get; } =
        new Dictionary<string, object?> { ["state"] = State };
}

internal record AuthContextStaleServerError(string State) : ServerError
{
    public override ServerErrorType ServerErrorType => ServerErrorType.AuthContextStale;
    public override string Title => "authContextStale";

    public override string Detail =>
        "Auth context exists but now stale (more than ten minutes old) so will not process redirect. Please create a new auth context and authenticate again.";

    public override int StatusCode => 400;

    public override IReadOnlyDictionary<string, object?> Extensions { get; } =
        new Dictionary<string, object?> { ["state"] = State };
}
