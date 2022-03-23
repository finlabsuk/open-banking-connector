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
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface IEntityContext<in TPublicRequest, TPublicQuery, TPublicResponse> :
        ICreateContext<TPublicRequest, TPublicResponse>,
        IReadContext<TPublicResponse>,
        IReadLocalContext<TPublicQuery, TPublicResponse>,
        IDeleteLocalContext
        where TPublicResponse : class { }

    internal interface IEntityContextInternal<in TPublicRequest, TPublicQuery, TPublicResponse> :
        IEntityContext<TPublicRequest, TPublicQuery, TPublicResponse>,
        ICreateContextInternal<TPublicRequest, TPublicResponse>,
        IReadContextInternal<TPublicResponse>,
        IReadLocalContextInternal<TPublicQuery, TPublicResponse>,
        IDeleteLocalContextInternal
        where TPublicResponse : class
        where TPublicRequest : class, ISupportsValidation { }
}
