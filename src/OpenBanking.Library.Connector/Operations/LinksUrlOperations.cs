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
    ///     Expected link URLs without query before transformation
    /// </summary>
    private readonly IList<Uri> _expectedLinkUrlsWithoutQuery;

    /// <summary>
    ///     Ignore expected link URL without query (limits validation)
    /// </summary>
    private readonly bool _ignoreExpectedUrlWithoutQuery;

    /// <summary>
    ///     Specified link URL without query after transformation
    /// </summary>
    private readonly string _publicUrlWithoutQuery;

    private LinksUrlOperations(
        IList<Uri> expectedLinkUrlsWithoutQuery,
        bool ignoreExpectedUrlWithoutQuery,
        string? publicUrlWithoutQuery,
        bool allowLinkUrlQueryParameters)
    {
        _expectedLinkUrlsWithoutQuery = expectedLinkUrlsWithoutQuery;
        _publicUrlWithoutQuery = publicUrlWithoutQuery ?? "https://localhost/placeholder";
        _allowLinkUrlQueryParameters = allowLinkUrlQueryParameters;
        _ignoreExpectedUrlWithoutQuery = ignoreExpectedUrlWithoutQuery;
    }

    public Uri ValidateAndTransformUrl(Uri linkUrl)
    {
        // Validate URL against expected URLs
        if (!_ignoreExpectedUrlWithoutQuery)
        {
            var matchFound = false;
            foreach (Uri expectedUrl in _expectedLinkUrlsWithoutQuery)
            {
                int urlsMatch = Uri.Compare(
                    linkUrl,
                    expectedUrl,
                    UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path,
                    UriFormat.Unescaped,
                    StringComparison.InvariantCulture);
                if (urlsMatch == 0)
                {
                    matchFound = true;
                    break;
                }
            }

            // Check URLs without fragment and query parameters match
            if (!matchFound)
            {
                throw new InvalidOperationException(
                    $"Base component of link URL is {linkUrl} but was expected to be one of {_expectedLinkUrlsWithoutQuery}.");
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
        IList<Uri> expectedLinkUrls,
        string? transformedLinkUrlWithoutQuery,
        bool responseLinksMayHaveIncorrectUrlBeforeQuery,
        bool allowQueryParameters)
    {
        var linksUrlOperations = new LinksUrlOperations(
            expectedLinkUrls,
            responseLinksMayHaveIncorrectUrlBeforeQuery,
            transformedLinkUrlWithoutQuery,
            allowQueryParameters);
        return linksUrlOperations;
    }

    public static IList<Uri> GetMethodExpectedLinkUrls(
        Uri expectedLinkUrl,
        ReadWriteGetCustomBehaviour? readWriteGetCustomBehaviour)
    {
        List<Uri> expectedLinkUrls = [expectedLinkUrl];
        bool responseLinksMayAddSlash = readWriteGetCustomBehaviour?.ResponseLinksMayAddSlash ?? false;
        if (responseLinksMayAddSlash)
        {
            expectedLinkUrls.Add(new Uri(expectedLinkUrl + "/"));
        }
        (string oldValue, string newValue)? responseLinksAllowReplace =
            readWriteGetCustomBehaviour?.ResponseLinksAllowReplace;
        if (responseLinksAllowReplace is not null)
        {
            (string oldValue, string newValue) = responseLinksAllowReplace.Value;
            expectedLinkUrls.Add(new Uri($"{expectedLinkUrl}".Replace(oldValue, newValue)));
        }
        return expectedLinkUrls;
    }

    public static IList<Uri> PostMethodExpectedLinkUrls(
        Uri externalApiUrl,
        string externalApiId,
        ReadWritePostCustomBehaviour? readWritePostCustomBehaviour)
    {
        var expectedLinkUrl = new Uri(externalApiUrl + $"/{externalApiId}");
        IList<Uri> expectedLinkUrls = GetMethodExpectedLinkUrls(
            expectedLinkUrl,
            readWritePostCustomBehaviour);
        bool responseLinksMayOmitId = readWritePostCustomBehaviour?.ResponseLinksMayOmitId ?? false;
        if (responseLinksMayOmitId)
        {
            expectedLinkUrls.Add(externalApiUrl);
        }
        return expectedLinkUrls;
    }
}
