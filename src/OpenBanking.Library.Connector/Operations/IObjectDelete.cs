// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

public class LocalDeleteParams
{
    public required Guid Id { get; init; }
    public required string? ModifiedBy { get; init; }
}

public class BankRegistrationDeleteParams : LocalDeleteParams
{
    public required bool ExcludeExternalApiOperation { get; init; }
}

public class ConsentDeleteParams : LocalDeleteParams
{
    public required IEnumerable<HttpHeader>? ExtraHeaders { get; init; }

    public required bool ExcludeExternalApiOperation { get; init; }
}

internal interface IObjectDelete<in TDeleteParams>
    where TDeleteParams : LocalDeleteParams
{
    Task<IList<IFluentResponseInfoOrWarningMessage>> DeleteAsync(TDeleteParams deleteParams);
}
