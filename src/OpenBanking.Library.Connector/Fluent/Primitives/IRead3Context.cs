// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    /// <summary>
    ///     Fluent interface methods for Read.
    /// </summary>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface IRead3Context<TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     READ objects using consent ID (includes GETing objects from bank API).
        ///     Objects will be read from bank database only.
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="consentId"></param>
        /// <param name="modifiedBy"></param>
        /// <returns></returns>
        Task<TPublicResponse> ReadAsync(
            string externalId,
            Guid consentId,
            string? modifiedBy = null);
    }

    internal interface
        IRead3ContextInternal<TPublicResponse> : IRead3Context<TPublicResponse>,
            IBaseContextInternal
        where TPublicResponse : class
    {
        IObjectRead3<TPublicResponse> ReadObject { get; }

        async Task<TPublicResponse> IRead3Context<TPublicResponse>.ReadAsync(
            string externalId,
            Guid consentId,
            string? modifiedBy)
        {
            (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
                await ReadObject.ReadAsync(
                    externalId,
                    consentId,
                    modifiedBy);

            return response;
        }
    }
}
