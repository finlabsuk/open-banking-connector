// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public abstract class OpenBankingResponseMessage
    {
        protected OpenBankingResponseMessage(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public static OpenBankingResponseInfoMessage Info(string message)
        {
            return new OpenBankingResponseInfoMessage(message);
        }

        public static OpenBankingResponseWarningMessage Warning(string message)
        {
            return new OpenBankingResponseWarningMessage(message);
        }

        public static OpenBankingResponseErrorMessage Error(string message)
        {
            return new OpenBankingResponseErrorMessage(message);
        }
    }

    public sealed class OpenBankingResponseInfoMessage : OpenBankingResponseMessage
    {
        internal OpenBankingResponseInfoMessage(string message) : base(message)
        {
        }
    }

    public sealed class OpenBankingResponseWarningMessage : OpenBankingResponseMessage
    {
        internal OpenBankingResponseWarningMessage(string message) : base(message)
        {
        }
    }

    public sealed class OpenBankingResponseErrorMessage : OpenBankingResponseMessage
    {
        internal OpenBankingResponseErrorMessage(string message) : base(message)
        {
        }

        internal OpenBankingResponseErrorMessage(Exception exception)
            : base(exception.WalkRecursive(e => e.InnerException).Select(e => e.Message)
                .JoinString(Environment.NewLine))
        {
        }
    }
}
