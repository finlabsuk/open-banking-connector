// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

/// <summary>
///     Fluent context for entity created in external (i.e. bank) database only.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
public interface IReadOnlyExternalEntityContext<TPublicResponse> :
    IReadAccountAccessConsentExternalEntityContext<TPublicResponse>
    where TPublicResponse : class { }

internal class ReadOnlyExternalEntityContext<TPublicResponse> :
    IReadOnlyExternalEntityContext<TPublicResponse>
    where TPublicResponse : class
{
    public ReadOnlyExternalEntityContext(
        IAccountAccessConsentExternalRead<TPublicResponse, AccountAccessConsentExternalReadParams> readObject)
    {
        ReadObject = readObject;
    }

    public IAccountAccessConsentExternalRead<TPublicResponse, AccountAccessConsentExternalReadParams> ReadObject
    {
        get;
    }
}
