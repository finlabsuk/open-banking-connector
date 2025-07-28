﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;
using ObSealCertificate =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request.ObSealCertificate;
using ObWacCertificate =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request.ObWacCertificate;
using EncryptionKeyDescription =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request.EncryptionKeyDescription;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Management;

public interface IManagementContext
{
    /// <summary>
    ///     API for BankRegistration objects.
    ///     A BankRegistration corresponds to an OAuth2 client registration with a Bank. Multiple BankRegistrations may be
    ///     created for the same bank.
    /// </summary>
    IBankRegistrationsContext BankRegistrations { get; }

    ILocalEntityContext<EncryptionKeyDescription, IEncryptionKeyDescriptionPublicQuery,
            EncryptionKeyDescriptionResponse, EncryptionKeyDescriptionResponse>
        EncryptionKeyDescriptions { get; }

    ILocalEntityContext<ObWacCertificate, IObWacCertificatePublicQuery,
            ObWacCertificateResponse, ObWacCertificateResponse>
        ObWacCertificates { get; }

    ILocalEntityContext<ObSealCertificate, IObSealCertificatePublicQuery,
            ObSealCertificateResponse, ObSealCertificateResponse>
        ObSealCertificates { get; }

    ISoftwareStatementsContext
        SoftwareStatements { get; }
}

internal class ManagementContext : IManagementContext
{
    private readonly ISharedContext _sharedContext;

    public ManagementContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;
    }

    public IBankRegistrationsContext
        BankRegistrations => new BankRegistrationsContext(_sharedContext);

    public ILocalEntityContext<ObWacCertificate, IObWacCertificatePublicQuery, ObWacCertificateResponse,
        ObWacCertificateResponse> ObWacCertificates =>
        new LocalEntityContext<ObWacCertificateEntity, ObWacCertificate, IObWacCertificatePublicQuery,
            ObWacCertificateResponse, ObWacCertificateResponse>(
            _sharedContext,
            new ObWacCertificatePost(
                _sharedContext.DbService.GetDbEntityMethods<ObWacCertificateEntity>(),
                _sharedContext.DbService.GetDbMethods(),
                _sharedContext.TimeProvider,
                _sharedContext.Instrumentation,
                _sharedContext.HttpClientSettingsProvider,
                _sharedContext.MemoryCache,
                _sharedContext.TppReportingMetrics,
                _sharedContext.SecretProvider));

    public ILocalEntityContext<ObSealCertificate, IObSealCertificatePublicQuery, ObSealCertificateResponse,
        ObSealCertificateResponse> ObSealCertificates =>
        new LocalEntityContext<ObSealCertificateEntity, ObSealCertificate, IObSealCertificatePublicQuery,
            ObSealCertificateResponse, ObSealCertificateResponse>(
            _sharedContext,
            new ObSealCertificatePost(
                _sharedContext.DbService.GetDbEntityMethods<ObSealCertificateEntity>(),
                _sharedContext.DbService.GetDbMethods(),
                _sharedContext.TimeProvider,
                _sharedContext.Instrumentation,
                _sharedContext.MemoryCache,
                _sharedContext.SecretProvider));

    public ILocalEntityContext<EncryptionKeyDescription, IEncryptionKeyDescriptionPublicQuery,
        EncryptionKeyDescriptionResponse, EncryptionKeyDescriptionResponse> EncryptionKeyDescriptions =>
        new LocalEntityContext<EncryptionKeyDescriptionEntity, EncryptionKeyDescription,
            IEncryptionKeyDescriptionPublicQuery, EncryptionKeyDescriptionResponse, EncryptionKeyDescriptionResponse>(
            _sharedContext,
            new EncryptionKeyDescriptionPost(
                _sharedContext.DbService.GetDbEntityMethods<EncryptionKeyDescriptionEntity>(),
                _sharedContext.DbService.GetDbMethods(),
                _sharedContext.TimeProvider,
                _sharedContext.Instrumentation,
                _sharedContext.SecretProvider,
                _sharedContext.EncryptionKeyInfo,
                _sharedContext.DbService.GetDbSettingsMethods(),
                _sharedContext.SettingsService));

    public ISoftwareStatementsContext SoftwareStatements
    {
        get
        {
            var softwareStatementOperations = new SoftwareStatementOperations(
                _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                _sharedContext.DbService.GetDbMethods(),
                _sharedContext.TimeProvider,
                _sharedContext.Instrumentation);
            return new SoftwareStatementsContext(
                _sharedContext,
                softwareStatementOperations,
                softwareStatementOperations);
        }
    }
}
