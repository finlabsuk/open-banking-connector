﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal static class Helpers
{
    public static string? TransformLinkUrl(
        string? linkUrlString,
        Uri apiRequestUrl,
        string? publicRequestUrlWithoutQuery,
        bool allowValidQueryParametersOnly,
        IList<string> validQueryParameters)
    {
        if (linkUrlString is null)
        {
            return null;
        }

        var linkUrl = new Uri(linkUrlString);

        int urlsMatch = Uri.Compare(
            linkUrl,
            apiRequestUrl,
            UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path,
            UriFormat.Unescaped,
            StringComparison.InvariantCulture);

        // Check URLs without fragment and query parameters match
        if (urlsMatch != 0)
        {
            throw new InvalidOperationException(
                $"Request URL {apiRequestUrl} and link URL {linkUrl} have different base URLs!");
        }

        // Check there are no fragment parameters
        if (!string.IsNullOrEmpty(linkUrl.Fragment))
        {
            throw new InvalidOperationException($"Link URL {linkUrl} has unexpected fragment.");
        }

        // Check query parameters are valid
        if (allowValidQueryParametersOnly &&
            !string.IsNullOrEmpty(linkUrl.Query))
        {
            string[] linkUrlQueryParameterPairs = linkUrl.Query.TrimStart('?').Split('&');
            foreach (string queryParameterPair in linkUrlQueryParameterPairs)
            {
                string queryParameterName = queryParameterPair.Split('=')[0];
                if (!validQueryParameters.Contains(queryParameterName))
                {
                    throw new InvalidOperationException(
                        $"External API returned link URL with query parameter {queryParameterName} which is unexpected.");
                }
            }
        }

        // Return relative URL
        if (publicRequestUrlWithoutQuery is null)
        {
            return linkUrl.Query;
        }

        // Return absolute URL
        var uriBuilder = new UriBuilder(publicRequestUrlWithoutQuery);
        uriBuilder.Query = linkUrl.Query;
        Uri fullUri = uriBuilder.Uri;
        return fullUri.ToString();
    }
}