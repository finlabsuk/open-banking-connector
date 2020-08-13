// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.FileProviders;

namespace FinnovationLabs.OpenBanking.Library.Connector.WebHost
{
    public class Helpers
    {
        public static void AddStaticFiles(IApplicationBuilder app, string pathToWebHostProjectRoot)
        {
            // Use URLs without ".html"
            RewriteOptions rewriterOptions = new RewriteOptions()
                .AddRedirect(
                    regex: @"^auth/fragment-redirect.html$",
                    replacement: "auth/fragment-redirect")
                .AddRewrite(
                    regex: @"^auth/fragment-redirect$",
                    replacement: "auth/fragment-redirect.html",
                    skipRemainingRules: false)
                .AddRedirect(
                    regex: @"^index.html$",
                    replacement: "index")
                .AddRewrite(
                    regex: @"^index$",
                    replacement: "index.html",
                    skipRemainingRules: false);
            app.UseRewriter(rewriterOptions);

            // Add default page
            DefaultFilesOptions fileOptions = new DefaultFilesOptions();
            fileOptions.DefaultFileNames.Clear();
            fileOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(fileOptions);

            // Add local static files
            app.UseStaticFiles();

            // Add shared static files            
            string? path = Path.Combine(
                path1: pathToWebHostProjectRoot,
                path2: "wwwroot");
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(path)
                });
        }
    }
}
