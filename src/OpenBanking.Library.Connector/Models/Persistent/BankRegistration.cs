// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using BankRegistrationRequest = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankRegistration;
using OAuth2RequestObjectClaimsOverridesRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.OAuth2RequestObjectClaimsOverrides;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    internal class BankRegistration :
        EntityBase,
        ISupportsFluentPost<BankRegistrationRequest, BankRegistrationPostResponse>,
        ISupportsFluentGetLocal<BankRegistration, IBankRegistrationPublicQuery, BankRegistrationGetLocalResponse>,
        ISupportsFluentDeleteLocal<BankRegistration>,
        IBankRegistrationPublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access static methods in generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public BankRegistration() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        public BankRegistration(
            string softwareStatementProfileId,
            OpenIdConfiguration openIdConfiguration,
            ClientRegistrationModelsPublic.OBClientRegistration1 obClientRegistration,
            Guid bankId,
            ClientRegistrationModelsPublic.OBClientRegistration1 obClientRegistrationRequest,
            OAuth2RequestObjectClaimsOverridesRequest? claimsOverrides,
            ClientRegistrationApiVersion clientRegistrationApi,
            RegistrationScope registrationScope,
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider) : base(id, name, createdBy, timeProvider)
        {
            ClientRegistrationApi = clientRegistrationApi;
            RegistrationScope = registrationScope;
            SoftwareStatementProfileId = softwareStatementProfileId;
            OpenIdConfiguration = openIdConfiguration;
            OBClientRegistrationRequest = obClientRegistrationRequest;
            OBClientRegistration = obClientRegistration;
            OAuth2RequestObjectClaimsOverrides = claimsOverrides;
            BankId = bankId;
        }

        public string SoftwareStatementProfileId { get; set; } = null!;

        /// <summary>
        ///     Functional APIs used for bank registration.
        /// </summary>
        public RegistrationScope RegistrationScope { get; set; }

        public ClientRegistrationApiVersion ClientRegistrationApi { get; set; }

        public OpenIdConfiguration OpenIdConfiguration { get; set; } = null!;

        public ClientRegistrationModelsPublic.OBClientRegistration1 OBClientRegistration { get; set; } = null!;

        public OAuth2RequestObjectClaimsOverridesRequest? OAuth2RequestObjectClaimsOverrides { get; set; }

        public BankRegistrationPostResponse PublicPostResponse =>
            new BankRegistrationPostResponse(
                Id,
                OBClientRegistrationRequest,
                BankId);

        /// <summary>
        ///     Bank this registration is with.
        /// </summary>
        public Guid BankId { get; set; }

        public ClientRegistrationModelsPublic.OBClientRegistration1 OBClientRegistrationRequest { get; set; } = null!;

        public BankRegistrationGetLocalResponse PublicGetLocalResponse =>
            new BankRegistrationGetLocalResponse(
                Id,
                OBClientRegistrationRequest,
                BankId);

        public PostEntityAsyncWrapperDelegate<BankRegistrationRequest, BankRegistrationPostResponse>
            PostEntityAsyncWrapper =>
            PostEntityAsync;

        public static async Task<(BankRegistrationPostResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            PostEntityAsync(
                ISharedContext context,
                BankRegistrationRequest request,
                string? createdBy)
        {
            PostBankRegistration i = new PostBankRegistration(
                context.ApiClient,
                context.DbService.GetDbEntityMethodsClass<BankRegistration>(),
                context.DbService.GetDbEntityMethodsClass<Bank>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation,
                context.ApiVariantMapper);

            (BankRegistrationPostResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages) result =
                await i.PostAsync(request, createdBy);
            return result;
        }
    }
}
