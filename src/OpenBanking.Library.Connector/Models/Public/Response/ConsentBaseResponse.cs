// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

public interface IConsentPublicQuery : IEntityBaseQuery
{
    /// <summary>
    ///     Associated BankRegistration object
    /// </summary>
    Guid BankRegistrationId { get; }

    /// <summary>
    ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    string ExternalApiId { get; }
}

public class ConsentBaseResponse : EntityBaseResponse, IConsentPublicQuery
{
    /// <summary>
    ///     User ID at external API (bank) which may or may not be available via ID token "sub" claim. If retrieved from ID
    ///     token or supplied on object creation, it will be stored here.
    /// </summary>
    public string? ExternalApiUserId { get; init; }

    public required DateTimeOffset AuthContextModified { get; init; }

    public string? AuthContextModifiedBy { get; init; }

    /// <summary>
    ///     ID of associated BankRegistration object
    /// </summary>
    public required Guid BankRegistrationId { get; init; }

    /// <summary>
    ///     External (bank) API ID. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    public required string ExternalApiId { get; init; }
}
