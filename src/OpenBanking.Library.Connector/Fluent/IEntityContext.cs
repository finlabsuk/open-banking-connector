// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IEntityContext<in TPublicRequest, TPublicQuery, TPublicResponse> :
        ICreateContext<TPublicRequest, TPublicResponse>,
        IReadContext<TPublicQuery, TPublicResponse>,
        IDeleteLocalContext
        where TPublicResponse : class { }
}
