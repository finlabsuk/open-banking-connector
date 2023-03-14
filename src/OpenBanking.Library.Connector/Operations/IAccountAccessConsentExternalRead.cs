// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class ExternalEntityReadParams
{
    public ExternalEntityReadParams(
        Guid consentId,
        string? modifiedBy,
        string? externalApiAccountId,
        string? publicRequestUrlWithoutQuery,
        string? queryString)
    {
        ConsentId = consentId;
        ModifiedBy = modifiedBy;
        ExternalApiAccountId = externalApiAccountId;
        PublicRequestUrlWithoutQuery = publicRequestUrlWithoutQuery;
        QueryString = queryString;
    }

    public Guid ConsentId { get; }
    public string? ModifiedBy { get; }
    public string? ExternalApiAccountId { get; }
    public string? PublicRequestUrlWithoutQuery { get; }
    public string? QueryString { get; }
}

internal class TransactionsReadParams : ExternalEntityReadParams
{
    public TransactionsReadParams(
        Guid consentId,
        string? modifiedBy,
        string? externalApiAccountId,
        string? publicRequestUrlWithoutQuery,
        string? queryString,
        string? externalApiStatementId) : base(
        consentId,
        modifiedBy,
        externalApiAccountId,
        publicRequestUrlWithoutQuery,
        queryString)
    {
        ExternalApiStatementId = externalApiStatementId;
    }

    public string? ExternalApiStatementId { get; }
}

internal interface IAccountAccessConsentExternalRead<TPublicResponse, TReadParams>
    where TReadParams : ExternalEntityReadParams
{
    Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ReadAsync(
        TReadParams readParams);
}
