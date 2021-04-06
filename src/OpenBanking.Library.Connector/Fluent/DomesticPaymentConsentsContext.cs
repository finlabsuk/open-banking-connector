// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IDomesticPaymentConsentsContext<TPublicRequest, TPublicResponse, TPublicGetResponse, TPublicQuery,
        TPublicLocalResponse> :
        IApiObjectContext<TPublicRequest, TPublicResponse, TPublicGetResponse, TPublicQuery, TPublicLocalResponse>
        where TPublicResponse : class
        where TPublicLocalResponse : class
        where TPublicGetResponse : class
    {
        /// <summary>
        ///     API for DomesticPayment which corresponds to a domestic payment.
        /// </summary>
    }

    internal class DomesticPaymentConsentsContext<TEntity, TPublicRequest, TPublicResponse, TPublicGetResponse, TPublicQuery,
        TPublicLocalResponse> :
        ApiObjectContext<TEntity, TPublicRequest, TPublicResponse, TPublicGetResponse, TPublicQuery, TPublicLocalResponse>,
        IDomesticPaymentConsentsContext<TPublicRequest, TPublicResponse, TPublicGetResponse, TPublicQuery, TPublicLocalResponse>
        where TEntity : class, TPublicQuery,
        ISupportsFluentGetLocal<TEntity, TPublicQuery, TPublicLocalResponse>,
        ISupportsFluentPost<TPublicRequest, TPublicResponse>,
        ISupportsFluentDeleteLocal<TEntity>,
        ISupportsFluentGet<TPublicGetResponse>, new()
        where TPublicRequest : class, ISupportsValidation
        where TPublicResponse : class
        where TPublicQuery : class
        where TPublicLocalResponse : class, TPublicQuery
        where TPublicGetResponse : class
    {
        private readonly GetContext<TEntity, TPublicGetResponse> _getContext;

        public DomesticPaymentConsentsContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _getContext = new GetContext<TEntity, TPublicGetResponse>(sharedContext);
        }

        public Task<IFluentResponse<TPublicGetResponse>> GetAsync(Guid id) =>
            _getContext.GetAsync(id);
    }
}
