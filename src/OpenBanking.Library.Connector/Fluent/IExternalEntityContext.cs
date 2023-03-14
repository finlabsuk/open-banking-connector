// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

/// <summary>
///     Fluent context for entity created in external (i.e. bank) database only.
/// </summary>
/// <typeparam name="TPublicRequest"></typeparam>
/// <typeparam name="TPublicResponse"></typeparam>
public interface IExternalEntityContext<in TPublicRequest, TPublicResponse> :
    ICreateExternalEntityContext<TPublicRequest, TPublicResponse>,
    IReadExternalEntityContext<TPublicResponse>
    where TPublicResponse : class { }

internal interface IExternalEntityContextInternal<in TPublicRequest, TPublicResponse> :
    IExternalEntityContext<TPublicRequest, TPublicResponse>,
    ICreateExternalEntityContextInternal<TPublicRequest, TPublicResponse>,
    IReadExternalEntityContextInternal<TPublicResponse>
    where TPublicResponse : class
    where TPublicRequest : class, ISupportsValidation { }

internal class ExternalEntityContextInternal<TPublicRequest, TPublicResponse> :
    IExternalEntityContextInternal<TPublicRequest, TPublicResponse>
    where TPublicRequest : class, ISupportsValidation
    where TPublicResponse : class
{
    public ExternalEntityContextInternal(
        IExternalCreate<TPublicRequest, TPublicResponse> postObject,
        IExternalRead<TPublicResponse> readObject)
    {
        ReadObject = readObject;
        CreateObject = postObject;
    }

    public IExternalRead<TPublicResponse> ReadObject { get; }
    public IExternalCreate<TPublicRequest, TPublicResponse> CreateObject { get; }
}
