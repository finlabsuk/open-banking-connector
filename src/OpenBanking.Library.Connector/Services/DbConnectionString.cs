// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Services;

public class DbConnectionString : IDbConnectionString
{
    private readonly Lazy<string> _connectionString;

    public DbConnectionString(DatabaseSettings settings, ISecretProvider secretProvider)
    {
        _connectionString = new Lazy<string>(
            () =>
            {
                if (!settings.ConnectionStrings.TryGetValue(settings.Provider, out string? connectionString))
                {
                    throw new ArgumentException(
                        $"No database connection string found for provider {settings.Provider}.");
                }

                string? password = null;
                if (settings.PasswordSettingNames.TryGetValue(settings.Provider, out string? passwordSettingName))
                {
                    SecretSource passowrdSource = settings.PasswordSources[settings.Provider];
                    var passwordSecret = new SecretDescription
                    {
                        Name = passwordSettingName,
                        Source = passowrdSource
                    };

                    // Attempt to obtain password
                    SecretResult passwordResult =
                        secretProvider.GetSecretAsync(passwordSecret).GetAwaiter().GetResult();
                    if (!passwordResult.SecretObtained)
                    {
                        string fullMessage =
                            $"Database settings specify specify password for provider {settings.Provider} with Source " +
                            $"{passowrdSource} and Name {passwordSettingName} " +
                            $"which could not be obtained. {passwordResult.ErrorMessage}";
                        throw new ArgumentException(fullMessage);
                    }

                    password = passwordResult.Secret!;
                    if (string.IsNullOrEmpty(password))
                    {
                        throw new ArgumentException(
                            $"Received empty string password for database provider {settings.Provider}.");
                    }
                }

                return settings.Provider switch
                {
                    DbProvider.Sqlite => connectionString,
                    DbProvider.PostgreSql => connectionString + (password is not null ? $";Password={password}" : ""),
                    DbProvider.MongoDb => AddMongoDbPassword(connectionString, password),
                    _ => throw new ArgumentOutOfRangeException(nameof(settings.Provider), settings.Provider, null)
                };
            });
    }

    public string GetConnectionString() => _connectionString.Value;

    private string AddMongoDbPassword(string connectionString, string? password)
    {
        if (password is null)
        {
            return connectionString;
        }
        int insertionIdx = connectionString.IndexOf('@');
        return connectionString.Insert(insertionIdx, $":{password}");
    }
}
