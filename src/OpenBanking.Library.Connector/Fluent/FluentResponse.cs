// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class FluentResponse<TData>
        where TData : class
    {
        internal FluentResponse(TData data)
        {
            Data = data;
        }

        internal FluentResponse(
            FluentResponseMessage message,
            TData? data = null)
            : this(messages: new[] { message }, data: data) { }

        internal FluentResponse(
            IList<FluentResponseMessage> messages,
            TData? data = null)
        {
            Messages = messages;
            Data = data;
        }

        public TData? Data { get; }

        public IList<FluentResponseMessage> Messages { get; } = new List<FluentResponseMessage>();

        public bool HasInfos => Messages.NullToEmpty().OfType<FluentResponseInfoMessage>().Any();
        public bool HasWarnings => Messages.NullToEmpty().OfType<FluentResponseWarningMessage>().Any();
        public bool HasErrors => Messages.NullToEmpty().OfType<FluentResponseErrorMessage>().Any();
    }
}
