using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    public partial class UpdateBankConfigurationForProductionUse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "authorization_endpoint",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "custom_behaviour",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "dynamic_client_registration_api_version",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "registration_endpoint",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "token_endpoint",
                table: "bank_registration");

            migrationBuilder.RenameColumn(
                name: "software_statement_and_certificate_profile_override_case",
                table: "bank_registration",
                newName: "software_statement_profile_override");

            migrationBuilder.AlterColumn<string>(
                name: "issuer_url",
                table: "bank",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "authorization_endpoint",
                table: "bank",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "custom_behaviour",
                table: "bank",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dcr_api_version",
                table: "bank",
                type: "text",
                nullable: false,
                defaultValue: "Version3p2");

            migrationBuilder.AddColumn<string>(
                name: "default_response_mode",
                table: "bank",
                type: "text",
                nullable: false,
                defaultValue: "Fragment");

            migrationBuilder.AddColumn<string>(
                name: "jwks_uri",
                table: "bank",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "registration_endpoint",
                table: "bank",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "supports_sca",
                table: "bank",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "token_endpoint",
                table: "bank",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "authorization_endpoint",
                table: "bank");

            migrationBuilder.DropColumn(
                name: "custom_behaviour",
                table: "bank");

            migrationBuilder.DropColumn(
                name: "dcr_api_version",
                table: "bank");

            migrationBuilder.DropColumn(
                name: "default_response_mode",
                table: "bank");

            migrationBuilder.DropColumn(
                name: "jwks_uri",
                table: "bank");

            migrationBuilder.DropColumn(
                name: "registration_endpoint",
                table: "bank");

            migrationBuilder.DropColumn(
                name: "supports_sca",
                table: "bank");

            migrationBuilder.DropColumn(
                name: "token_endpoint",
                table: "bank");

            migrationBuilder.RenameColumn(
                name: "software_statement_profile_override",
                table: "bank_registration",
                newName: "software_statement_and_certificate_profile_override_case");

            migrationBuilder.AddColumn<string>(
                name: "authorization_endpoint",
                table: "bank_registration",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "custom_behaviour",
                table: "bank_registration",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dynamic_client_registration_api_version",
                table: "bank_registration",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "registration_endpoint",
                table: "bank_registration",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "token_endpoint",
                table: "bank_registration",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "issuer_url",
                table: "bank",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
