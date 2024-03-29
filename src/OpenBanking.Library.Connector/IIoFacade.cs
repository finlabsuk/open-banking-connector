﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector;

internal interface IIoFacade
{
    string GetContentPath();

    IEnumerable<string> GetDirectoryFiles(string path, string filter);

    bool FileExists(string path);

    void WriteFile(string path, byte[] bytes);
}
