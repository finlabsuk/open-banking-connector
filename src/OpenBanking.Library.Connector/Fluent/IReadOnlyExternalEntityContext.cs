// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    /// <summary>
    ///     Fluent context for entity created in external (i.e. bank) database only.
    /// </summary>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface IReadOnlyExternalEntityContext<TPublicResponse> :
        IRead2Context<TPublicResponse>
        where TPublicResponse : class { }

    internal interface IReadOnlyExternalEntityContextInternal<TPublicResponse> :
        IReadOnlyExternalEntityContext<TPublicResponse>,
        IRead2ContextInternal<TPublicResponse>
        where TPublicResponse : class { }

    internal class ReadOnlyExternalEntityContextInternal<TPublicResponse> :
        IReadOnlyExternalEntityContextInternal<TPublicResponse>
        where TPublicResponse : class
    {
        public ReadOnlyExternalEntityContextInternal(ISharedContext context, IObjectRead2<TPublicResponse> readObject)
        {
            ReadObject = readObject;
            Context = context;
        }

        public IObjectRead2<TPublicResponse> ReadObject { get; }

        public ISharedContext Context { get; }
    }
}
