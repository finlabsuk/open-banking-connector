// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using BankApiSetPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankApiSet;
using BankRegistrationPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal abstract class
        ReadWritePost<TEntity, TPublicRequest, TPublicResponse, TApiRequest, TApiResponse> : EntityPost<
            TEntity,
            TPublicRequest, TPublicResponse, TApiRequest, TApiResponse>
        where TEntity : class, IEntity,
        new()
        where TPublicRequest : Base
        where TApiRequest : class, ISupportsValidation
        where TApiResponse : class, ISupportsValidation
    {
        public ReadWritePost(
            IDbReadWriteEntityMethods<TEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper) { }

        protected abstract string RelativePath { get; }

        protected abstract string ClientCredentialsGrantScope { get; }

        protected abstract Task<TPublicResponse> CreateLocalEntity(
            TPublicRequest request,
            TApiRequest apiRequest,
            TApiResponse apiResponse,
            string? createdBy,
            ITimeProvider timeProvider);

        protected abstract IApiPostRequests<TApiRequest, TApiResponse> ApiRequests(
            BankApiSetPersisted bankApiSet,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient);

        // protected virtual void WriteObject(Utf8JsonWriter jsonWriter, TApiRequest apiRequest) { }
        //
        // protected virtual void WriteObject(Utf8JsonWriter jsonWriter, TApiResponse apiResponse) { }
        //
        //
        // protected virtual TApiResponse ReadObject(JsonDocument jsonDocument)
        // {
        //     throw new NotImplementedException();
        // }

        protected override async
            Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiPost(PostRequestInfo requestInfo)
        {
            (
                    TApiRequest apiRequest,
                    Uri endpointUrl,
                    BankApiSetPersisted bankApiInformation,
                    BankRegistrationPersisted bankRegistration,
                    string bankFinancialId,
                    TokenEndpointResponse? userTokenEndpointResponse,
                    List<IFluentResponseInfoOrWarningMessage> nonErrorMessages) =
                await ApiPostRequestData(requestInfo.Request);

            // Get software statement profile
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementAndCertificateProfileOverrideCase);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get client credentials grant token if necessary
            TokenEndpointResponse tokenEndpointResponse =
                userTokenEndpointResponse ??
                await PostTokenRequest.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    null,
                    apiClient,
                    _instrumentationClient);

            // Create new Open Banking object by posting JWT
            JsonSerializerSettings? requestJsonSerializerSettings = null;
            JsonSerializerSettings? responseJsonSerializerSettings = null;

            IApiPostRequests<TApiRequest, TApiResponse> apiRequests =
                ApiRequests(
                    bankApiInformation,
                    bankFinancialId,
                    tokenEndpointResponse,
                    processedSoftwareStatementProfile,
                    _instrumentationClient);


            // // JSON write
            // string json;
            // {
            //     var options = new JsonWriterOptions
            //     {
            //         Indented = true
            //     };
            //     using var stream = new MemoryStream();
            //     using var writer = new Utf8JsonWriter(stream, options);
            //     WriteObject(writer, apiRequest);
            //     writer.Flush();
            //     json = Encoding.UTF8.GetString(stream.ToArray());
            //     Console.WriteLine("JSON 1");
            //     Console.WriteLine(json);
            // }
            //
            // // JSON read
            // var documentOptions = new JsonDocumentOptions
            // {
            //     CommentHandling = JsonCommentHandling.Skip
            // };
            //
            // using JsonDocument document = JsonDocument.Parse(json, documentOptions);
            // TApiResponse apiResponse1 = ReadObject(document);
            //
            // Console.WriteLine("Done");
            //
            // throw new Exception("Stop");


            (TApiResponse apiResponse, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages2) =
                await EntityPostCommon(
                    requestInfo,
                    apiRequest,
                    apiRequests,
                    apiClient,
                    endpointUrl,
                    requestJsonSerializerSettings,
                    responseJsonSerializerSettings,
                    nonErrorMessages);

            // Create persisted entity
            TPublicResponse response = await CreateLocalEntity(
                requestInfo.Request,
                apiRequest,
                apiResponse,
                requestInfo.ModifiedBy,
                _timeProvider);

            // Persist updates (this happens last so as not to happen if there are any previous errors)
            await _dbSaveChangesMethod.SaveChangesAsync();

            return (response, nonErrorMessages2);
        }

        protected abstract
            Task<(
                TApiRequest apiRequest,
                Uri endpointUrl,
                BankApiSetPersisted bankApiInformation,
                BankRegistrationPersisted bankRegistration,
                string bankFinancialId,
                TokenEndpointResponse? userTokenEndpointResponse,
                List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiPostRequestData(TPublicRequest request);
    }
}
