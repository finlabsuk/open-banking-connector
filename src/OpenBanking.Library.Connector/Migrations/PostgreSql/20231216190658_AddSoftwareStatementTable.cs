using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddSoftwareStatementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "software_statement_id",
                table: "bank_registration",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ob_seal_certificate",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    associated_key_id = table.Column<string>(type: "text", nullable: false),
                    associated_key = table.Column<string>(type: "jsonb", nullable: false),
                    certificate = table.Column<string>(type: "text", nullable: false),
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ob_seal_certificate", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ob_wac_certificate",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    associated_key = table.Column<string>(type: "jsonb", nullable: false),
                    certificate = table.Column<string>(type: "text", nullable: false),
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ob_wac_certificate", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "software_statement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organisation_id = table.Column<string>(type: "text", nullable: false),
                    software_id = table.Column<string>(type: "text", nullable: false),
                    sandbox_environment = table.Column<bool>(type: "boolean", nullable: false),
                    default_ob_wac_certificate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    default_ob_seal_certificate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    default_query_redirect_url = table.Column<string>(type: "text", nullable: false),
                    default_fragment_redirect_url = table.Column<string>(type: "text", nullable: false),
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_software_statement", x => x.id);
                    table.ForeignKey(
                        name: "fk_software_statement_ob_seal_certificate_default_ob_seal_certifi",
                        column: x => x.default_ob_seal_certificate_id,
                        principalTable: "ob_seal_certificate",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_software_statement_ob_wac_certificate_default_ob_wac_certifica",
                        column: x => x.default_ob_wac_certificate_id,
                        principalTable: "ob_wac_certificate",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bank_registration_software_statement_id",
                table: "bank_registration",
                column: "software_statement_id");

            migrationBuilder.CreateIndex(
                name: "ix_software_statement_default_ob_seal_certificate_id",
                table: "software_statement",
                column: "default_ob_seal_certificate_id");

            migrationBuilder.CreateIndex(
                name: "ix_software_statement_default_ob_wac_certificate_id",
                table: "software_statement",
                column: "default_ob_wac_certificate_id");

            migrationBuilder.AddForeignKey(
                name: "fk_bank_registration_software_statement_software_statement_id",
                table: "bank_registration",
                column: "software_statement_id",
                principalTable: "software_statement",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bank_registration_software_statement_software_statement_id",
                table: "bank_registration");

            migrationBuilder.DropTable(
                name: "software_statement");

            migrationBuilder.DropTable(
                name: "ob_seal_certificate");

            migrationBuilder.DropTable(
                name: "ob_wac_certificate");

            migrationBuilder.DropIndex(
                name: "ix_bank_registration_software_statement_id",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "software_statement_id",
                table: "bank_registration");
        }
    }
}
