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
    public interface IReadFundsConfirmationContext<TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     READ funds confirmation by ID (includes GETing object from bank API).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifiedBy"></param>
        /// <param name="apiResponseWriteFile"></param>
        /// <param name="apiResponseOverrideFile"></param>
        /// <returns></returns>
        Task<TPublicResponse> ReadFundsConfirmationAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }

    internal interface
        IReadFundsConfirmationContextInternal<TPublicResponse> : IReadFundsConfirmationContext<TPublicResponse>,
            IBaseContextInternal
        where TPublicResponse : class
    {
        IObjectRead<TPublicResponse> ReadFundsConfirmationObject { get; }

        async Task<TPublicResponse> IReadFundsConfirmationContext<TPublicResponse>.
            ReadFundsConfirmationAsync(
                Guid id,
                string? modifiedBy,
                string? apiResponseWriteFile,
                string? apiResponseOverrideFile)
        {
            (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
                await ReadFundsConfirmationObject.ReadAsync(
                    id,
                    modifiedBy,
                    apiResponseWriteFile,
                    apiResponseOverrideFile);
            return response;
        }
    }
}
