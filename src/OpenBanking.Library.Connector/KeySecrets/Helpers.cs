// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets
{
    /// <summary>
    ///     Helper methods for Key Secrets.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        ///     Key name based on settings class, dictionary key (ID) and value property name.
        ///     (This is really designed for use with <see cref="SoftwareStatementProfilesSettings" />.)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string KeyWithId<TSettings>(string id, string propertyName)
            where TSettings : class, ISettings<TSettings>, new() =>
            $"{new TSettings().SettingsGroupName}:{id}:{propertyName}";

        /// <summary>
        ///     Assemble object from individual key secrets where object properties
        ///     are either string or <see cref="List{T}" />
        /// </summary>
        /// <param name="keyFcn"></param>
        /// <param name="keySecretReadOnlyProvider"></param>
        /// <typeparam name="TSettingsElement"></typeparam>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public static async Task<TSettingsElement> GetAsync<TSettingsElement>(
            Func<string, string> keyFcn,
            IKeySecretProvider keySecretReadOnlyProvider)
        {
            var values = new List<object>();
            foreach (PropertyInfo property in typeof(TSettingsElement).GetProperties())
            {
                string key = keyFcn(property.Name);
                KeySecret keySecret = await keySecretReadOnlyProvider.GetKeySecretAsync(key) ??
                                      throw new KeyNotFoundException($"No key secret found for key {key}.");

                if (property.PropertyType == typeof(string))
                {
                    values.Add(keySecret.Value);
                }
                else if (property.PropertyType == typeof(List<string>))
                {
                    List<string> processedValue = keySecret.Value.Split(" ").ToList();
                    values.Add(processedValue);
                }
                else
                {
                    throw new InvalidDataException(
                        $"Properties of type {typeof(TSettingsElement)} are stored as key secrets and should be of either string or List<string> type.");
                }
            }

            return (TSettingsElement) Activator.CreateInstance(typeof(TSettingsElement), values.ToArray<object>())!;
        }
    }
}
