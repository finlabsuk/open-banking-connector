// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using RequestBank = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.Bank;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class CreateBank
    {
        private readonly IDbEntityRepository<Bank> _bankRepo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly ITimeProvider _timeProvider;

        public CreateBank(
            IDbEntityRepository<Bank> bankRepo,
            IDbMultiEntityMethods dbMultiEntityMethods,
            ITimeProvider timeProvider)
        {
            _bankRepo = bankRepo;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _timeProvider = timeProvider;
        }

        public async Task<BankResponse> CreateAsync(RequestBank requestBank, string? createdBy)
        {
            requestBank.ArgNotNull(nameof(requestBank));

            // Check no existing bank has same name
            Bank? existingBank = (await _bankRepo.GetAsync(b => b.Name == requestBank.Name)).SingleOrDefault();
            if (existingBank is object)
            {
                throw new ArgumentException("Bank already exists with same Name. Name must be unique.");
            }

            // Persist bank object
            Bank persistedBank = new Bank(
                timeProvider: _timeProvider,
                registrationScopeApiSet: requestBank.RegistrationScopeApiSet,
                issuerUrl: requestBank.IssuerUrl,
                financialId: requestBank.FinancialId,
                name: requestBank.Name,
                createdBy: createdBy);
            await _bankRepo.AddAsync(persistedBank);
            await _dbMultiEntityMethods.SaveChangesAsync();

            // Return response
            return persistedBank.PublicResponse;
        }
    }
}
