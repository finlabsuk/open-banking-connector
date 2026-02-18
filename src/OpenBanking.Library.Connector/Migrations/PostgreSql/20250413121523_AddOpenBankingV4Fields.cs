using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddOpenBankingV4Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "created_with_v4",
                table: "domestic_vrp_consent",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "created_with_v4",
                table: "domestic_payment_consent",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "aisp_use_v4",
                table: "bank_registration",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "pisp_use_v4",
                table: "bank_registration",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "vrp_use_v4",
                table: "bank_registration",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "created_with_v4",
                table: "account_access_consent",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_with_v4",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "created_with_v4",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "aisp_use_v4",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "pisp_use_v4",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "vrp_use_v4",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "created_with_v4",
                table: "account_access_consent");
        }
    }
}
