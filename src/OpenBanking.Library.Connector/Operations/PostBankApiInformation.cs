// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class PostBankApiInformation
    {
        private readonly IDbReadWriteEntityMethods<BankApiInformation> _bankProfileRepo;
        private readonly IDbReadWriteEntityMethods<Bank> _bankRepo;
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly ITimeProvider _timeProvider;

        public PostBankApiInformation(
            IDbReadWriteEntityMethods<BankApiInformation> bankProfileRepo,
            IDbReadWriteEntityMethods<Bank> bankRepo,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider)
        {
            _bankProfileRepo = bankProfileRepo;
            _bankRepo = bankRepo;
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _timeProvider = timeProvider;
        }

        public async Task<(BankApiInformationPostResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            PostAsync(
                Models.Public.Request.BankApiInformation request,
                string? createdBy)
        {
            request.ArgNotNull(nameof(request));

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();


            // Load relevant objects
            Guid bankId = request.BankId;
            Bank bank = await _bankRepo.GetAsync(bankId) ??
                        throw new KeyNotFoundException($"No record found for BankId {bankId} specified by request.");

            // Create and store persistent object
            BankApiInformation bankApiInformation = new BankApiInformation(
                request.PaymentInitiationApi,
                bankId,
                Guid.NewGuid(),
                request.Name,
                createdBy,
                _timeProvider);
            await _bankProfileRepo.AddAsync(bankApiInformation);

            // Persist updates
            await _dbSaveChangesMethod.SaveChangesAsync();

            return (bankApiInformation.PublicPostResponse, nonErrorMessages);
        }
    }
}
