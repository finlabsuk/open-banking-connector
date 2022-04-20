// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    /// <summary>
    ///     Fluent Response message
    /// </summary>
    public interface IFluentResponseMessage
    {
        public string Message { get; }
    }

    /// <summary>
    ///     Fluent response message valid for inclusion in <see cref="FluentSuccessResponse{TData}" />
    /// </summary>
    public interface IFluentSuccessResponseMessage : IFluentResponseMessage { }

    /// <summary>
    ///     Fluent response message valid for inclusion in <see cref="FluentBadRequestErrorResponse{TData}" />
    /// </summary>
    public interface IFluentBadRequestErrorResponseMessage : IFluentResponseMessage { }

    /// <summary>
    ///     Fluent response message valid for inclusion in <see cref="FluentOtherErrorResponse{TData}" />
    /// </summary>
    public interface IFluentOtherErrorResponseMessage : IFluentResponseMessage { }

    /// <summary>
    ///     Fluent response info or warning message
    /// </summary>
    public interface IFluentResponseInfoOrWarningMessage : IFluentSuccessResponseMessage,
        IFluentBadRequestErrorResponseMessage, IFluentOtherErrorResponseMessage { }

    /// <summary>
    ///     Fluent response error message
    /// </summary>
    public interface IFluentResponseErrorMessage : IFluentResponseMessage { }

    /// <summary>
    ///     Shared implementation for Fluent response messages
    /// </summary>
    public abstract class FluentResponseMessage : IFluentResponseMessage
    {
        protected FluentResponseMessage(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public static FluentResponseInfoMessage Info(string message)
        {
            return new FluentResponseInfoMessage(message);
        }

        public static FluentResponseWarningMessage Warning(string message)
        {
            return new FluentResponseWarningMessage(message);
        }

        public static FluentResponseBadRequestErrorMessage BadRequestError(string message)
        {
            return new FluentResponseBadRequestErrorMessage(message);
        }

        public static FluentResponseOtherErrorMessage OtherError(string message)
        {
            return new FluentResponseOtherErrorMessage(message);
        }
    }

    /// <summary>
    ///     Fluent response info message. This message does not terminate execution of request.
    /// </summary>
    public class FluentResponseInfoMessage : FluentResponseMessage, IFluentResponseInfoOrWarningMessage
    {
        internal FluentResponseInfoMessage(string message) : base(message) { }
    }

    /// <summary>
    ///     Fluent response warning message. This message does not terminate execution of request.
    /// </summary>
    public class FluentResponseWarningMessage : FluentResponseMessage, IFluentResponseInfoOrWarningMessage
    {
        internal FluentResponseWarningMessage(string message) : base(message) { }
    }

    /// <summary>
    ///     Fluent response bad request error message. Indicates issue with user request data passed to
    ///     Open Banking Connector. This message does terminate execution of request.
    /// </summary>
    public class FluentResponseBadRequestErrorMessage : FluentResponseMessage, IFluentResponseErrorMessage,
        IFluentBadRequestErrorResponseMessage
    {
        internal FluentResponseBadRequestErrorMessage(string message) : base(message) { }
    }

    /// <summary>
    ///     Fluent response other error message. Indicates internal issue unrelated to user request data passed to
    ///     Open Banking Connector. This message does terminate execution of request.
    /// </summary>
    public class FluentResponseOtherErrorMessage : FluentResponseMessage, IFluentResponseErrorMessage,
        IFluentOtherErrorResponseMessage
    {
        internal FluentResponseOtherErrorMessage(string message) : base(message) { }

        internal FluentResponseOtherErrorMessage(Exception exception) : this(message: CombineMessages(exception)) { }

        public static string CombineMessages(Exception exception)
        {
            IEnumerable<string> x = exception
                .WalkRecursive(e => e.InnerException)
                .Select(e => e.Message);
            return x.JoinString(Environment.NewLine);
        }
    }
}
