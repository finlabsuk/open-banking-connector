// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IAuthContextsContext
    {
        /// <summary>
        ///     Update auth context with auth result which is data received from bank (e.g. via redirect) following user
        ///     authorisation of consent.
        /// </summary>
        Task<IFluentResponse<AuthContextResponse>> UpdateAuthResultLocalAsync(
            AuthResult publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }

    internal class AuthContextsContext : IAuthContextsContext
    {
        private readonly AuthResultUpdate _authResultUpdate;

        public AuthContextsContext(ISharedContext sharedContext)
        {
            _authResultUpdate = new AuthResultUpdate(sharedContext);
        }

        public Task<IFluentResponse<AuthContextResponse>> UpdateAuthResultLocalAsync(
            AuthResult publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _authResultUpdate.PostAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);
    }
}
