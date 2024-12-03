// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;

public interface IEncryptionKeyDescriptionPublicQuery : IEntityBaseQuery
{
    public SecretDescription Key { get; }
}

/// <summary>
///     Response to EncryptionKeyDescription read and create requests.
/// </summary>
public class EncryptionKeyDescriptionResponse : EntityBaseResponse, IEncryptionKeyDescriptionPublicQuery
{
    public required SecretDescription Key { get; init; }
}
