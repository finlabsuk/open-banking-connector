// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public abstract class FluentResponseMessage
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

        public static FluentResponseErrorMessage Error(string message)
        {
            return new FluentResponseErrorMessage(message);
        }
    }

    public sealed class FluentResponseInfoMessage : FluentResponseMessage
    {
        internal FluentResponseInfoMessage(string message) : base(message) { }
    }

    public sealed class FluentResponseWarningMessage : FluentResponseMessage
    {
        internal FluentResponseWarningMessage(string message) : base(message) { }
    }

    public sealed class FluentResponseErrorMessage : FluentResponseMessage
    {
        internal FluentResponseErrorMessage(string message) : base(message) { }

        internal FluentResponseErrorMessage(Exception exception)
            : base(
                exception.WalkRecursive(e => e.InnerException).Select(e => e.Message)
                    .JoinString(Environment.NewLine)) { }
    }
}
