// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    /// <summary>
    ///     Fluent context for entity created in external (i.e. bank) database only.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface IExternalEntityContext<in TPublicRequest, TPublicResponse> :
        ICreate2Context<TPublicRequest, TPublicResponse>,
        IRead3Context<TPublicResponse>
        where TPublicResponse : class { }

    internal interface IExternalEntityContextInternal<in TPublicRequest, TPublicResponse> :
        IExternalEntityContext<TPublicRequest, TPublicResponse>,
        ICreate2ContextInternal<TPublicRequest, TPublicResponse>,
        IRead3ContextInternal<TPublicResponse>
        where TPublicResponse : class
        where TPublicRequest : class, ISupportsValidation { }

    internal class ExternalEntityContextInternal<TPublicRequest, TPublicResponse> :
        IExternalEntityContextInternal<TPublicRequest, TPublicResponse>
        where TPublicRequest : class, ISupportsValidation
        where TPublicResponse : class
    {
        public ExternalEntityContextInternal(
            ISharedContext context,
            IObjectCreate2<TPublicRequest, TPublicResponse> postObject,
            IObjectRead3<TPublicResponse> readObject)
        {
            ReadObject = readObject;
            Context = context;
            CreateObject = postObject;
        }

        public IObjectRead3<TPublicResponse> ReadObject { get; }

        public ISharedContext Context { get; }
        public IObjectCreate2<TPublicRequest, TPublicResponse> CreateObject { get; }
    }
}
