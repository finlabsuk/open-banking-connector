// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FluentValidation.Results;
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
    internal class AuthorisationRedirectObject : IPostOnlyWithPublicInterface<AuthorisationRedirectObject,
        AuthorisationRedirectObjectRequest, AuthorisationRedirectObjectResponse>
    {
        public Func<AuthorisationRedirectObjectRequest, ValidationResult> ValidatePublicRequestWrapper =>
            ValidatePublicRequest;

        public Func<ISharedContext, AuthorisationRedirectObjectRequest, string?,
                Task<AuthorisationRedirectObjectResponse>>
            PostEntityAsyncWrapper =>
            PostEntityAsync;

        public static ValidationResult ValidatePublicRequest(AuthorisationRedirectObjectRequest request)
        {
            return new AuthorisationRedirectObjectValidator()
                .Validate(request);
        }

        public static async Task<AuthorisationRedirectObjectResponse> PostEntityAsync(
            ISharedContext context,
            AuthorisationRedirectObjectRequest request,
            string? createdBy)
        {
            PostAuthorisationRedirectObject handler = new PostAuthorisationRedirectObject(
                bankRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<Bank>(),
                bankProfileRepo: context.DbEntityRepositoryFactory.CreateDbEntityRepository<BankApiInformation>(),
                dbContextService: context.DbContextService,
                domesticConsentRepo: context.DbEntityRepositoryFactory
                    .CreateDbEntityRepository<DomesticPaymentConsent>(),
                bankRegistrationRepo: context.DbEntityRepositoryFactory
                    .CreateDbEntityRepository<BankRegistration>(),
                softwareStatementProfileRepo: context.SoftwareStatementProfileCachedRepo);

            AuthorisationRedirectObjectResponse? resp = await handler.PostAsync(request);

            return resp;
        }
    }
}
