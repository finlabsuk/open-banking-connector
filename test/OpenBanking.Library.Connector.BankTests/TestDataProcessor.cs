// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public class TestDataProcessor
    {
        private readonly string _dataFolderPath;

        private readonly string _testFolderRelPath;

        public TestDataProcessor(string dataFolderPath, string testFolderRelPath)
        {
            _dataFolderPath = dataFolderPath;
            _testFolderRelPath = testFolderRelPath;
        }

        public TestDataProcessor AddTestSubfolder(string subfolder)
        {
            string testFolder = Path.Combine(_testFolderRelPath, subfolder);
            return new TestDataProcessor(_dataFolderPath, testFolder);
        }

        public async Task ProcessData<TData>(TData data)
            where TData : class
        {
            string dirPath = Path.Join(
                _dataFolderPath,
                _testFolderRelPath);
            string filePath = Path.Join(
                dirPath,
                $"{typeof(TData).Name.Replace('.', '_')}.json");
            Directory.CreateDirectory(dirPath);
            string dataToWrite = JsonConvert.SerializeObject(
                data,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore // NULLs map to JSON undefined to support
                    // compatibility with potential Node-JS
                    // port of OBC with TypeScript and use of undefined
                    // in "nullable" types
                });

            // Only write to file if necessary
            if (File.Exists(filePath))
            {
                string fileContents = await File.ReadAllTextAsync(filePath);
                if (fileContents == dataToWrite)
                {
                    return;
                }
            }

            // Write to file.
            // Sometimes lock is not immediately released from another test so need to try a couple of times.
            var maxAttempts = 2;
            var currentAttempt = 0;
            while (currentAttempt < maxAttempts)
            {
                try
                {
                    await File.WriteAllTextAsync(
                        filePath,
                        dataToWrite);
                    return;
                }
                catch (IOException)
                {
                    Thread.Sleep(50);
                    currentAttempt++;
                    if (currentAttempt == maxAttempts)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
