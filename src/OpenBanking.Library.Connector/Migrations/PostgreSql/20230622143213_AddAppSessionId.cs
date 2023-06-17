using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddAppSessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "app_session_id",
                table: "auth_context",
                type: "text",
                nullable: false,
                defaultValue: "");
            
            migrationBuilder.Sql(@"UPDATE auth_context SET app_session_id = nonce;");

            migrationBuilder.CreateIndex(
                name: "ix_auth_context_app_session_id",
                table: "auth_context",
                column: "app_session_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_auth_context_app_session_id",
                table: "auth_context");

            migrationBuilder.DropColumn(
                name: "app_session_id",
                table: "auth_context");
        }
    }
}
