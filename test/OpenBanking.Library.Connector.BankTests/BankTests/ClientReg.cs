// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    public partial class ClientReg
    {
        public async Task<(Guid bankId, Guid bankRegistrationId, Guid bankApiInformationId)> ClientRegistrationTest(
            string softwareStatementProfileId,
            RegistrationScope registrationScope,
            IRequestBuilder requestBuilder,
            BankProfile bankProfile,
            TestDataProcessor testDataProcessor)
        {
            // Get/update bank name
            string configurationName = await GetBankNameNextVersion(
                requestBuilder,
                bankProfile.BankProfileEnum.ToString(),
                softwareStatementProfileId);

            // Create bank
            Bank bankRequest = bankProfile.BankObject("placeholder: dynamically generated based on unused names");
            await testDataProcessor.ProcessData(bankRequest);
            bankRequest.Name = configurationName;
            IFluentResponse<BankPostResponse> bankResp = await requestBuilder.ClientRegistration
                .Banks
                .PostAsync(bankRequest);

            bankResp.Should().NotBeNull();
            bankResp.Messages.Should().BeEmpty();
            bankResp.Data.Should().NotBeNull();
            Guid bankId = bankResp.Data!.Id;

            // Create bank registration
            BankRegistration registrationRequest = bankProfile.BankRegistration(
                "placeholder: dynamically generated based on unused names",
                default,
                softwareStatementProfileId,
                registrationScope);
            await testDataProcessor.ProcessData(registrationRequest);
            registrationRequest.Name = configurationName;
            registrationRequest.BankId = bankId;
            IFluentResponse<BankRegistrationPostResponse> registrationResp = await requestBuilder.ClientRegistration
                .BankRegistrations
                .PostAsync(registrationRequest);

            registrationResp.Should().NotBeNull();
            registrationResp.Messages.Should().BeEmpty();
            registrationResp.Data.Should().NotBeNull();
            Guid bankRegistrationId = registrationResp.Data!.Id;

            // Create bank API information
            BankApiInformation apiInformationRequest = bankProfile.BankApiInformation(
                "placeholder: dynamically generated based on unused names",
                default);
            await testDataProcessor.ProcessData(apiInformationRequest);
            apiInformationRequest.Name = configurationName;
            apiInformationRequest.BankId = bankId;
            IFluentResponse<BankApiInformationPostResponse> apiInformationResponse = await requestBuilder
                .ClientRegistration
                .BankApiInformationObjects
                .PostAsync(apiInformationRequest);

            apiInformationResponse.Should().NotBeNull();
            apiInformationResponse.Messages.Should().BeEmpty();
            apiInformationResponse.Data.Should().NotBeNull();
            Guid bankApiInformationId = apiInformationResponse.Data!.Id;

            return (bankId, bankRegistrationId, bankApiInformationId);
        }

        /// <summary>
        ///     Customisation point for <paramref name="softwareStatementProfileId" /> as function of
        ///     <paramref name="registrationScope" />
        /// </summary>
        /// <param name="registrationScope"></param>
        /// <param name="softwareStatementProfileId"></param>
        partial void CustomSoftwareStatementProfileId(
            RegistrationScope registrationScope,
            ref string softwareStatementProfileId);

        public string SoftwareStatementProfileId(RegistrationScope registrationScope)
        {
            string softwareStatementProfileId = registrationScope switch
            {
                RegistrationScope.PaymentInitiation => "PaymentInitiation",
                RegistrationScope.All => "All",
                RegistrationScope.None => throw new ArgumentOutOfRangeException(
                    $"Software statement profile IDs available for non-empty {nameof(RegistrationScope)} only."),
                RegistrationScope.AccountAndTransaction => "AccountTransaction",
                RegistrationScope.FundsConfirmation => "FundsConfirmation",
                _ => throw new ArgumentOutOfRangeException(nameof(registrationScope))
            };
            CustomSoftwareStatementProfileId(
                registrationScope,
                ref softwareStatementProfileId);
            return softwareStatementProfileId;
        }

        private static async Task<string> GetBankNameNextVersion(
            IRequestBuilder builder,
            string defaultBankName,
            string stringsoftwareStatementProfileId)
        {
            (string prefix, int? postfixIfCurrentVersionExists) tmp = await GetBankNameCurrentVersionInner(
                builder,
                defaultBankName,
                stringsoftwareStatementProfileId);
            int postfix = (tmp.postfixIfCurrentVersionExists ?? 0) + 1;
            string bankName = tmp.prefix + postfix;
            return bankName;
        }

        private static async Task<(bool currentVersionExists, string? bankName)> GetBankNameCurrentVersion(
            IRequestBuilder builder,
            string defaultBankName,
            string softwareStatementProfileId)
        {
            (string prefix, int? postfixIfCurrentVersionExists) tmp = await GetBankNameCurrentVersionInner(
                builder,
                defaultBankName,
                softwareStatementProfileId);
            bool currentVersionExists = tmp.postfixIfCurrentVersionExists != null;
            string? bankName = currentVersionExists ? tmp.prefix + tmp.postfixIfCurrentVersionExists : null;
            return (currentVersionExists, bankName);
        }

        private static async Task<(string prefix, int? postfixIfCurrentVersionExists)> GetBankNameCurrentVersionInner(
            IRequestBuilder builder,
            string defaultBankName,
            string softwareStatementProfileId)
        {
            string prefix = softwareStatementProfileId + "_" + defaultBankName + "_v";

            // Get banks
            IFluentResponse<IQueryable<BankGetLocalResponse>> bankList = await builder.ClientRegistration
                .Banks
                .GetLocalAsync(b => b.Name.StartsWith(prefix));

            bankList.Should().NotBeNull();
            bankList.Messages.Should().BeEmpty();
            bankList.Data.Should().NotBeNull();

            // Determine bank name current postfix (highest version number)
            IOrderedEnumerable<int> orderedPostfixes;
            try
            {
                orderedPostfixes =
                    bankList.Data!
                        .AsEnumerable()
                        .Select(x => int.Parse(x.Name.Substring(prefix.Length)))
                        .OrderByDescending(x => x);
            }
            catch
            {
                throw new InvalidOperationException("Unexpected bank name postfix found in database.");
            }

            int currentPostfix = orderedPostfixes.FirstOrDefault();

            return (prefix, currentPostfix != 0 ? (int?) currentPostfix : null);
        }
    }
}
