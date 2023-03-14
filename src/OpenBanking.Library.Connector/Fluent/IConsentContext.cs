// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

/// <summary>
///     Fluent context for entity created both in local and external (i.e. bank) database.
/// </summary>
/// <typeparam name="TPublicRequest"></typeparam>
/// <typeparam name="TPublicReadResponse"></typeparam>
/// <typeparam name="TPublicCreateResponse"></typeparam>
public interface IConsentContext<in TPublicRequest, TPublicCreateResponse, TPublicReadResponse> :
    ICreateConsentContext<TPublicRequest, TPublicCreateResponse>,
    IReadConsentContext<TPublicReadResponse>
    where TPublicReadResponse : class
    where TPublicCreateResponse : class { }

internal interface IConsentContextInternal<in TPublicRequest, TPublicCreateResponse, TPublicReadResponse> :
    IConsentContext<TPublicRequest, TPublicCreateResponse, TPublicReadResponse>,
    ICreateConsentContextInternal<TPublicRequest, TPublicCreateResponse>,
    IReadConsentContextInternal<TPublicReadResponse>
    where TPublicRequest : class, ISupportsValidation
    where TPublicReadResponse : class
    where TPublicCreateResponse : class { }
