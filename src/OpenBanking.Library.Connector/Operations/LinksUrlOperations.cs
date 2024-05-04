// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class LinksUrlOperations
{
    private readonly bool _allowLinkUrlQueryParameters;

    private readonly Uri _expectedLinkUrl;
    private readonly string _publicUrlWithoutQuery;
    private readonly IList<string> _validQueryParameters;

    public LinksUrlOperations(
        Uri expectedLinkUrl,
        string? publicUrlWithoutQuery,
        bool allowLinkUrlQueryParameters,
        IList<string> validQueryParameters)
    {
        _expectedLinkUrl = expectedLinkUrl;
        _publicUrlWithoutQuery = publicUrlWithoutQuery ?? "https://localhost/placeholder";
        _allowLinkUrlQueryParameters = allowLinkUrlQueryParameters;
        _validQueryParameters = validQueryParameters;
    }

    public Uri ValidateAndTransformUrl(Uri linkUrl)
    {
        int urlsMatch = Uri.Compare(
            linkUrl,
            _expectedLinkUrl,
            UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path,
            UriFormat.Unescaped,
            StringComparison.InvariantCulture);

        // Check URLs without fragment and query parameters match
        if (urlsMatch != 0)
        {
            throw new InvalidOperationException(
                $"Request URL {_expectedLinkUrl} and link URL {linkUrl} have different base URLs!");
        }

        // Check there are no fragment parameters
        if (!string.IsNullOrEmpty(linkUrl.Fragment))
        {
            throw new InvalidOperationException($"Link URL {linkUrl} has unexpected fragment.");
        }

        // Check there are no query parameters unless allowed
        if (!string.IsNullOrEmpty(linkUrl.Query))
        {
            if (!_allowLinkUrlQueryParameters)
            {
                throw new InvalidOperationException(
                    $"External API returned link URL with query parameters {linkUrl.Query.TrimStart('?')} which is unexpected.");
            }
        }

        // Return absolute URL
        var uriBuilder = new UriBuilder(_publicUrlWithoutQuery);
        uriBuilder.Query = linkUrl.Query;
        Uri fullUri = uriBuilder.Uri;
        return fullUri;
    }

    public static LinksUrlOperations CreateLinksUrlOperations(
        Uri expectedLinkUrl,
        string? transformedLinkUrlWithoutQuery,
        ReadWriteGetCustomBehaviour? readWriteGetCustomBehaviour,
        bool allowQueryParameters)
    {
        bool responseLinksAddSlash = readWriteGetCustomBehaviour?.ResponseLinksAddSlash ?? false;
        if (responseLinksAddSlash)
        {
            expectedLinkUrl = new Uri(expectedLinkUrl + "/");
        }
        var linksUrlOperations = new LinksUrlOperations(
            expectedLinkUrl,
            transformedLinkUrlWithoutQuery,
            allowQueryParameters,
            new List<string>());
        return linksUrlOperations;
    }

    public static Uri GetExpectedLinkUrlWithoutQuery(
        ReadWritePostCustomBehaviour? readWritePostCustomBehaviour,
        Uri externalApiUrl,
        string externalApiId)
    {
        bool responseLinksOmitId = readWritePostCustomBehaviour?.ResponseLinksOmitId ?? false;
        Uri expectedLinkUrlWithoutQuery = responseLinksOmitId
            ? externalApiUrl
            : new Uri(externalApiUrl + $"/{externalApiId}");
        return expectedLinkUrlWithoutQuery;
    }
}
