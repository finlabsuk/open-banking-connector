﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace FinnovationLabs.OpenBanking.Library.Connector;

[ExcludeFromCodeCoverage]
internal class IoFacade : IIoFacade
{
    private readonly Func<string> _getContentPath;

    public IoFacade() : this(() => "C:\\") { }

    public IoFacade(Func<string> getContentPath)
    {
        _getContentPath = getContentPath.ArgNotNull(nameof(getContentPath));
    }

    public string GetContentPath() => _getContentPath();

    public IEnumerable<string> GetDirectoryFiles(string path, string filter) =>
        Directory.GetFiles(path, filter, SearchOption.AllDirectories);

    public bool FileExists(string path) => File.Exists(path);

    public void WriteFile(string path, byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
    }
}
