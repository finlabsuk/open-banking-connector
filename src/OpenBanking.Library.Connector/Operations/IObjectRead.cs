// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class LocalReadParams
{
    public LocalReadParams(Guid id, string? modifiedBy)
    {
        Id = id;
        ModifiedBy = modifiedBy;
    }

    public Guid Id { get; }
    public string? ModifiedBy { get; }
}

internal class BankRegistrationReadParams : LocalReadParams
{
    public BankRegistrationReadParams(
        Guid id,
        string? modifiedBy,
        bool? useRegistrationAccessToken,
        bool? includeExternalApiOperation) : base(id, modifiedBy)
    {
        UseRegistrationAccessToken = useRegistrationAccessToken;
        IncludeExternalApiOperation = includeExternalApiOperation;
    }

    public bool? UseRegistrationAccessToken { get; }
    public bool? IncludeExternalApiOperation { get; }
}

internal class ConsentBaseReadParams : LocalReadParams
{
    public ConsentBaseReadParams(Guid id, string? modifiedBy, string? publicRequestUrlWithoutQuery) : base(
        id,
        modifiedBy)
    {
        PublicRequestUrlWithoutQuery = publicRequestUrlWithoutQuery;
    }

    public string? PublicRequestUrlWithoutQuery { get; }
}

internal class ConsentReadParams : ConsentBaseReadParams
{
    public ConsentReadParams(
        Guid id,
        string? modifiedBy,
        string? publicRequestUrlWithoutQuery,
        bool includeExternalApiOperation) : base(id, modifiedBy, publicRequestUrlWithoutQuery)
    {
        IncludeExternalApiOperation = includeExternalApiOperation;
    }

    public bool IncludeExternalApiOperation { get; }
}

internal interface IObjectRead<TPublicResponse, in TReadParams>
    where TReadParams : LocalReadParams
{
    Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ReadAsync(
        TReadParams readParams);
}

internal interface IObjectReadFundsConfirmation<TPublicResponse, in TReadParams>
    where TReadParams : ConsentBaseReadParams
{
    Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadFundsConfirmationAsync(TReadParams readParams);
}

internal interface
    IObjectReadWithSearch<TPublicQuery, TPublicResponse, in TReadParams> : IObjectRead<TPublicResponse, TReadParams>
    where TReadParams : LocalReadParams
{
    Task<(IQueryable<TPublicResponse> response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages
            )>
        ReadAsync(Expression<Func<TPublicQuery, bool>> predicate);
}
