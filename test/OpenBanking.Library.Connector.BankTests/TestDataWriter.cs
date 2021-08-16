// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public class TestDataWriter
    {
        private readonly string _fileNamePrefix;
        private readonly string _testDataPath;


        public TestDataWriter(string testDataPath, string fileNamePrefix)
        {
            _testDataPath = testDataPath;
            _fileNamePrefix = fileNamePrefix;
        }

        public TestDataWriter AppendToPath(string subfolder)
        {
            string newPath = Path.Combine(_testDataPath, subfolder);

            // Create directory if doesn't exist
            Directory.CreateDirectory(newPath);

            return new TestDataWriter(
                newPath,
                _fileNamePrefix);
        }

        public TestDataWriter AppendToFileNamePrefix(string text) =>
            new TestDataWriter(
                _testDataPath,
                _fileNamePrefix + text);

        public string GetFilePath(string fileNamePostfix) => Path.Combine(
            _testDataPath,
            _fileNamePrefix +
            fileNamePostfix);

        public async Task ProcessData<TData>(TData data, string fileNamePostFix)
            where TData : class
        {
            string filePath = GetFilePath(fileNamePostFix);
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
