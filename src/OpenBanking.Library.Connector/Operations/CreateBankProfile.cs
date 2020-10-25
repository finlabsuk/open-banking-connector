// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using RequestBankProfile = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class CreateBankProfile
    {
        private readonly IDbEntityRepository<BankProfile> _bankProfileRepo;
        private readonly IDbReadOnlyEntityRepository<BankRegistration> _bankRegistrationRepo;
        private readonly IDbEntityRepository<Bank> _bankRepo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly ITimeProvider _timeProvider;

        public CreateBankProfile(
            IDbEntityRepository<BankProfile> bankProfileRepo,
            IDbReadOnlyEntityRepository<BankRegistration> bankRegistrationRepo,
            IDbEntityRepository<Bank> bankRepo,
            IDbMultiEntityMethods dbMultiEntityMethods,
            ITimeProvider timeProvider)
        {
            _bankProfileRepo = bankProfileRepo;
            _bankRegistrationRepo = bankRegistrationRepo;
            _bankRepo = bankRepo;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _timeProvider = timeProvider;
        }

        public async Task<BankProfileResponse> CreateAsync(RequestBankProfile request, string? createdBy)
        {
            request.ArgNotNull(nameof(request));

            // Load relevant objects
            Guid bankId = request.BankId;
            Bank bank = await _bankRepo.GetAsync(bankId) ??
                        throw new KeyNotFoundException($"No record found for BankId {bankId} specified by request.");

            // TODO: check bank profile only specifies APIs consistent with Bank.

            // Create and store persistent object
            BankProfile persistentBankProfile = new BankProfile(
                timeProvider: _timeProvider,
                paymentInitiationApi: request.PaymentInitiationApi,
                bankId: bankId,
                createdBy: createdBy);
            await _bankProfileRepo.AddAsync(persistentBankProfile);

            // Update registration references
            if (request.ReplaceDefaultBankProfile || request.ReplaceStagingBankProfile)
            {
                ReadWriteProperty<Guid?> profileId = new ReadWriteProperty<Guid?>(
                    data: persistentBankProfile.Id,
                    timeProvider: _timeProvider,
                    modifiedBy: null);
                if (request.ReplaceDefaultBankProfile)
                {
                    bank.DefaultBankProfileId = profileId;
                }

                if (request.ReplaceStagingBankProfile)
                {
                    bank.StagingBankProfileId = profileId;
                }
            }

            // Persist updates
            await _dbMultiEntityMethods.SaveChangesAsync();

            // Return response
            return persistentBankProfile.PublicResponse;
        }
    }
}
