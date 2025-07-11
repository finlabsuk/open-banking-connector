﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FluentValidation;
using FluentValidation.Results;
using AuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

public interface IAuthContextsContext
{
    /// <summary>
    ///     Update auth context with auth result which is data received from bank (e.g. via redirect) following user
    ///     authorisation of consent.
    /// </summary>
    Task<AuthContextUpdateAuthResultResponse> UpdateAuthResultAsync(
        AuthResult authResult,
        string? createdBy = null);
}

internal class AuthContextsContext : IAuthContextsContext
{
    public AuthContextsContext(ISharedContext sharedContext)
    {
        UpdateLocalObject = new AuthContextUpdate(
            sharedContext.DbService.GetDbMethods(),
            sharedContext.TimeProvider,
            sharedContext.DbService.GetDbEntityMethods<AuthContextPersisted>(),
            sharedContext.Instrumentation,
            new GrantPost(
                sharedContext.ApiClient,
                sharedContext.Instrumentation,
                sharedContext.MemoryCache,
                sharedContext.TimeProvider),
            sharedContext.BankProfileService,
            sharedContext.MemoryCache,
            sharedContext.EncryptionKeyInfo,
            sharedContext.ObWacCertificateMethods,
            sharedContext.ObSealCertificateMethods);
    }

    public IObjectUpdate<AuthResult, AuthContextUpdateAuthResultResponse> UpdateLocalObject { get; }

    public async Task<AuthContextUpdateAuthResultResponse> UpdateAuthResultAsync(
        AuthResult authResult,
        string? createdBy = null)
    {
        authResult.ArgNotNull(nameof(authResult));

        // Validate request data and convert to messages
        ValidationResult validationResult = await authResult.ValidateAsync();
        if (validationResult.Errors.Any(failure => failure.Severity == Severity.Error))
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Execute operation catching errors 
        (AuthContextUpdateAuthResultResponse response,
                IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await UpdateLocalObject.CreateAsync(
                authResult,
                createdBy);

        return response;
    }
}
