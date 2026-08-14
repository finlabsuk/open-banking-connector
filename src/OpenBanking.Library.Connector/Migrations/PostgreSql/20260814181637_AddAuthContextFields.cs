using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddAuthContextFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "default_query_redirect_url",
                table: "software_statement",
                newName: "default_query_redirect_uri");

            migrationBuilder.RenameColumn(
                name: "default_fragment_redirect_url",
                table: "software_statement",
                newName: "default_fragment_redirect_uri");

            migrationBuilder.AddColumn<string>(
                name: "auth_context_acr",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "auth_context_auth_time",
                table: "domestic_vrp_consent",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "auth_context_acr",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "auth_context_auth_time",
                table: "domestic_payment_consent",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "redirect_uri",
                table: "auth_context",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "auth_context_acr",
                table: "account_access_consent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "auth_context_auth_time",
                table: "account_access_consent",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql("UPDATE settings SET schema_version = 2;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auth_context_acr",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "auth_context_auth_time",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "auth_context_acr",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "auth_context_auth_time",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "redirect_uri",
                table: "auth_context");

            migrationBuilder.DropColumn(
                name: "auth_context_acr",
                table: "account_access_consent");

            migrationBuilder.DropColumn(
                name: "auth_context_auth_time",
                table: "account_access_consent");

            migrationBuilder.RenameColumn(
                name: "default_query_redirect_uri",
                table: "software_statement",
                newName: "default_query_redirect_url");

            migrationBuilder.RenameColumn(
                name: "default_fragment_redirect_uri",
                table: "software_statement",
                newName: "default_fragment_redirect_url");
        }
    }
}
