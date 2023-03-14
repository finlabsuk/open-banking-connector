// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

/// <summary>
///     Fluent response for request that does not return data
/// </summary>
public interface IFluentResponse
{
    public IReadOnlyList<IFluentResponseMessage> Messages { get; }

    public bool HasInfos => Messages.OfType<FluentResponseInfoMessage>().Any();
    public bool HasWarnings => Messages.OfType<FluentResponseWarningMessage>().Any();
    public bool HasErrors => Messages.OfType<IFluentResponseErrorMessage>().Any();
}

/// <summary>
///     Fluent response for request that returns data
/// </summary>
public interface IFluentResponse<out TData> : IFluentResponse
    where TData : class
{
    public TData? Data { get; }
}

/// <summary>
///     Fluent success response for request that does not return data
/// </summary>
public sealed class FluentSuccessResponse : IFluentResponse
{
    internal FluentSuccessResponse(IReadOnlyList<IFluentSuccessResponseMessage> messages)
    {
        Messages = messages;
    }

    public IReadOnlyList<IFluentResponseMessage> Messages { get; }
}

/// <summary>
///     Fluent success response for request that returns data
/// </summary>
public sealed class FluentSuccessResponse<TData> : IFluentResponse<TData>
    where TData : class
{
    internal FluentSuccessResponse(
        TData data,
        IReadOnlyList<IFluentSuccessResponseMessage> messages)
    {
        Data = data;
        Messages = messages;
    }

    public TData? Data { get; }
    public IReadOnlyList<IFluentResponseMessage> Messages { get; }
}

/// <summary>
///     Fluent bad request error response for request that does not return data
/// </summary>
public sealed class FluentBadRequestErrorResponse : IFluentResponse
{
    internal FluentBadRequestErrorResponse(IReadOnlyList<IFluentBadRequestErrorResponseMessage> messages)
    {
        Messages = messages;
    }

    public IReadOnlyList<IFluentResponseMessage> Messages { get; }
}

/// <summary>
///     Fluent bad request error response for request that returns data
/// </summary>
public sealed class FluentBadRequestErrorResponse<TData> : IFluentResponse<TData>
    where TData : class
{
    internal FluentBadRequestErrorResponse(IReadOnlyList<IFluentBadRequestErrorResponseMessage> messages)
    {
        Messages = messages;
    }

    public TData? Data => null;
    public IReadOnlyList<IFluentResponseMessage> Messages { get; }
}

/// <summary>
///     Fluent other error response for request that does not return data
/// </summary>
public sealed class FluentOtherErrorResponse : IFluentResponse
{
    internal FluentOtherErrorResponse(IReadOnlyList<IFluentOtherErrorResponseMessage> messages)
    {
        Messages = messages;
    }

    public IReadOnlyList<IFluentResponseMessage> Messages { get; }
}

/// <summary>
///     Fluent other error response for request that returns data
/// </summary>
public sealed class FluentOtherErrorResponse<TData> : IFluentResponse<TData>
    where TData : class
{
    internal FluentOtherErrorResponse(IReadOnlyList<IFluentOtherErrorResponseMessage> messages)
    {
        Messages = messages;
    }

    public TData? Data => null;
    public IReadOnlyList<IFluentResponseMessage> Messages { get; }
}
