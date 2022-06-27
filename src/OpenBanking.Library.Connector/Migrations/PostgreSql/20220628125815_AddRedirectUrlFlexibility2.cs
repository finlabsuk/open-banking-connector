using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    public partial class AddRedirectUrlFlexibility2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "nonce_modified",
                table: "domestic_vrp_consent",
                newName: "auth_context_modified");

            migrationBuilder.RenameColumn(
                name: "nonce",
                table: "domestic_vrp_consent",
                newName: "auth_context_nonce");

            migrationBuilder.RenameColumn(
                name: "nonce_modified",
                table: "domestic_payment_consent",
                newName: "auth_context_modified");

            migrationBuilder.RenameColumn(
                name: "nonce",
                table: "domestic_payment_consent",
                newName: "auth_context_nonce");

            migrationBuilder.RenameColumn(
                name: "nonce_modified",
                table: "account_access_consent",
                newName: "auth_context_modified");

            migrationBuilder.RenameColumn(
                name: "nonce",
                table: "account_access_consent",
                newName: "auth_context_nonce");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "auth_context_nonce",
                table: "domestic_vrp_consent",
                newName: "nonce");

            migrationBuilder.RenameColumn(
                name: "auth_context_modified",
                table: "domestic_vrp_consent",
                newName: "nonce_modified");

            migrationBuilder.RenameColumn(
                name: "auth_context_nonce",
                table: "domestic_payment_consent",
                newName: "nonce");

            migrationBuilder.RenameColumn(
                name: "auth_context_modified",
                table: "domestic_payment_consent",
                newName: "nonce_modified");

            migrationBuilder.RenameColumn(
                name: "auth_context_nonce",
                table: "account_access_consent",
                newName: "nonce");

            migrationBuilder.RenameColumn(
                name: "auth_context_modified",
                table: "account_access_consent",
                newName: "nonce_modified");
        }
    }
}
