using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddDefaultQueryRedirectUri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bank_registration_group",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "other_redirect_uris",
                table: "bank_registration");
            
            migrationBuilder.DropColumn(
                name: "default_response_mode",
                table: "bank_registration");
            
            migrationBuilder.AddColumn<string>(
                name: "default_query_redirect_uri",
                table: "bank_registration",
                type: "text",
                nullable: false, defaultValue:"");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "default_query_redirect_uri",
                table: "bank_registration");
            
            migrationBuilder.AddColumn<string>(
                name: "default_response_mode",
                table: "bank_registration",
                type: "text",
                nullable: false, defaultValue:"Fragment");

            migrationBuilder.AddColumn<string>(
                name: "bank_registration_group",
                table: "bank_registration",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "other_redirect_uris",
                table: "bank_registration",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
