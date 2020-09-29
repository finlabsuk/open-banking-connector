// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    internal class
        FluentContextPostOnlyEntity<TPostOnly, TPublicRequest, TPublicResponse>
        : IFluentContextPostOnlyEntity<TPublicRequest, TPublicResponse>
        where TPostOnly : class, IPostOnlyWithPublicInterface<TPostOnly, TPublicRequest, TPublicResponse>, new()
        where TPublicRequest : class // required by IPostOnlyWithPublicInterface
        where TPublicResponse : class // required by IPostOnlyWithPublicInterface
    {
        protected readonly ISharedContext _context;

        internal FluentContextPostOnlyEntity(ISharedContext context)
        {
            _context = context;
        }

        public async Task<FluentResponse<TPublicResponse>> PostAsync(TPublicRequest publicRequest, string? createdBy)
        {
            publicRequest.ArgNotNull(nameof(publicRequest));

            IList<FluentResponseMessage> validationErrors = Validate(publicRequest);
            if (validationErrors.Count > 0)
            {
                return new FluentResponse<TPublicResponse>(messages: validationErrors, data: null);
            }

            try
            {
                TPublicResponse resp = await new TPostOnly().PostEntityAsyncWrapper(
                    arg1: _context,
                    arg2: publicRequest,
                    arg3: createdBy);
                return new FluentResponse<TPublicResponse>(resp);
            }
            catch (AggregateException ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentResponse<TPublicResponse>(messages: ex.CreateErrorMessages(), data: null);
            }
            catch (Exception ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentResponse<TPublicResponse>(message: ex.CreateErrorMessage(), data: null);
            }
        }


        private static IList<FluentResponseMessage> Validate(TPublicRequest requestObject)
        {
            ValidationResult validationResult =
                new TPostOnly().ValidatePublicRequestWrapper(requestObject);

            return validationResult.GetOpenBankingResponses();
        }
    }
}
