// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    internal delegate Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        PostEntityAsyncWrapperDelegate<in TPublicRequest,
            TPublicResponse>(
            ISharedContext context,
            TPublicRequest request,
            string? createdBy);

    /// <summary>
    ///     "Post-only" type with public interface. Post-only types are similar to entity types with public interface (
    ///     <see cref="ISupportsFluentDeleteLocal{TSelf,TPublicRequest,TPublicResponse,TPublicQuery}" />) but are
    ///     not directly persisted to DB and can only be POSTed.
    /// </summary>
    internal interface ISupportsFluentPost<in TPublicRequest, TPublicPostResponse>
    {
        /// <summary>
        ///     NB: static method needs to be wrapped in instance method due to C# not yet supporting abstract interface static
        ///     members
        /// </summary>
        PostEntityAsyncWrapperDelegate<TPublicRequest, TPublicPostResponse> PostEntityAsyncWrapper { get; }
    }
}
