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
    public interface ICreateBankProfile
    {
        Task<BankProfileResponse> CreateAsync(RequestBankProfile bankProfile);
    }

    public class CreateBankProfile : ICreateBankProfile
    {
        private readonly IDbEntityRepository<BankProfile> _bankProfileRepo;
        private readonly IDbEntityRepository<BankRegistration> _bankRegistrationRepo;
        private readonly IDbEntityRepository<Bank> _bankRepo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;

        public CreateBankProfile(
            IDbEntityRepository<BankProfile> bankProfileRepo,
            IDbEntityRepository<BankRegistration> bankRegistrationRepo,
            IDbEntityRepository<Bank> bankRepo,
            IDbMultiEntityMethods dbMultiEntityMethods)
        {
            _bankProfileRepo = bankProfileRepo;
            _bankRegistrationRepo = bankRegistrationRepo;
            _bankRepo = bankRepo;
            _dbMultiEntityMethods = dbMultiEntityMethods;
        }

        public async Task<BankProfileResponse> CreateAsync(RequestBankProfile requestBankProfile)
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
                        bankRegistrationId = bank.StagingBankRegistrationId ?? throw new ArgumentException(
                            "Bank specified by BankName doesn't have StagingBankRegistrationId.");
                    }
                    else
                    {
                        bankRegistrationId = bank.DefaultBankRegistrationId ?? throw new ArgumentException(
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
                bankRegistrationId: bankRegistrationId,
                paymentInitiationApi: requestBankProfile.PaymentInitiationApi,
                bankId: bankId);
            await _bankProfileRepo.AddAsync(persistentBankProfile);

            // Update registration references
            if (requestBankProfile.ReplaceDefaultBankProfile || requestBankProfile.ReplaceStagingBankProfile)
            {
                Bank bank = await _bankRepo.GetAsync(bankId) ??
                             throw new KeyNotFoundException("No record found for BankId.");
                if (requestBankProfile.ReplaceDefaultBankProfile)
                {
                    bank.DefaultBankProfileId = persistentBankProfile.Id;
                }

                if (requestBankProfile.ReplaceStagingBankProfile)
                {
                    bank.StagingBankProfileId = persistentBankProfile.Id;
                }
            }

            // Persist updates
            await _dbMultiEntityMethods.SaveChangesAsync();

            // Return response
            return new BankProfileResponse(persistentBankProfile);
        }
    }
}
