﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.BankConfiguration;

public class BankRegistrationCleanup
{
    public Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        ILogger logger)
    {
        List<BankRegistrationEntity> entityList =
            postgreSqlDbContext
                .BankRegistration
                .ToList();

        foreach (BankRegistrationEntity bankRegistration in entityList)
        {
            // Check for un-migrated registrations (should no longer exist)
            if (bankRegistration.SoftwareStatementId is null)
            {
                string message =
                    $"In its database record, BankRegistration with ID {bankRegistration.Id} specifies " +
                    $"use of software statement with null ID.";
                throw new Exception(message);
            }

            // Check for empty RedirectUris
            if (!bankRegistration.RedirectUris.Any())
            {
                throw new Exception(
                    $"Found non-empty RedirectUris for BankRegistration with ID {bankRegistration.Id}.");
            }

            // Check for empty DefaultFragmentRedirectUri
            if (string.IsNullOrEmpty(bankRegistration.DefaultFragmentRedirectUri))
            {
                throw new Exception(
                    $"Null or empty DefaultFragmentRedirectUri found for BankRegistration with ID {bankRegistration.Id}.");
            }

            // Check DefaultFragmentRedirectUri
            if (!bankRegistration.RedirectUris.Contains(bankRegistration.DefaultFragmentRedirectUri))
            {
                throw new Exception(
                    $"DefaultFragmentRedirectUri not included " +
                    $"in RedirectUris for BankRegistration with ID {bankRegistration.Id}.");
            }

            // Check for empty DefaultQueryRedirectUri
            if (string.IsNullOrEmpty(bankRegistration.DefaultQueryRedirectUri))
            {
                throw new Exception(
                    $"Null or empty DefaultQueryRedirectUri found for BankRegistration with ID {bankRegistration.Id}.");
            }

            // Check DefaultQueryRedirectUri
            if (!bankRegistration.RedirectUris.Contains(bankRegistration.DefaultQueryRedirectUri))
            {
                throw new Exception(
                    $"DefaultQueryRedirectUri not included " +
                    $"in RedirectUris for BankRegistration with ID {bankRegistration.Id}.");
            }
        }
        return Task.CompletedTask;
    }
}
