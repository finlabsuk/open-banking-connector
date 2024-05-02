// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Management;

public interface ISoftwareStatementsContext :
    ILocalEntityContext<SoftwareStatement, ISoftwareStatementPublicQuery,
        SoftwareStatementResponse, SoftwareStatementResponse>,
    IUpdateContext<SoftwareStatementUpdate, SoftwareStatementResponse> { }

internal interface ISoftwareStatementsContextInternal : ISoftwareStatementsContext,
    ILocalEntityContextInternal<SoftwareStatement, ISoftwareStatementPublicQuery,
        SoftwareStatementResponse, SoftwareStatementResponse>,
    IUpdateContextInternal<SoftwareStatementUpdate, SoftwareStatementResponse> { }

internal class SoftwareStatementsContextInternal : LocalEntityContextInternal<SoftwareStatementEntity, SoftwareStatement
        , ISoftwareStatementPublicQuery,
        SoftwareStatementResponse, SoftwareStatementResponse>,
    ISoftwareStatementsContextInternal
{
    public SoftwareStatementsContextInternal(
        ISharedContext sharedContext,
        IObjectCreate<SoftwareStatement, SoftwareStatementResponse, LocalCreateParams> postObject,
        IObjectUpdate2<SoftwareStatementUpdate, SoftwareStatementResponse> updateObject) : base(
        sharedContext,
        postObject)
    {
        UpdateObject = updateObject;
    }

    public IObjectUpdate2<SoftwareStatementUpdate, SoftwareStatementResponse> UpdateObject { get; }
}
