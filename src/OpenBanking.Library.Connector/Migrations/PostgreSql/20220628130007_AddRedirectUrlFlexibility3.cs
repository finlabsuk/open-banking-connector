using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    public partial class AddRedirectUrlFlexibility3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "nonce_modified_by",
                table: "domestic_vrp_consent",
                newName: "auth_context_modified_by");

            migrationBuilder.RenameColumn(
                name: "nonce_modified_by",
                table: "domestic_payment_consent",
                newName: "auth_context_modified_by");

            migrationBuilder.RenameColumn(
                name: "nonce_modified_by",
                table: "account_access_consent",
                newName: "auth_context_modified_by");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "auth_context_modified_by",
                table: "domestic_vrp_consent",
                newName: "nonce_modified_by");

            migrationBuilder.RenameColumn(
                name: "auth_context_modified_by",
                table: "domestic_payment_consent",
                newName: "nonce_modified_by");

            migrationBuilder.RenameColumn(
                name: "auth_context_modified_by",
                table: "account_access_consent",
                newName: "nonce_modified_by");
        }
    }
}
