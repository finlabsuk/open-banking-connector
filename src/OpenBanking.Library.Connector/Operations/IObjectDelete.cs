// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class LocalDeleteParams
{
    public LocalDeleteParams(Guid id, string? modifiedBy)
    {
        Id = id;
        ModifiedBy = modifiedBy;
    }

    public Guid Id { get; }
    public string? ModifiedBy { get; }
}

internal class BankRegistrationDeleteParams : LocalDeleteParams
{
    public BankRegistrationDeleteParams(
        Guid id,
        string? modifiedBy,
        bool? includeExternalApiOperation,
        bool? useRegistrationAccessToken) : base(id, modifiedBy)
    {
        IncludeExternalApiOperation = includeExternalApiOperation;
        UseRegistrationAccessToken = useRegistrationAccessToken;
    }

    public bool? IncludeExternalApiOperation { get; }
    public bool? UseRegistrationAccessToken { get; }
}

internal class ConsentDeleteParams : LocalDeleteParams
{
    public ConsentDeleteParams(
        Guid id,
        string? modifiedBy,
        bool includeExternalApiOperation) : base(id, modifiedBy)
    {
        IncludeExternalApiOperation = includeExternalApiOperation;
    }

    public bool IncludeExternalApiOperation { get; }
}

internal interface IObjectDelete<in TDeleteParams>
    where TDeleteParams : LocalDeleteParams
{
    Task<IList<IFluentResponseInfoOrWarningMessage>> DeleteAsync(TDeleteParams deleteParams);
}
