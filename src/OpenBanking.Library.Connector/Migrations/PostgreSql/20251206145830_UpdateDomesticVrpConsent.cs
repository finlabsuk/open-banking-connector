using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class UpdateDomesticVrpConsent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "schema_version",
                table: "settings",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.AddColumn<bool>(
                name: "migrated_to_v4",
                table: "domestic_vrp_consent",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "migrated_to_v4_modified",
                table: "domestic_vrp_consent",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "schema_version",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "migrated_to_v4",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "migrated_to_v4_modified",
                table: "domestic_vrp_consent");
        }
    }
}
