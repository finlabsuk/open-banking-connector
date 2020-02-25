using Microsoft.EntityFrameworkCore.Migrations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.SqliteMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoftwareStatementProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    SoftwareStatementHeaderBase64 = table.Column<string>(nullable: true),
                    SoftwareStatementPayloadBase64 = table.Column<string>(nullable: true),
                    SoftwareStatementPayload_SoftwareId = table.Column<string>(nullable: true),
                    SoftwareStatementPayload_SoftwareClientId = table.Column<string>(nullable: true),
                    SoftwareStatementPayload_SoftwareClientName = table.Column<string>(nullable: true),
                    SoftwareStatementPayload_SoftwareClientDescription = table.Column<string>(nullable: true),
                    SoftwareStatementPayload_SoftwareVersion = table.Column<float>(nullable: true),
                    SoftwareStatementPayload_SoftwareClientUri = table.Column<string>(nullable: true),
                    SoftwareStatementPayload_SoftwareRedirectUris = table.Column<string>(nullable: true),
                    SoftwareStatementPayload_SoftwareRoles = table.Column<string>(nullable: true),
                    SoftwareStatementPayload_OrgId = table.Column<string>(nullable: true),
                    SoftwareStatementPayload_OrgName = table.Column<string>(nullable: true),
                    SoftwwareStatementSignatureBase64 = table.Column<string>(nullable: true),
                    ObSigningKid = table.Column<string>(nullable: true),
                    ObSigningKey = table.Column<string>(nullable: true),
                    ObSigningPem = table.Column<string>(nullable: true),
                    ObTransportKey = table.Column<string>(nullable: true),
                    ObTransportPem = table.Column<string>(nullable: true),
                    DefaultFragmentRedirectUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareStatementProfiles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoftwareStatementProfiles");
        }
    }
}
