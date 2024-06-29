using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class SupportPkce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_and_transaction_api");

            migrationBuilder.DropTable(
                name: "payment_initiation_api");

            migrationBuilder.DropTable(
                name: "variable_recurring_payments_api");

            migrationBuilder.DropTable(
                name: "bank");

            migrationBuilder.AddColumn<string>(
                name: "auth_context_code_verifier",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "auth_context_code_verifier",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "code_verifier",
                table: "auth_context",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "auth_context_code_verifier",
                table: "account_access_consent",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_auth_context_code_verifier",
                table: "auth_context",
                column: "code_verifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_auth_context_code_verifier",
                table: "auth_context");

            migrationBuilder.DropColumn(
                name: "auth_context_code_verifier",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "auth_context_code_verifier",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "code_verifier",
                table: "auth_context");

            migrationBuilder.DropColumn(
                name: "auth_context_code_verifier",
                table: "account_access_consent");

            migrationBuilder.CreateTable(
                name: "bank",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    authorization_endpoint = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    custom_behaviour = table.Column<string>(type: "jsonb", nullable: true),
                    dcr_api_version = table.Column<string>(type: "text", nullable: false),
                    financial_id = table.Column<string>(type: "text", nullable: false),
                    id_token_sub_claim_type = table.Column<string>(type: "text", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    issuer_url = table.Column<string>(type: "text", nullable: false),
                    jwks_uri = table.Column<string>(type: "text", nullable: false),
                    reference = table.Column<string>(type: "text", nullable: true),
                    registration_endpoint = table.Column<string>(type: "text", nullable: true),
                    supports_sca = table.Column<bool>(type: "boolean", nullable: false),
                    token_endpoint = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bank", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "account_and_transaction_api",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_id = table.Column<Guid>(type: "uuid", nullable: false),
                    api_version = table.Column<string>(type: "text", nullable: false),
                    base_url = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    reference = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account_and_transaction_api", x => x.id);
                    table.ForeignKey(
                        name: "fk_account_and_transaction_api_bank_bank_id",
                        column: x => x.bank_id,
                        principalTable: "bank",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_initiation_api",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_id = table.Column<Guid>(type: "uuid", nullable: false),
                    api_version = table.Column<string>(type: "text", nullable: false),
                    base_url = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    reference = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_initiation_api", x => x.id);
                    table.ForeignKey(
                        name: "fk_payment_initiation_api_bank_bank_id",
                        column: x => x.bank_id,
                        principalTable: "bank",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "variable_recurring_payments_api",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_id = table.Column<Guid>(type: "uuid", nullable: false),
                    api_version = table.Column<string>(type: "text", nullable: false),
                    base_url = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    reference = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_variable_recurring_payments_api", x => x.id);
                    table.ForeignKey(
                        name: "fk_variable_recurring_payments_api_bank_bank_id",
                        column: x => x.bank_id,
                        principalTable: "bank",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_account_and_transaction_api_bank_id",
                table: "account_and_transaction_api",
                column: "bank_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_initiation_api_bank_id",
                table: "payment_initiation_api",
                column: "bank_id");

            migrationBuilder.CreateIndex(
                name: "ix_variable_recurring_payments_api_bank_id",
                table: "variable_recurring_payments_api",
                column: "bank_id");
        }
    }
}
