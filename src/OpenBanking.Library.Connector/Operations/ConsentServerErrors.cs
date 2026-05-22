// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal enum ConsentIdSource
{
    UrlPath,
    RequestHeader,
    RequestBody,
    DatabaseForeignKey
}

internal enum ConsentIdRequestSource
{
    RequestHeader,
    RequestBody
}

internal record ConsentNotFoundServerError(ConsentType ConsentType, Guid ConsentId) : ServerError
{
    public override ServerErrorType ServerErrorType => ServerErrorType.ConsentNotFound;
    public override string Title => "Consent not found";

    public override string Detail =>
        $"No record found for {ConsentType.ToString().FromPascalCaseToLowerWords()} with ID {ConsentId}.";

    public override int StatusCode => 404;

    public override IReadOnlyDictionary<string, object?> Extensions { get; } =
        new Dictionary<string, object?>
        {
            ["consentType"] = ConsentType.ToString().ToCamelCase(),
            ["consentId"] = ConsentId
        };
}

internal record ConsentFromRequestNotFoundServerError(
    ConsentType ConsentType,
    Guid ConsentId,
    ConsentIdRequestSource Source) : ServerError
{
    public override ServerErrorType ServerErrorType => ServerErrorType.ConsentFromRequestNotFound;
    public override string Title => "Consent from request not found";

    public override string Detail =>
        $"No record found for {ConsentType.ToString().FromPascalCaseToLowerWords()} with ID {ConsentId}.";

    public override int StatusCode => 400;

    public override IReadOnlyDictionary<string, object?> Extensions { get; } =
        new Dictionary<string, object?>
        {
            ["consentType"] = ConsentType.ToString().ToCamelCase(),
            ["consentId"] = ConsentId,
            ["consentIdSource"] = Source.ToString().ToCamelCase()
        };
}

internal static class ConsentServerErrors
{
    internal static Exception ConsentNotFoundException(
        ConsentType consentType,
        ConsentIdSource source,
        Guid id) =>
        source switch
        {
            ConsentIdSource.UrlPath =>
                new HttpResponseException(new ConsentNotFoundServerError(consentType, id)),
            ConsentIdSource.RequestHeader =>
                new HttpResponseException(
                    new ConsentFromRequestNotFoundServerError(consentType, id, ConsentIdRequestSource.RequestHeader)),
            ConsentIdSource.RequestBody =>
                new HttpResponseException(
                    new ConsentFromRequestNotFoundServerError(consentType, id, ConsentIdRequestSource.RequestBody)),
            ConsentIdSource.DatabaseForeignKey =>
                new InvalidOperationException("Invalid foreign key"),
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
}
