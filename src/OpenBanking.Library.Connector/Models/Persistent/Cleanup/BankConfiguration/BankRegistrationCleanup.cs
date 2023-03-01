// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.BankConfiguration;

public class BankRegistrationCleanup
{
    public async Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        IProcessedSoftwareStatementProfileStore processedSoftwareStatementProfileStore,
        ILogger logger)
    {
        IQueryable<BankRegistration> entityList =
            postgreSqlDbContext
                .BankRegistration
                .Include(x => x.BankNavigation);

        foreach (BankRegistration bankRegistration in entityList)
        {
            // Check if software statement profile available
            ProcessedSoftwareStatementProfile? processedSoftwareStatementProfile = null;
            string softwareStatementProfileId = bankRegistration.SoftwareStatementProfileId;
            string? softwareStatementProfileOverride = bankRegistration.SoftwareStatementProfileOverride;
            try
            {
                processedSoftwareStatementProfile = await processedSoftwareStatementProfileStore.GetAsync(
                    softwareStatementProfileId,
                    softwareStatementProfileOverride);
            }
            catch
            {
                string message =
                    $"In its database record, BankRegistration with ID {bankRegistration.Id} specifies " +
                    $"use of software statement profile with ID {softwareStatementProfileId}";
                message += softwareStatementProfileOverride is null
                    ? ". "
                    : $" and override {softwareStatementProfileOverride}. ";
                message += "This software statement profile was not found in configuration/secrets.";
                logger.LogWarning(message);
            }

            // Populate redirect URIs in case of record created with earlier software version where
            // these were not persisted in the database
            if (processedSoftwareStatementProfile is not null &&
                string.IsNullOrEmpty(bankRegistration.DefaultRedirectUri))
            {
                string defaultRedirectUri = processedSoftwareStatementProfile.DefaultFragmentRedirectUrl;
                if (string.IsNullOrEmpty(defaultRedirectUri))
                {
                    throw new Exception(
                        $"Can't find defaultRedirectUri for software statement profile {softwareStatementProfileId} ");
                }

                List<string> otherRedirectUris =
                    processedSoftwareStatementProfile.SoftwareStatementPayload.SoftwareRedirectUris;
                otherRedirectUris.Remove(defaultRedirectUri);

                bankRegistration.DefaultRedirectUri = defaultRedirectUri;
                bankRegistration.OtherRedirectUris = otherRedirectUris;

                string message =
                    $"In its database record, BankRegistration with ID {bankRegistration.Id} specifies " +
                    $"use of software statement profile with ID {softwareStatementProfileId}";
                message += softwareStatementProfileOverride is null
                    ? ". "
                    : $" and override {softwareStatementProfileOverride}. ";
                message +=
                    $"This has been used to set {nameof(bankRegistration.DefaultRedirectUri)} and " +
                    $"{nameof(bankRegistration.OtherRedirectUris)} as part of database cleanup.";
                logger.LogInformation(message);
            }

            // Update Lloyds custom behaviour
            if (bankRegistration.BankRegistrationGroup is BankRegistrationGroup.Lloyds_Production)
            {
                bool? idTokenNonceClaimIsPreviousValue =
                    bankRegistration.BankNavigation.CustomBehaviour?.AccountAccessConsentAuthGet
                        ?.IdTokenNonceClaimIsPreviousValue;
                bool? responseLinksOmitId =
                    bankRegistration.BankNavigation.CustomBehaviour?
                        .AccountAccessConsentPost?.ResponseLinksOmitId;
                DateTimeOffsetConverterEnum? previousPaymentDateTimeJsonConverter =
                    bankRegistration.BankNavigation.CustomBehaviour?
                        .DirectDebitGet?.PreviousPaymentDateTimeJsonConverter;
                var changeMade = false;

                CustomBehaviourClass newCustomBehaviour = GetNewCustomBehaviour(bankRegistration);

                if (idTokenNonceClaimIsPreviousValue is null)
                {
                    newCustomBehaviour.AccountAccessConsentAuthGet ??=
                        new ConsentAuthGetCustomBehaviour();
                    newCustomBehaviour.AccountAccessConsentAuthGet
                        .IdTokenNonceClaimIsPreviousValue = true;
                    changeMade = true;
                }

                if (responseLinksOmitId is null)
                {
                    newCustomBehaviour.AccountAccessConsentPost ??=
                        new AccountAccessConsentPostCustomBehaviour();
                    newCustomBehaviour.AccountAccessConsentPost.ResponseLinksOmitId = true;
                    changeMade = true;
                }

                if (previousPaymentDateTimeJsonConverter is null)
                {
                    newCustomBehaviour.DirectDebitGet ??=
                        new DirectDebitGetCustomBehaviour();
                    newCustomBehaviour.DirectDebitGet.PreviousPaymentDateTimeJsonConverter =
                        DateTimeOffsetConverterEnum.JsonInvalidStringBecomesNull;
                    changeMade = true;
                }

                if (changeMade)
                {
                    bankRegistration.BankNavigation.CustomBehaviour = newCustomBehaviour;
                    string message =
                        $"In its database record, BankRegistration with ID {bankRegistration.Id} and BankRegistrationGroup {bankRegistration.BankRegistrationGroup} specifies " +
                        $"use of bank with ID {bankRegistration.BankId}. ";
                    message +=
                        "The custom_behaviour field of this bank record has been updated " +
                        "as part of database cleanup.";
                    logger.LogInformation(message);
                }
            }

            // Update NatWest custom behaviour
            if (bankRegistration.BankRegistrationGroup is BankRegistrationGroup.NatWest_NatWestProduction
                or BankRegistrationGroup.NatWest_RoyalBankOfScotlandProduction
                or BankRegistrationGroup.NatWest_UlsterBankNiProduction)
            {
                bool? doNotValidateIdTokenAcrClaim =
                    bankRegistration.BankNavigation.CustomBehaviour?
                        .AccountAccessConsentAuthGet?.DoNotValidateIdTokenAcrClaim;
                bool? doNotValidateIdTokenAcrClaim2 =
                    bankRegistration.BankNavigation.CustomBehaviour?
                        .AuthCodeGrantPost?.DoNotValidateIdTokenAcrClaim;
                var changeMade = false;

                CustomBehaviourClass newCustomBehaviour = GetNewCustomBehaviour(bankRegistration);
                if (doNotValidateIdTokenAcrClaim is null)
                {
                    newCustomBehaviour.AccountAccessConsentAuthGet ??=
                        new ConsentAuthGetCustomBehaviour();
                    newCustomBehaviour.AccountAccessConsentAuthGet.DoNotValidateIdTokenAcrClaim =
                        true;
                    changeMade = true;
                }

                if (doNotValidateIdTokenAcrClaim2 is null)
                {
                    newCustomBehaviour.AuthCodeGrantPost ??=
                        new GrantPostCustomBehaviour();
                    newCustomBehaviour.AuthCodeGrantPost.DoNotValidateIdTokenAcrClaim = true;
                    changeMade = true;
                }

                if (changeMade)
                {
                    bankRegistration.BankNavigation.CustomBehaviour = newCustomBehaviour;
                    string message =
                        $"In its database record, BankRegistration with ID {bankRegistration.Id} and BankRegistrationGroup {bankRegistration.BankRegistrationGroup} specifies " +
                        $"use of bank with ID {bankRegistration.BankId}. ";
                    message +=
                        "The custom_behaviour field of this bank record has been updated " +
                        "as part of database cleanup.";
                    logger.LogInformation(message);
                }
            }

            // Update Barclays custom behaviour
            if (bankRegistration.BankRegistrationGroup is BankRegistrationGroup.Barclays_Production)
            {
                bool? responseLinksOmitId = bankRegistration.BankNavigation.CustomBehaviour?.AccountAccessConsentPost
                    ?.ResponseLinksOmitId;
                var changeMade = false;

                CustomBehaviourClass newCustomBehaviour = GetNewCustomBehaviour(bankRegistration);

                if (responseLinksOmitId is null)
                {
                    newCustomBehaviour.AccountAccessConsentPost ??=
                        new AccountAccessConsentPostCustomBehaviour();
                    newCustomBehaviour.AccountAccessConsentPost.ResponseLinksOmitId = true;
                    changeMade = true;
                }

                if (changeMade)
                {
                    bankRegistration.BankNavigation.CustomBehaviour = newCustomBehaviour;
                    string message =
                        $"In its database record, BankRegistration with ID {bankRegistration.Id} and BankRegistrationGroup {bankRegistration.BankRegistrationGroup} specifies " +
                        $"use of bank with ID {bankRegistration.BankId}. ";
                    message +=
                        "The custom_behaviour field of this bank record has been updated " +
                        "as part of database cleanup.";
                    logger.LogInformation(message);
                }
            }
        }
    }

    private static CustomBehaviourClass GetNewCustomBehaviour(BankRegistration bankRegistration)
    {
        CustomBehaviourClass newCustomBehaviour;
        if (bankRegistration.BankNavigation.CustomBehaviour is null)
        {
            newCustomBehaviour = new CustomBehaviourClass();
        }
        else
        {
            newCustomBehaviour = JsonConvert.DeserializeObject<CustomBehaviourClass>(
                JsonConvert.SerializeObject(
                    bankRegistration.BankNavigation.CustomBehaviour,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }))!;
        }

        return newCustomBehaviour;
    }
}
