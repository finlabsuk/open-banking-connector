// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Utility
{
    public static class DataFile
    {
        public static async Task<TData> ReadFile<TData>(
            string readFile,
            JsonSerializerSettings? jsonSerializerSettings)
        {
            string fileText = await File.ReadAllTextAsync(readFile);
            TData apiResponse =
                JsonConvert.DeserializeObject<TData>(
                    fileText,
                    jsonSerializerSettings) ??
                throw new Exception("Can't de-serialise supplied bank API response");
            return apiResponse;
        }

        public static async Task WriteFile<TData>(
            TData data,
            string writeFile,
            JsonSerializerSettings? jsonSerializerSettings)
        {
            string dataToWrite = JsonConvert.SerializeObject(
                data,
                Formatting.Indented,
                jsonSerializerSettings);

            // Only write to file if necessary
            if (File.Exists(writeFile))
            {
                string fileContents = await File.ReadAllTextAsync(writeFile);
                if (fileContents == dataToWrite)
                {
                    return;
                }
            }

            // Write to file.
            // Sometimes lock is not immediately released so need to try a couple of times.
            // This code should only be used for test and debug.
            var maxAttempts = 2;
            var currentAttempt = 0;
            while (currentAttempt < maxAttempts)
            {
                try
                {
                    await File.WriteAllTextAsync(
                        writeFile,
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
