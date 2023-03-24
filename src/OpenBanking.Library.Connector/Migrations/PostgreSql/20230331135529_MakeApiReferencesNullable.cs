using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class MakeApiReferencesNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_account_access_consent_account_and_transaction_api_account_a",
                table: "account_access_consent");

            migrationBuilder.DropForeignKey(
                name: "fk_domestic_payment_consent_payment_initiation_api_payment_init",
                table: "domestic_payment_consent");

            migrationBuilder.DropForeignKey(
                name: "fk_domestic_vrp_consent_variable_recurring_payments_api_variab",
                table: "domestic_vrp_consent");

            migrationBuilder.AlterColumn<Guid>(
                name: "variable_recurring_payments_api_id",
                table: "domestic_vrp_consent",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "payment_initiation_api_id",
                table: "domestic_payment_consent",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "account_and_transaction_api_id",
                table: "account_access_consent",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "fk_account_access_consent_account_and_transaction_api_account_a",
                table: "account_access_consent",
                column: "account_and_transaction_api_id",
                principalTable: "account_and_transaction_api",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_account_access_consent_account_and_transaction_api_account_a",
                table: "account_access_consent");

            migrationBuilder.DropForeignKey(
                name: "fk_domestic_payment_consent_payment_initiation_api_payment_init",
                table: "domestic_payment_consent");

            migrationBuilder.DropForeignKey(
                name: "fk_domestic_vrp_consent_variable_recurring_payments_api_variab",
                table: "domestic_vrp_consent");

            migrationBuilder.AlterColumn<Guid>(
                name: "variable_recurring_payments_api_id",
                table: "domestic_vrp_consent",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "payment_initiation_api_id",
                table: "domestic_payment_consent",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "account_and_transaction_api_id",
                table: "account_access_consent",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_account_access_consent_account_and_transaction_api_account_a",
                table: "account_access_consent",
                column: "account_and_transaction_api_id",
                principalTable: "account_and_transaction_api",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_domestic_payment_consent_payment_initiation_api_payment_init",
                table: "domestic_payment_consent",
                column: "payment_initiation_api_id",
                principalTable: "payment_initiation_api",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_domestic_vrp_consent_variable_recurring_payments_api_variab",
                table: "domestic_vrp_consent",
                column: "variable_recurring_payments_api_id",
                principalTable: "variable_recurring_payments_api",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
