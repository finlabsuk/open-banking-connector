// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

namespace FinnovationLabs.OpenBanking.Library.Connector.Extensions
{
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Converts from assembly location (e.g. "OpenBanking.Library.Connector.BankTests\bin\Debug\netcoreapp3.1\FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.dll")
        /// to project folder (e.g. "OpenBanking.Library.Connector.BankTests"). We use this to get project folder to avoid
        /// making assumptions about current directory.
        /// </summary>
        /// <param name="assemblyLocation"></param>
        /// <returns></returns>
        public static DirectoryInfo GetProjectFolderFromAssemblyLocation(this DirectoryInfo assemblyLocation)
        {
            return assemblyLocation.Parent?.Parent?.Parent?.Parent ??
                   throw new Exception("Can't determine project folder from assembly location.");
        }
    }
}
