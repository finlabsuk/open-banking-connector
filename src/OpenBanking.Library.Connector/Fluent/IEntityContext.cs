// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    /// <summary>
    ///     Fluent context for entity created both in local and external (i.e. bank) database.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicQuery"></typeparam>
    /// <typeparam name="TPublicReadResponse"></typeparam>
    /// <typeparam name="TPublicReadLocalResponse"></typeparam>
    public interface IEntityContext<in TPublicRequest, TPublicQuery, TPublicReadResponse, TPublicReadLocalResponse> :
        ICreateContext<TPublicRequest, TPublicReadResponse>,
        IReadContext<TPublicReadResponse>,
        IReadLocalContext<TPublicQuery, TPublicReadLocalResponse>,
        IDeleteLocalContext
        where TPublicReadResponse : class
        where TPublicReadLocalResponse : class { }

    internal interface IEntityContextInternal<in TPublicRequest, TPublicQuery, TPublicResponse,
        TPublicReadLocalResponse> :
        IEntityContext<TPublicRequest, TPublicQuery, TPublicResponse, TPublicReadLocalResponse>,
        ICreateContextInternal<TPublicRequest, TPublicResponse>,
        IReadContextInternal<TPublicResponse>,
        IReadLocalContextInternal<TPublicQuery, TPublicReadLocalResponse>,
        IDeleteLocalContextInternal
        where TPublicResponse : class
        where TPublicRequest : class, ISupportsValidation
        where TPublicReadLocalResponse : class { }
}
