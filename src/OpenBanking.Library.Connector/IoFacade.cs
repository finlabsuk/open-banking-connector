// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace FinnovationLabs.OpenBanking.Library.Connector
{
    [ExcludeFromCodeCoverage]
    internal class IoFacade : IIoFacade
    {
        private readonly Func<string> _getContentPath;

        public IoFacade() : this(() => "C:\\") { }

        public IoFacade(Func<string> getContentPath)
        {
            _getContentPath = getContentPath.ArgNotNull(nameof(getContentPath));
        }

        public string GetContentPath()
        {
            return _getContentPath();
        }

        public IEnumerable<string> GetDirectoryFiles(string path, string filter)
        {
            return Directory.GetFiles(path: path, searchPattern: filter, searchOption: SearchOption.AllDirectories);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public void WriteFile(string path, byte[] bytes)
        {
            File.WriteAllBytes(path: path, bytes: bytes);
        }
    }
}
