// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class LinksUrlOperations
{
    private readonly bool _allowValidQueryParametersOnly;

    private readonly Uri _expectedLinkUrlWithoutQuery;
    private readonly string _publicUrlWithoutQuery;
    private readonly IList<string> _validQueryParameters;

    public LinksUrlOperations(
        Uri expectedLinkUrlWithoutQuery,
        string? publicUrlWithoutQuery,
        bool allowValidQueryParametersOnly,
        IList<string> validQueryParameters)
    {
        _expectedLinkUrlWithoutQuery = expectedLinkUrlWithoutQuery;
        _publicUrlWithoutQuery = publicUrlWithoutQuery ?? "https://localhost/placeholder";
        _allowValidQueryParametersOnly = allowValidQueryParametersOnly;
        _validQueryParameters = validQueryParameters;
    }

    public Uri ValidateAndTransformUrl(Uri linkUrl)
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
                $"Request URL {_expectedLinkUrlWithoutQuery} and link URL {linkUrl} have different base URLs!");
        }

        // Check there are no fragment parameters
        if (!string.IsNullOrEmpty(linkUrl.Fragment))
        {
            throw new InvalidOperationException($"Link URL {linkUrl} has unexpected fragment.");
        }

        // Check query parameters are valid
        if (_allowValidQueryParametersOnly &&
            !string.IsNullOrEmpty(linkUrl.Query))
        {
            string[] linkUrlQueryParameterPairs = linkUrl.Query.TrimStart('?').Split('&');
            foreach (string queryParameterPair in linkUrlQueryParameterPairs)
            {
                string queryParameterName = queryParameterPair.Split('=')[0];
                if (!_validQueryParameters.Contains(queryParameterName))
                {
                    throw new InvalidOperationException(
                        $"External API returned link URL with query parameter {queryParameterName} which is unexpected.");
                }
            }
        }

        // Return absolute URL
        var uriBuilder = new UriBuilder(_publicUrlWithoutQuery);
        uriBuilder.Query = linkUrl.Query;
        Uri fullUri = uriBuilder.Uri;
        return fullUri;
    }
}
