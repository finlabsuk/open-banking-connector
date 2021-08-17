// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public class FilePathBuilder
    {
        private readonly string _fileNameExtension;
        private readonly string _fileNameWithoutExtension;
        private readonly string _filePath;


        public FilePathBuilder(string filePath, string fileNameWithoutExtension, string fileNameExtension)
        {
            _filePath = filePath;
            _fileNameWithoutExtension = fileNameWithoutExtension;
            _fileNameExtension = fileNameExtension;
        }

        public FilePathBuilder AppendToPath(string subfolder)
        {
            string newPath = Path.Combine(_filePath, subfolder);

            // Create directory if doesn't exist
            Directory.CreateDirectory(newPath);

            return new FilePathBuilder(
                newPath,
                _fileNameWithoutExtension,
                _fileNameExtension);
        }

        public FilePathBuilder AppendToFileName(string text) =>
            new FilePathBuilder(
                _filePath,
                _fileNameWithoutExtension + text,
                _fileNameExtension);

        public string GetFilePath() => Path.Combine(
            _filePath,
            _fileNameWithoutExtension +
            _fileNameExtension);

        public async Task WriteFile<TData>(TData data)
            where TData : class
        {
            await DataFile.WriteFile(
                data,
                GetFilePath(),
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore // NULLs in C# map to JSON undefined to support
                    // compatibility with potential Node-JS
                    // port of OBC with TypeScript and use of undefined
                    // in "nullable" types
                });
        }
    }
}
