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
        ICreateContext<TPublicRequest, TPublicResponse>,
        IReadContext<TPublicResponse>
        where TPublicResponse : class { }

    internal interface IExternalEntityContextInternal<in TPublicRequest, TPublicResponse> :
        IExternalEntityContext<TPublicRequest, TPublicResponse>,
        ICreateContextInternal<TPublicRequest, TPublicResponse>,
        IReadContextInternal<TPublicResponse>
        where TPublicResponse : class
        where TPublicRequest : class, ISupportsValidation { }

    internal class ExternalEntityContextInternal<TPublicRequest, TPublicResponse> :
        IExternalEntityContextInternal<TPublicRequest, TPublicResponse>
        where TPublicRequest : class, ISupportsValidation
        where TPublicResponse : class
    {
        public ExternalEntityContextInternal(
            ISharedContext context,
            IObjectPost<TPublicRequest, TPublicResponse> postObject,
            IObjectRead<TPublicResponse> readObject)
        {
            ReadObject = readObject;
            Context = context;
            PostObject = postObject;
        }

        public IObjectRead<TPublicResponse> ReadObject { get; }

        public ISharedContext Context { get; }
        public IObjectPost<TPublicRequest, TPublicResponse> PostObject { get; }
    }
}
