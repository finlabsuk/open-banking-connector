// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;

/// <summary>
///     HTTP response for request that does not return data
/// </summary>
public class HttpResponse
{
    public HttpResponse(HttpResponseMessages? messages)
    {
        Messages = messages;
    }

    /// <summary>
    ///     Messages from Open Banking Connector.
    /// </summary>
    public HttpResponseMessages? Messages { get; }
}

/// <summary>
///     HTTP response for request that returns data
/// </summary>
public class HttpResponse<TData>
    where TData : class
{
    public HttpResponse(HttpResponseMessages? messages, TData? data)
    {
        Messages = messages;
        Data = data;
    }

    /// <summary>
    ///     Messages from Open Banking Connector.
    /// </summary>
    public HttpResponseMessages? Messages { get; }

    /// <summary>
    ///     Data from Open Banking Connector.
    /// </summary>
    public TData? Data { get; }
}
