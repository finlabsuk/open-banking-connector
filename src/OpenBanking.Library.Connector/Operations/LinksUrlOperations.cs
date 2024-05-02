// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class LinksUrlOperations
{
    /// <summary>
    ///     Provided to support requests returning multiple objects where pagination via query parameters may be used to limit
    ///     size of
    ///     response. In such pagination the URL without query remains unchanged and pages are specified via query parameters.
    /// </summary>
    private readonly bool _allowLinkUrlQueryParameters;

    /// <summary>
    ///     Expected link URL without query before transformation
    /// </summary>
    private readonly Uri _expectedLinkUrlWithoutQuery;

    /// <summary>
    ///     Ignore expected link URL without query (limits validation)
    /// </summary>
    private readonly bool _ignoreExpectedUrlWithoutQuery;

    /// <summary>
    ///     Specified link URL without query after transformation
    /// </summary>
    private readonly string _publicUrlWithoutQuery;

    private LinksUrlOperations(
        Uri expectedLinkUrlWithoutQuery,
        bool ignoreExpectedUrlWithoutQuery,
        string? publicUrlWithoutQuery,
        bool allowLinkUrlQueryParameters)
    {
        _expectedLinkUrlWithoutQuery = expectedLinkUrlWithoutQuery;
        _publicUrlWithoutQuery = publicUrlWithoutQuery ?? "https://localhost/placeholder";
        _allowLinkUrlQueryParameters = allowLinkUrlQueryParameters;
        _ignoreExpectedUrlWithoutQuery = ignoreExpectedUrlWithoutQuery;
    }

    public Uri ValidateAndTransformUrl(Uri linkUrl)
    {
        if (!_ignoreExpectedUrlWithoutQuery)
        {
            int urlsMatch = Uri.Compare(
                linkUrl,
                _expectedLinkUrlWithoutQuery,
                UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path,
                UriFormat.Unescaped,
                StringComparison.InvariantCulture);

            // Check URLs without fragment and query parameters match
            if (urlsMatch != 0)
            {
                throw new InvalidOperationException(
                    $"Base component of link URL is {linkUrl} but was expected to be {_expectedLinkUrlWithoutQuery}.");
            }
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

        // Return URL with query
        return new UriBuilder(_publicUrlWithoutQuery) { Query = linkUrl.Query }.Uri;
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
        bool responseLinksMayHaveIncorrectUrlBeforeQuery =
            readWriteGetCustomBehaviour?.ResponseLinksMayHaveIncorrectUrlBeforeQuery ?? false;
        var linksUrlOperations = new LinksUrlOperations(
            expectedLinkUrl,
            responseLinksMayHaveIncorrectUrlBeforeQuery,
            transformedLinkUrlWithoutQuery,
            allowQueryParameters);
        return linksUrlOperations;
    }

    /// <summary>
    ///     For POST requests to external API, expected link URL without query formed by adding ID of newly-created object to
    ///     request URL
    /// </summary>
    /// <param name="readWritePostCustomBehaviour"></param>
    /// <param name="externalApiUrl"></param>
    /// <param name="externalApiId"></param>
    /// <returns></returns>
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
