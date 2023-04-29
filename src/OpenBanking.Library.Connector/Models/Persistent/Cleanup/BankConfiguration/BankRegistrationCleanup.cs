// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
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

            // Update bank profile
            if (bankRegistration.BankProfile is BankProfileEnum.DbTransitionalDefault)
            {
                string issuerUrl = bankRegistration.BankNavigation.IssuerUrl;
                BankProfileEnum newBankProfile = CorrectedBankProfile(issuerUrl);

                string message =
                    $"In its database record, BankRegistration with ID {bankRegistration.Id} specifies " +
                    $"no BankProfile but specifies use of bank with Issuer URL {issuerUrl}. ";

                if (newBankProfile is not BankProfileEnum.DbTransitionalDefault)
                {
                    bankRegistration.BankProfile = newBankProfile;
                    message +=
                        $"This has been used to set {nameof(bankRegistration.BankProfile)} to " +
                        $"{newBankProfile} as part of database cleanup.";
                    logger.LogInformation(message);
                }
                else
                {
                    message +=
                        $"No suitable BankProfile could be found for this BankRegistration " +
                        $"during database cleanup.";
                    logger.LogWarning(message);
                }
            }

            // Update bank group
            if (bankRegistration.BankGroup is BankGroupEnum.DbTransitionalDefault &&
                bankRegistration.BankProfile is not BankProfileEnum.DbTransitionalDefault)
            {
                BankGroupEnum newBankGroup = BankProfileService.GetBankGroupEnum(bankRegistration.BankProfile);
                string message =
                    $"In its database record, BankRegistration with ID {bankRegistration.Id} specifies " +
                    $"no BankGroup but a valid BankProfile. ";

                bankRegistration.BankGroup = newBankGroup;
                message +=
                    $"This has been used to set {nameof(bankRegistration.BankGroup)} to " +
                    $"{newBankGroup} as part of database cleanup.";
                logger.LogInformation(message);
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

    private static BankProfileEnum CorrectedBankProfile(string issuerUrl) =>
        issuerUrl switch
        {
            // from ObieGenerator.cs
            "https://ob19-auth1-ui.o3bank.co.uk" => BankProfileEnum.Obie_Modelo,
            // from HsbcGenerator.cs (sandbox missing)
            "https://api.ob.firstdirect.com" => BankProfileEnum.Hsbc_FirstDirect,
            "https://api.ob.business.hsbc.co.uk" => BankProfileEnum.Hsbc_UkBusiness,
            "https://api.ob.hsbckinetic.co.uk" => BankProfileEnum.Hsbc_UkKinetic,
            "https://api.ob.hsbc.co.uk" => BankProfileEnum.Hsbc_UkPersonal,
            // from NatWestGenerator.cs (sandboxes missing)
            "https://personal.secure1.natwest.com" => BankProfileEnum.NatWest_NatWest,
            "https://corporate.secure1.natwest.com" => BankProfileEnum.NatWest_NatWestBankline,
            "https://clearspend.secure1.natwest.com" => BankProfileEnum.NatWest_NatWestClearSpend,
            "https://personal.secure1.rbs.co.uk" => BankProfileEnum.NatWest_RoyalBankOfScotland,
            "https://corporate.secure1.rbs.co.uk" => BankProfileEnum.NatWest_RoyalBankOfScotlandBankline,
            "https://clearspend.secure1.rbs.co.uk" => BankProfileEnum.NatWest_RoyalBankOfScotlandClearSpend,
            "https://toa.secure1.rbs.co.uk" => BankProfileEnum.NatWest_TheOne,
            "https://noa.secure1.rbs.co.uk" => BankProfileEnum.NatWest_NatWestOne,
            "https://voa.secure1.rbs.co.uk" => BankProfileEnum.NatWest_VirginOne,
            "https://personal.secure1.ulsterbank.co.uk" => BankProfileEnum.NatWest_UlsterBankNi,
            "https://corporate.secure1.ulsterbank.co.uk" => BankProfileEnum.NatWest_UlsterBankNiBankline,
            "https://clearspend.secure1.ulsterbank.co.uk" => BankProfileEnum.NatWest_UlsterBankNiClearSpend,
            // from LloydsGenerator.cs
            "https://as.aspsp.sandbox.lloydsbanking.com/oauth2" => BankProfileEnum.Lloyds_Sandbox,
            "https://authorise-api.lloydsbank.co.uk/prod01/channel/mtls/lyds/personal" => BankProfileEnum
                .Lloyds_LloydsPersonal,
            "https://authorise-api.lloydsbank.co.uk/prod01/channel/mtls/lyds/business" => BankProfileEnum
                .Lloyds_LloydsBusiness,
            "https://authorise-api.lloydsbank.co.uk/prod01/channel/mtls/lyds/commercial" => BankProfileEnum
                .Lloyds_LloydsCommerical,
            "https://authorise-api.halifax-online.co.uk/prod01/channel/mtls/hfx/personal" => BankProfileEnum
                .Lloyds_HalifaxPersonal,
            "https://authorise-api.bankofscotland.co.uk/prod01/channel/mtls/bos/personal" => BankProfileEnum
                .Lloyds_BankOfScotlandPersonal,
            "https://authorise-api.bankofscotland.co.uk/prod01/channel/mtls/bos/business" => BankProfileEnum
                .Lloyds_BankOfScotlandBusiness,
            "https://authorise-api.bankofscotland.co.uk/prod01/channel/mtls/bos/commercial" => BankProfileEnum
                .Lloyds_BankOfScotlandCommerical,
            "https://authorise-api.mbna.co.uk/prod01/channel/mtls/mbn/personal" => BankProfileEnum.Lloyds_MbnaPersonal,
            // from BarclaysGenerator.cs
            "https://token.sandbox.barclays.com" => BankProfileEnum.Barclays_Sandbox,
            "https://oauth.tiaa.barclays.com/BarclaysPersonal" => BankProfileEnum.Barclays_Personal,
            "https://oauth.tiaa.barclays.com/BarclaysWealth" => BankProfileEnum.Barclays_Wealth,
            "https://oauth.tiaa.barclays.com/BarclaycardUK" => BankProfileEnum.Barclays_Barclaycard,
            "https://oauth.tiaa.barclays.com/BarclaysBusiness" => BankProfileEnum.Barclays_Business,
            "https://oauth.tiaa.barclays.com/BarclaysCorporate" => BankProfileEnum.Barclays_Corporate,
            "https://oauth.tiaa.barclays.com/BCP" => BankProfileEnum.Barclays_BarclaycardCommercialPayments,
            _ => BankProfileEnum.DbTransitionalDefault // default
        };
}
