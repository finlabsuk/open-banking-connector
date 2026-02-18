// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Management;

public interface ISoftwareStatementsContext :
    ILocalEntityContext<SoftwareStatement, ISoftwareStatementPublicQuery,
        SoftwareStatementResponse, SoftwareStatementResponse>,
    IReadAllContext<SoftwareStatementsResponse>,
    IUpdateContext<SoftwareStatementUpdate, SoftwareStatementResponse> { }

internal class SoftwareStatementsContext : LocalEntityContext<SoftwareStatementEntity, SoftwareStatement
        , ISoftwareStatementPublicQuery,
        SoftwareStatementResponse, SoftwareStatementResponse>,
    ISoftwareStatementsContext
{
    public SoftwareStatementsContext(
        ISharedContext sharedContext,
        IObjectCreate<SoftwareStatement, SoftwareStatementResponse, LocalCreateParams> postObject) : base(
        sharedContext,
        postObject)
    {
        var softwareStatementOperations = new SoftwareStatementOperations(
            sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
            sharedContext.DbService.GetDbMethods(),
            sharedContext.TimeProvider,
            sharedContext.Instrumentation);
        UpdateObject = softwareStatementOperations;
        ReadAllObject = softwareStatementOperations;
    }

    public IObjectUpdate2<SoftwareStatementUpdate, SoftwareStatementResponse> UpdateObject { get; }

    public IObjectReadAll<SoftwareStatementsResponse, LocalReadAllParams> ReadAllObject { get; }
}
