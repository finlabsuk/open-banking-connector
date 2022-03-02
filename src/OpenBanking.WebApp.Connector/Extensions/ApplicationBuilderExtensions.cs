// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Rewrite;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseDefaultFilesLocal(this IApplicationBuilder app)
    {
        // Re-direct literal URLs for default files
        RewriteOptions rewriterOptions = new RewriteOptions()
            .AddRedirect(
                @"^index.html$",
                "/");
        app.UseRewriter(rewriterOptions);

        // Add default files
        app.UseDefaultFiles();

        return app;
    }
}