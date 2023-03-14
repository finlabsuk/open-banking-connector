// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.FileProviders;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseWebHostStaticFiles(this IApplicationBuilder app)
    {
        // Standardised URL without ".html"
        RewriteOptions rewriterOptions = new RewriteOptions()
            .AddRedirect(
                @"^auth/fragment-redirect.html$",
                "auth/fragment-redirect")
            .AddRewrite(
                @"^auth/fragment-redirect$",
                "auth/fragment-redirect.html",
                false);
        app.UseRewriter(rewriterOptions);

        // Add shared static files from this assembly
        var webHostAssembly = Assembly.GetAssembly(typeof(ApplicationBuilderExtensions))!;
        app.UseStaticFiles(
            new StaticFileOptions
            {
                FileProvider = new ManifestEmbeddedFileProvider(
                    webHostAssembly,
                    "wwwroot")
            });

        return app;
    }
}
