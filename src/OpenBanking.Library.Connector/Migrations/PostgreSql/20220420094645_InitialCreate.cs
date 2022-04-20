using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bank",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    issuer_url = table.Column<string>(type: "text", nullable: false),
                    financial_id = table.Column<string>(type: "text", nullable: false),
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
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
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
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
                name: "bank_registration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_id = table.Column<Guid>(type: "uuid", nullable: false),
                    software_statement_profile_id = table.Column<string>(type: "text", nullable: false),
                    software_statement_and_certificate_profile_override_case = table.Column<string>(type: "text", nullable: true),
                    dynamic_client_registration_api_version = table.Column<string>(type: "text", nullable: false),
                    registration_scope = table.Column<string>(type: "text", nullable: false),
                    registration_endpoint = table.Column<string>(type: "text", nullable: false),
                    token_endpoint = table.Column<string>(type: "text", nullable: false),
                    authorization_endpoint = table.Column<string>(type: "text", nullable: false),
                    token_endpoint_auth_method = table.Column<string>(type: "text", nullable: false),
                    custom_behaviour = table.Column<string>(type: "text", nullable: true),
                    external_api_id = table.Column<string>(type: "text", nullable: false),
                    external_api_secret = table.Column<string>(type: "text", nullable: true),
                    registration_access_token = table.Column<string>(type: "text", nullable: true),
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bank_registration", x => x.id);
                    table.ForeignKey(
                        name: "fk_bank_registration_bank_bank_id",
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
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
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
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "account_access_consent",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_registration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_and_transaction_api_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_api_id = table.Column<string>(type: "text", nullable: false),
                    access_token_access_token = table.Column<string>(type: "text", nullable: true),
                    access_token_expires_in = table.Column<int>(type: "integer", nullable: false),
                    access_token_refresh_token = table.Column<string>(type: "text", nullable: true),
                    access_token_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    access_token_modified_by = table.Column<string>(type: "text", nullable: true),
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account_access_consent", x => x.id);
                    table.ForeignKey(
                        name: "fk_account_access_consent_account_and_transaction_api_account_a",
                        column: x => x.account_and_transaction_api_id,
                        principalTable: "account_and_transaction_api",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_account_access_consent_bank_registration_bank_registration_id",
                        column: x => x.bank_registration_id,
                        principalTable: "bank_registration",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "domestic_payment_consent",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_registration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_initiation_api_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_api_id = table.Column<string>(type: "text", nullable: false),
                    access_token_access_token = table.Column<string>(type: "text", nullable: true),
                    access_token_expires_in = table.Column<int>(type: "integer", nullable: false),
                    access_token_refresh_token = table.Column<string>(type: "text", nullable: true),
                    access_token_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    access_token_modified_by = table.Column<string>(type: "text", nullable: true),
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_domestic_payment_consent", x => x.id);
                    table.ForeignKey(
                        name: "fk_domestic_payment_consent_bank_registration_bank_registration",
                        column: x => x.bank_registration_id,
                        principalTable: "bank_registration",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_domestic_payment_consent_payment_initiation_api_payment_init",
                        column: x => x.payment_initiation_api_id,
                        principalTable: "payment_initiation_api",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "domestic_vrp_consent",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_registration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    variable_recurring_payments_api_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_api_id = table.Column<string>(type: "text", nullable: false),
                    access_token_access_token = table.Column<string>(type: "text", nullable: true),
                    access_token_expires_in = table.Column<int>(type: "integer", nullable: false),
                    access_token_refresh_token = table.Column<string>(type: "text", nullable: true),
                    access_token_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    access_token_modified_by = table.Column<string>(type: "text", nullable: true),
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_domestic_vrp_consent", x => x.id);
                    table.ForeignKey(
                        name: "fk_domestic_vrp_consent_bank_registration_bank_registration_id",
                        column: x => x.bank_registration_id,
                        principalTable: "bank_registration",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_domestic_vrp_consent_variable_recurring_payments_api_variab",
                        column: x => x.variable_recurring_payments_api_id,
                        principalTable: "variable_recurring_payments_api",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "auth_context",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    discriminator = table.Column<string>(type: "text", nullable: false),
                    account_access_consent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    domestic_payment_consent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    domestic_vrp_consent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auth_context", x => x.id);
                    table.ForeignKey(
                        name: "fk_auth_context_account_access_consent_account_access_consent_id",
                        column: x => x.account_access_consent_id,
                        principalTable: "account_access_consent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_auth_context_domestic_payment_consent_domestic_payment_consen",
                        column: x => x.domestic_payment_consent_id,
                        principalTable: "domestic_payment_consent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_auth_context_domestic_vrp_consent_domestic_vrp_consent_id",
                        column: x => x.domestic_vrp_consent_id,
                        principalTable: "domestic_vrp_consent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_account_access_consent_account_and_transaction_api_id",
                table: "account_access_consent",
                column: "account_and_transaction_api_id");

            migrationBuilder.CreateIndex(
                name: "ix_account_access_consent_bank_registration_id",
                table: "account_access_consent",
                column: "bank_registration_id");

            migrationBuilder.CreateIndex(
                name: "ix_account_and_transaction_api_bank_id",
                table: "account_and_transaction_api",
                column: "bank_id");

            migrationBuilder.CreateIndex(
                name: "ix_auth_context_account_access_consent_id",
                table: "auth_context",
                column: "account_access_consent_id");

            migrationBuilder.CreateIndex(
                name: "ix_auth_context_domestic_payment_consent_id",
                table: "auth_context",
                column: "domestic_payment_consent_id");

            migrationBuilder.CreateIndex(
                name: "ix_auth_context_domestic_vrp_consent_id",
                table: "auth_context",
                column: "domestic_vrp_consent_id");

            migrationBuilder.CreateIndex(
                name: "ix_bank_registration_bank_id",
                table: "bank_registration",
                column: "bank_id");

            migrationBuilder.CreateIndex(
                name: "ix_domestic_payment_consent_bank_registration_id",
                table: "domestic_payment_consent",
                column: "bank_registration_id");

            migrationBuilder.CreateIndex(
                name: "ix_domestic_payment_consent_payment_initiation_api_id",
                table: "domestic_payment_consent",
                column: "payment_initiation_api_id");

            migrationBuilder.CreateIndex(
                name: "ix_domestic_vrp_consent_bank_registration_id",
                table: "domestic_vrp_consent",
                column: "bank_registration_id");

            migrationBuilder.CreateIndex(
                name: "ix_domestic_vrp_consent_variable_recurring_payments_api_id",
                table: "domestic_vrp_consent",
                column: "variable_recurring_payments_api_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_initiation_api_bank_id",
                table: "payment_initiation_api",
                column: "bank_id");

            migrationBuilder.CreateIndex(
                name: "ix_variable_recurring_payments_api_bank_id",
                table: "variable_recurring_payments_api",
                column: "bank_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auth_context");

            migrationBuilder.DropTable(
                name: "account_access_consent");

            migrationBuilder.DropTable(
                name: "domestic_payment_consent");

            migrationBuilder.DropTable(
                name: "domestic_vrp_consent");

            migrationBuilder.DropTable(
                name: "account_and_transaction_api");

            migrationBuilder.DropTable(
                name: "payment_initiation_api");

            migrationBuilder.DropTable(
                name: "bank_registration");

            migrationBuilder.DropTable(
                name: "variable_recurring_payments_api");

            migrationBuilder.DropTable(
                name: "bank");
        }
    }
}
