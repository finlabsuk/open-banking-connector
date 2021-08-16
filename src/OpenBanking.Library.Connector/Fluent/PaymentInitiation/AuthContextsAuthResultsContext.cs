// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    internal class AuthContextsAuthResultsContext :
        IPostLocalContext<AuthResult, DomesticPaymentConsentAuthContextResponse>

    {
        private readonly AuthAuthResultsPost _authAuthResultsPost;

        public AuthContextsAuthResultsContext(ISharedContext sharedContext)
        {
            _authAuthResultsPost = new AuthAuthResultsPost(sharedContext);
        }

        public Task<IFluentResponse<DomesticPaymentConsentAuthContextResponse>> PostLocalAsync(
            AuthResult publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _authAuthResultsPost.PostAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);
    }
}
