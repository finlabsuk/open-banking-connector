// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions
{
    public static class FluentResponseExtensions
    {
        public static HttpResponse ToHttpResponse(this IFluentResponse value) =>
            new HttpResponse(messages: value.GetHttpResponseMessages());

        public static HttpResponse<TData> ToHttpResponse<TData>(this IFluentResponse<TData> value)
            where TData : class =>
            new HttpResponse<TData>(value.GetHttpResponseMessages(), value.Data);

        private static HttpResponseMessages? GetHttpResponseMessages(this IFluentResponse value)
        {
            HttpResponseMessages? messages = null;
            if (value.Messages.Any())
            {
                messages = new HttpResponseMessages();

                List<string> infos = value.Messages.OfType<FluentResponseInfoMessage>().Select(m => m.Message)
                    .ToList();
                if (infos.Count > 0)
                {
                    messages.InformationMessages = infos;
                }

                List<string> warnings = value.Messages.OfType<FluentResponseWarningMessage>().Select(m => m.Message)
                    .ToList();
                if (warnings.Count > 0)
                {
                    messages.WarningMessages = warnings;
                }

                List<string> errors = value.Messages.OfType<IFluentResponseErrorMessage>().Select(m => m.Message)
                    .ToList();
                if (errors.Count > 0)
                {
                    messages.ErrorMessages = errors;
                }
            }

            return messages;
        }
    }
}
