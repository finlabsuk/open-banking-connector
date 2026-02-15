// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

public class LocalReadParams
{
    public required Guid Id { get; init; }
    public required string? ModifiedBy { get; init; }
}

public class LocalReadAllParams { }

public class BankRegistrationReadParams : LocalReadParams
{
    public required bool ExcludeExternalApiOperation { get; init; }
}

public class ConsentBaseReadParams : LocalReadParams
{
    public required IEnumerable<HttpHeader>? ExtraHeaders { get; init; }
    public required string? PublicRequestUrlWithoutQuery { get; init; }
}

public class ConsentReadParams : ConsentBaseReadParams
{
    public required bool ExcludeExternalApiOperation { get; init; }
}

internal interface IObjectRead<TPublicResponse, in TReadParams>
    where TReadParams : LocalReadParams
{
    Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ReadAsync(
        TReadParams readParams);
}

internal interface
    IObjectReadWithSearch<TPublicQuery, TPublicResponse, in TReadParams> : IObjectRead<TPublicResponse, TReadParams>
    where TReadParams : LocalReadParams
{
    Task<(IQueryable<TPublicResponse> response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages
            )>
        ReadAsync(Expression<Func<TPublicQuery, bool>> predicate);
}

internal interface IObjectReadAll<TPublicResponse, in TReadAllParams>
    where TReadAllParams : LocalReadAllParams
{
    Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ReadAllAsync(
        TReadAllParams readParams);
}
