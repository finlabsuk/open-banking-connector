// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

/// <summary>
///     Create operations on local entities (objects stored in local database only).
/// </summary>
/// <typeparam name="TPublicRequest"></typeparam>
/// <typeparam name="TPublicResponse"></typeparam>
/// <typeparam name="TCreateParams"></typeparam>
internal abstract class BaseCreate<TPublicRequest, TPublicResponse, TCreateParams> :
    IObjectCreate<TPublicRequest, TPublicResponse, TCreateParams>
    where TPublicRequest : EntityBase
    where TCreateParams : LocalCreateParams
{
    protected readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    protected readonly IInstrumentationClient _instrumentationClient;
    protected readonly ITimeProvider _timeProvider;


    public BaseCreate(
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient)
    {
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
    }

    public async Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateAsync(
            TPublicRequest request,
            TCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // POST to bank API and create entity
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await ApiPost(request, createParams);
        nonErrorMessages.AddRange(newNonErrorMessages);

        return (response, nonErrorMessages);
    }

    /// <summary>
    ///     Empty function as by definition POST local does not include POST to bank API.
    /// </summary>
    /// <returns></returns>
    protected abstract Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)>
        ApiPost(
            TPublicRequest request,
            TCreateParams createParams);
}
