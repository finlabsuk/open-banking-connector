// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public static class FolderPaths
    {
        private static readonly Lazy<string> ProjectRootField = new(
            () =>
            {
                string assemblyLocation = typeof(FolderPaths).Assembly.Location;
                string projectRoot = Path.GetFullPath(Path.Join(assemblyLocation, "..", "..", "..", ".."));
                string projectRootName = new DirectoryInfo(projectRoot).Name;
                if (projectRootName != "OpenBanking.Library.Connector.BankTests")
                {
                    throw new DirectoryNotFoundException(
                        "Can't locate bank tests project root folder. " +
                        "It is necessary to do this to locate data and Consent Authoriser folders");
                }

                return projectRoot;
            });

        private static readonly Lazy<string> ConsentAuthoriserFolderField = new(
            () =>
            {
                string consentAuthoriserFolder =
                    Path.GetFullPath(Path.Join(ProjectRootField.Value, "..", "BankTestsConsentAuthoriser"));
                if (!Directory.Exists(consentAuthoriserFolder))
                {
                    throw new DirectoryNotFoundException(
                        "Can't locate Consent Authoriser root folder. " +
                        "It is necessary to do this to run the Consent Authoriser.");
                }

                return consentAuthoriserFolder;
            });

        private static readonly Lazy<string> ConsentAuthoriserBuildFolderField = new(
            () =>
            {
                string consentAuthoriserBuildFolder =
                    Path.GetFullPath(Path.Join(ConsentAuthoriserFolderField.Value, ".build"));
                if (!Directory.Exists(consentAuthoriserBuildFolder))
                {
                    throw new DirectoryNotFoundException(
                        "Can't locate Consent Authoriser build folder. " +
                        "Please run tsc in folder {consentAuthoriserFolder} to compile TypeScript.");
                }

                return consentAuthoriserBuildFolder;
            });

        public static string ProjectRoot => ProjectRootField.Value;
        public static string ConsentAuthoriserBuildFolder => ConsentAuthoriserBuildFolderField.Value;
    }
}
