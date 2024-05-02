// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal interface IObjectUpdate<in TPublicRequest, TPublicResponse>
{
    Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> CreateAsync(
        TPublicRequest request,
        string? modifiedBy);
}

internal interface IObjectUpdate2<in TPublicRequest, TPublicResponse>
{
    Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        UpdateAsync(
            TPublicRequest request,
            LocalReadParams readParams);
}
