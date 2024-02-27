// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;
using ObSealCertificate =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request.ObSealCertificate;
using ObWacCertificate =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request.ObWacCertificate;
using SoftwareStatement =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request.SoftwareStatement;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.BankConfiguration;

public interface IManagementContext
{
    /// <summary>
    ///     API for BankRegistration objects.
    ///     A BankRegistration corresponds to an OAuth2 client registration with a Bank. Multiple BankRegistrations may be
    ///     created for the same bank.
    /// </summary>
    IBankRegistrationsContext BankRegistrations { get; }

    ILocalEntityContext<ObWacCertificate, IObWacCertificatePublicQuery,
            ObWacCertificateResponse, ObWacCertificateResponse>
        ObWacCertificates { get; }

    ILocalEntityContext<ObSealCertificate, IObSealCertificatePublicQuery,
            ObSealCertificateResponse, ObSealCertificateResponse>
        ObSealCertificates { get; }

    ILocalEntityContext<SoftwareStatement, ISoftwareStatementPublicQuery,
            SoftwareStatementResponse, SoftwareStatementResponse>
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
        BankRegistrations => new BankRegistrationsContextInternal(_sharedContext);

    public ILocalEntityContext<ObWacCertificate, IObWacCertificatePublicQuery, ObWacCertificateResponse,
        ObWacCertificateResponse> ObWacCertificates =>
        new LocalEntityContextInternal<ObWacCertificateEntity, ObWacCertificate, IObWacCertificatePublicQuery,
            ObWacCertificateResponse, ObWacCertificateResponse>(
            _sharedContext,
            new ObWacCertificatePost(
                _sharedContext.DbService.GetDbEntityMethodsClass<ObWacCertificateEntity>(),
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                _sharedContext.Instrumentation,
                _sharedContext.HttpClientSettingsProvider,
                _sharedContext.MemoryCache,
                _sharedContext.SecretProvider));

    public ILocalEntityContext<ObSealCertificate, IObSealCertificatePublicQuery, ObSealCertificateResponse,
        ObSealCertificateResponse> ObSealCertificates =>
        new LocalEntityContextInternal<ObSealCertificateEntity, ObSealCertificate, IObSealCertificatePublicQuery,
            ObSealCertificateResponse, ObSealCertificateResponse>(
            _sharedContext,
            new ObSealCertificatePost(
                _sharedContext.DbService.GetDbEntityMethodsClass<ObSealCertificateEntity>(),
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                _sharedContext.Instrumentation,
                _sharedContext.MemoryCache,
                _sharedContext.SecretProvider));

    public ILocalEntityContext<SoftwareStatement, ISoftwareStatementPublicQuery, SoftwareStatementResponse,
        SoftwareStatementResponse> SoftwareStatements =>
        new LocalEntityContextInternal<SoftwareStatementEntity, SoftwareStatement, ISoftwareStatementPublicQuery,
            SoftwareStatementResponse, SoftwareStatementResponse>(
            _sharedContext,
            new SoftwareStatementPost(
                _sharedContext.DbService.GetDbEntityMethodsClass<SoftwareStatementEntity>(),
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                _sharedContext.SoftwareStatementProfileCachedRepo,
                _sharedContext.Instrumentation,
                _sharedContext.DbService.GetDbEntityMethodsClass<ObSealCertificateEntity>(),
                _sharedContext.DbService.GetDbEntityMethodsClass<ObWacCertificateEntity>(),
                _sharedContext.SecretProvider,
                _sharedContext.HttpClientSettingsProvider,
                _sharedContext.MemoryCache));
}
