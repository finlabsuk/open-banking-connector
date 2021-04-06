// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using RequestBank = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.Bank;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class PostBank
    {
        private readonly IDbReadWriteEntityMethods<Bank> _bankRepo;
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly ITimeProvider _timeProvider;

        public PostBank(
            IDbReadWriteEntityMethods<Bank> bankRepo,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider)
        {
            _bankRepo = bankRepo;
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _timeProvider = timeProvider;
        }

        public async Task<(BankPostResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            PostAsync(
                RequestBank requestBank,
                string? createdBy)
        {
            requestBank.ArgNotNull(nameof(requestBank));

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Check no existing bank has same name
            Bank? existingBank = (await _bankRepo.GetAsync(b => b.Name == requestBank.Name)).SingleOrDefault();
            if (!(existingBank is null))
            {
                throw new ArgumentException("Bank already exists with same Name. Name must be unique.");
            }

            // Persist bank object
            Bank persistedBank = new Bank(
                requestBank.IssuerUrl,
                requestBank.FinancialId,
                Guid.NewGuid(),
                requestBank.Name,
                createdBy,
                _timeProvider);
            await _bankRepo.AddAsync(persistedBank);
            await _dbSaveChangesMethod.SaveChangesAsync();

            return (persistedBank.PublicPostResponse, nonErrorMessages);
        }
    }
}
