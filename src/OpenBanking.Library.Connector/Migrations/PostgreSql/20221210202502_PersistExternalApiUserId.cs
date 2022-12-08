using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    public partial class PersistExternalApiUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "external_api_user_id",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "external_api_user_id_modified",
                table: "domestic_vrp_consent",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "external_api_user_id_modified_by",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "external_api_user_id",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "external_api_user_id_modified",
                table: "domestic_payment_consent",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "external_api_user_id_modified_by",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "id_token_sub_claim_type",
                table: "bank",
                type: "text",
                nullable: false,
                defaultValue: "ConsentId");

            migrationBuilder.AddColumn<string>(
                name: "external_api_user_id",
                table: "account_access_consent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "external_api_user_id_modified",
                table: "account_access_consent",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "external_api_user_id_modified_by",
                table: "account_access_consent",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "external_api_user_id",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "external_api_user_id_modified",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "external_api_user_id_modified_by",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "external_api_user_id",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "external_api_user_id_modified",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "external_api_user_id_modified_by",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "id_token_sub_claim_type",
                table: "bank");

            migrationBuilder.DropColumn(
                name: "external_api_user_id",
                table: "account_access_consent");

            migrationBuilder.DropColumn(
                name: "external_api_user_id_modified",
                table: "account_access_consent");

            migrationBuilder.DropColumn(
                name: "external_api_user_id_modified_by",
                table: "account_access_consent");
        }
    }
}
