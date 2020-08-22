// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class FluentResponse<TData> : FluentResponseBase
    where TData: class
    {
        public FluentResponse(TData data)
        {
            Data = data;
        }

        public FluentResponse(
            FluentResponseMessage message,
            TData? data = null)
            : this(messages: new[] { message }, data: data) { }

        public FluentResponse(
            IList<FluentResponseMessage> messages,
            TData? data = null)
            : base(messages)
        {
            Data = data;
        }

        public TData? Data { get; }
    }
}
