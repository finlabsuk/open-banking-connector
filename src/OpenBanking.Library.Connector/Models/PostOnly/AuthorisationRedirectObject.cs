// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using AuthorisationRedirectObjectRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.AuthorisationRedirectObject;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.PostOnly
{
    /// <summary>
    ///     "Post-only" type. Post-only types are similar to entity types but are
    ///     not persisted to DB. Their main use is to hold customisation points
    ///     for the type defined by IPostOnlyWithPublicInterface which do not belong
    ///     on public request or response types.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal class AuthorisationRedirectObject : ISupportsFluentPost<AuthorisationRedirectObjectRequest,
        AuthorisationRedirectObjectResponse>
    {
        public AuthorisationRedirectObjectResponse PublicResponse => new AuthorisationRedirectObjectResponse();

        public PostEntityAsyncWrapperDelegate<AuthorisationRedirectObjectRequest, AuthorisationRedirectObjectResponse>
            PostEntityAsyncWrapper =>
            PostEntityAsync;

        public static async Task<(AuthorisationRedirectObjectResponse response,
                IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            PostEntityAsync(
                ISharedContext context,
                AuthorisationRedirectObjectRequest request,
                string? createdBy)
        {
            PostAuthorisationRedirectObject handler = new PostAuthorisationRedirectObject(
                context.DbService.GetDbEntityMethodsClass<Bank>(),
                context.DbService.GetDbEntityMethodsClass<BankApiInformation>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.DbService
                    .GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                context.DbService
                    .GetDbEntityMethodsClass<BankRegistration>(),
                context.SoftwareStatementProfileCachedRepo);

            (AuthorisationRedirectObjectResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)
                result = await handler.PostAsync(request, createdBy);

            return result;
        }
    }
}
