// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.AccountAndTransaction;

/// <summary>
///     Fluent context for entity created in external (i.e. bank) database only.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
public interface ITransactionsContext<TPublicResponse> :
    IReadTransactionsContext<TPublicResponse>
    where TPublicResponse : class { }

internal interface ITransactionsContextInternal<TPublicResponse> :
    ITransactionsContext<TPublicResponse>,
    IReadTransactionsContextInternal<TPublicResponse>
    where TPublicResponse : class { }

internal class TransactionsContextInternal<TPublicResponse> :
    ITransactionsContextInternal<TPublicResponse>
    where TPublicResponse : class
{
    public TransactionsContextInternal(
        ISharedContext context,
        IAccountAccessConsentExternalRead<TPublicResponse, TransactionsReadParams> readObject)
    {
        ReadObject = readObject;
        Context = context;
    }

    public ISharedContext Context { get; }

    public IAccountAccessConsentExternalRead<TPublicResponse, TransactionsReadParams> ReadObject { get; }
}
