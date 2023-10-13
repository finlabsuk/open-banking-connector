using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddUseSimulatedBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_account_access_consent_account_and_transaction_api_account_a",
                table: "account_access_consent");

            migrationBuilder.DropForeignKey(
                name: "fk_bank_registration_bank_bank_id",
                table: "bank_registration");

            migrationBuilder.DropForeignKey(
                name: "fk_domestic_payment_consent_payment_initiation_api_payment_init",
                table: "domestic_payment_consent");

            migrationBuilder.DropForeignKey(
                name: "fk_domestic_vrp_consent_variable_recurring_payments_api_variab",
                table: "domestic_vrp_consent");

            migrationBuilder.DropIndex(
                name: "ix_domestic_vrp_consent_variable_recurring_payments_api_id",
                table: "domestic_vrp_consent");

            migrationBuilder.DropIndex(
                name: "ix_domestic_payment_consent_payment_initiation_api_id",
                table: "domestic_payment_consent");

            migrationBuilder.DropIndex(
                name: "ix_bank_registration_bank_id",
                table: "bank_registration");

            migrationBuilder.DropIndex(
                name: "ix_account_access_consent_account_and_transaction_api_id",
                table: "account_access_consent");

            migrationBuilder.DropColumn(
                name: "variable_recurring_payments_api_id",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "payment_initiation_api_id",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "bank_id",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "account_and_transaction_api_id",
                table: "account_access_consent");

            migrationBuilder.RenameColumn(
                name: "default_redirect_uri",
                table: "bank_registration",
                newName: "default_fragment_redirect_uri");

            migrationBuilder.AddColumn<string>(
                name: "redirect_uris",
                table: "bank_registration",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<bool>(
                name: "use_simulated_bank",
                table: "bank_registration",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "redirect_uris",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "use_simulated_bank",
                table: "bank_registration");

            migrationBuilder.RenameColumn(
                name: "default_fragment_redirect_uri",
                table: "bank_registration",
                newName: "default_redirect_uri");

            migrationBuilder.AddColumn<Guid>(
                name: "variable_recurring_payments_api_id",
                table: "domestic_vrp_consent",
                type: "uuid",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 10);

            migrationBuilder.AddColumn<Guid>(
                name: "payment_initiation_api_id",
                table: "domestic_payment_consent",
                type: "uuid",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 10);

            migrationBuilder.AddColumn<Guid>(
                name: "bank_id",
                table: "bank_registration",
                type: "uuid",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AddColumn<Guid>(
                name: "account_and_transaction_api_id",
                table: "account_access_consent",
                type: "uuid",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 10);

            migrationBuilder.CreateIndex(
                name: "ix_domestic_vrp_consent_variable_recurring_payments_api_id",
                table: "domestic_vrp_consent",
                column: "variable_recurring_payments_api_id");

            migrationBuilder.CreateIndex(
                name: "ix_domestic_payment_consent_payment_initiation_api_id",
                table: "domestic_payment_consent",
                column: "payment_initiation_api_id");

            migrationBuilder.CreateIndex(
                name: "ix_bank_registration_bank_id",
                table: "bank_registration",
                column: "bank_id");

            migrationBuilder.CreateIndex(
                name: "ix_account_access_consent_account_and_transaction_api_id",
                table: "account_access_consent",
                column: "account_and_transaction_api_id");

            migrationBuilder.AddForeignKey(
                name: "fk_account_access_consent_account_and_transaction_api_account_a",
                table: "account_access_consent",
                column: "account_and_transaction_api_id",
                principalTable: "account_and_transaction_api",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_bank_registration_bank_bank_id",
                table: "bank_registration",
                column: "bank_id",
                principalTable: "bank",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_domestic_payment_consent_payment_initiation_api_payment_init",
                table: "domestic_payment_consent",
                column: "payment_initiation_api_id",
                principalTable: "payment_initiation_api",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_domestic_vrp_consent_variable_recurring_payments_api_variab",
                table: "domestic_vrp_consent",
                column: "variable_recurring_payments_api_id",
                principalTable: "variable_recurring_payments_api",
                principalColumn: "id");
        }
    }
}
