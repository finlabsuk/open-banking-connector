// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    internal interface IPostOnlyWithPublicInterface<TSelf, TPublicRequest, TPublicResponse>
        where TSelf : class, IPostOnlyWithPublicInterface<TSelf, TPublicRequest, TPublicResponse>, new()
        where TPublicRequest : class
        where TPublicResponse : class
    {
        /// <summary>
        ///     NB: static method needs to be wrapped in instance method due to C# not yet supporting abstract interface static
        ///     members
        /// </summary>
        Func<TPublicRequest, ValidationResult> ValidatePublicRequestWrapper { get; }

        /// <summary>
        ///     NB: static method needs to be wrapped in instance method due to C# not yet supporting abstract interface static
        ///     members
        /// </summary>
        Func<ISharedContext, TPublicRequest, string?, Task<TPublicResponse>> PostEntityAsyncWrapper { get; }
    }
}
