// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

/// <summary>
///     Fluent context for entity created in local database only.
/// </summary>
/// <typeparam name="TPublicRequest"></typeparam>
/// <typeparam name="TPublicQuery"></typeparam>
/// <typeparam name="TPublicCreateLocalResponse"></typeparam>
/// <typeparam name="TPublicReadLocalResponse"></typeparam>
public interface ILocalEntityContext<in TPublicRequest, TPublicQuery, TPublicCreateLocalResponse,
    TPublicReadLocalResponse> :
    ICreateLocalContext<TPublicRequest, TPublicCreateLocalResponse>,
    IReadLocalContext<TPublicQuery, TPublicReadLocalResponse>,
    IDeleteLocalContext
    where TPublicCreateLocalResponse : class
    where TPublicReadLocalResponse : class
    where TPublicRequest : class, ISupportsValidation { }
