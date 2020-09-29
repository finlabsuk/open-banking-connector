// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IDbEntityRepository<BankRegistration> _bankRegistrationRepo;
        private readonly IDbEntityRepository<Bank> _bankRepo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly ITimeProvider _timeProvider;

        public CreateBankProfile(
            IDbEntityRepository<BankProfile> bankProfileRepo,
            IDbEntityRepository<BankRegistration> bankRegistrationRepo,
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

        public async Task<BankProfileResponse> CreateAsync(RequestBankProfile requestBankProfile, string? createdBy)
        {
            requestBankProfile.ArgNotNull(nameof(requestBankProfile));

            string bankRegistrationId;
            string bankId;
            switch (requestBankProfile.BankRegistrationId)
            {
                // No bank registration specified directly or indirectly
                case null when requestBankProfile.BankName is null:
                    throw new ArgumentException("One of BankRegistrationId and BankName must be non-null.");

                // Look up bank registration
                case null:
                {
                    Bank bank;
                    try
                    {
                        bank = (await _bankRepo.GetAsync(b => b.Name == requestBankProfile.BankName)).Single();
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException("No record found for BankName.");
                    }

                    bankId = bank.Id;

                    if (requestBankProfile.UseStagingBankRegistration)
                    {
                        bankRegistrationId = bank.StagingBankRegistrationId.Data ?? throw new ArgumentException(
                            "Bank specified by BankName doesn't have StagingBankRegistrationId.");
                    }
                    else
                    {
                        bankRegistrationId = bank.DefaultBankRegistrationId.Data ?? throw new ArgumentException(
                            "Bank specified by BankName doesn't have DefaultBankRegistrationId.");
                    }

                    break;
                }

                default:
                {
                    if (!(requestBankProfile.BankName is null))
                    {
                        throw new ArgumentException("Both BankRegistrationId and BankName are non-null.");
                    }

                    bankRegistrationId = requestBankProfile.BankRegistrationId;
                    BankRegistration bankRegistration = await _bankRegistrationRepo.GetAsync(bankRegistrationId) ??
                                                        throw new KeyNotFoundException(
                                                            "No record found for BankRegistrationId.");
                    bankId = bankRegistration.BankId;
                    break;
                }
            }

            // Create and store persistent object
            BankProfile persistentBankProfile = new BankProfile(
                timeProvider: _timeProvider,
                bankRegistrationId: bankRegistrationId,
                paymentInitiationApi: requestBankProfile.PaymentInitiationApi,
                bankId: bankId,
                createdBy: createdBy);
            await _bankProfileRepo.AddAsync(persistentBankProfile);

            // Update registration references
            if (requestBankProfile.ReplaceDefaultBankProfile || requestBankProfile.ReplaceStagingBankProfile)
            {
                Bank bank = await _bankRepo.GetAsync(bankId) ??
                            throw new KeyNotFoundException("No record found for BankId.");
                ReadWriteProperty<string?> profileId = new ReadWriteProperty<string?>(
                    data: persistentBankProfile.Id,
                    timeProvider: _timeProvider,
                    modifiedBy: null);
                if (requestBankProfile.ReplaceDefaultBankProfile)
                {
                    bank.DefaultBankProfileId = profileId;
                }

                if (requestBankProfile.ReplaceStagingBankProfile)
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
