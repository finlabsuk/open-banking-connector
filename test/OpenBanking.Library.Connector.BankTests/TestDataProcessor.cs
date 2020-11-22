// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public class TestDataProcessor
    {
        private readonly string _projectRootPath;

        private readonly string _testFolderRelPath;

        public TestDataProcessor(string projectRootPath, string testFolderRelPath)
        {
            _projectRootPath = projectRootPath;
            _testFolderRelPath = testFolderRelPath;
        }

        public TestDataProcessor AddTestSubfolder(string subfolder)
        {
            string testFolder = Path.Combine(path1: _testFolderRelPath, path2: subfolder);
            return new TestDataProcessor(projectRootPath: _projectRootPath, testFolderRelPath: testFolder);
        }

        public void ProcessData<TData>(TData data) where TData : class
        {
            string filePath = Path.Combine(
                path1: _projectRootPath,
                path2: "data",
                path3: _testFolderRelPath,
                path4: $"{typeof(TData).Name.Replace(oldChar: '.', newChar: '_')}.json");
            FileInfo file = new FileInfo(filePath);
            file.Directory?.Create();
            File.WriteAllText(
                path: file.FullName,
                contents: JsonConvert.SerializeObject(
                    value: data,
                    formatting: Formatting.Indented,
                    settings: new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore // NULLs map to JSON undefined to support
                        // compatibility with potential Node-JS
                        // port of OBC with TypeScript and use of undefined
                        // in "nullable" types
                    }));
        }
    }
}
