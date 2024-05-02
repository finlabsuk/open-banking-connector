using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class UpdateSoftwareStatement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_account_access_consent_bank_registration_bank_registration_id",
                table: "account_access_consent");

            migrationBuilder.DropForeignKey(
                name: "fk_auth_context_account_access_consent_account_access_consent_id",
                table: "auth_context");

            migrationBuilder.DropForeignKey(
                name: "fk_auth_context_domestic_payment_consent_domestic_payment_consen",
                table: "auth_context");

            migrationBuilder.DropForeignKey(
                name: "fk_domestic_payment_consent_bank_registration_bank_registration",
                table: "domestic_payment_consent");

            migrationBuilder.DropForeignKey(
                name: "fk_encrypted_object_account_access_consent_account_access_consen",
                table: "encrypted_object");

            migrationBuilder.DropForeignKey(
                name: "fk_encrypted_object_domestic_payment_consent_domestic_payment_co",
                table: "encrypted_object");

            migrationBuilder.DropForeignKey(
                name: "fk_encrypted_object_domestic_vrp_consent_domestic_vrp_consent_id",
                table: "encrypted_object");

            migrationBuilder.DropForeignKey(
                name: "fk_software_statement_ob_seal_certificate_default_ob_seal_certifi",
                table: "software_statement");

            migrationBuilder.DropForeignKey(
                name: "fk_software_statement_ob_wac_certificate_default_ob_wac_certifica",
                table: "software_statement");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "modified",
                table: "software_statement",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
            
            migrationBuilder.AlterColumn<string>(
                name: "discriminator",
                table: "encrypted_object",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "discriminator",
                table: "auth_context",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "fk_account_access_consent_bank_registration_bank_registration_",
                table: "account_access_consent",
                column: "bank_registration_id",
                principalTable: "bank_registration",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_auth_context_account_access_consent_account_access_consent_",
                table: "auth_context",
                column: "account_access_consent_id",
                principalTable: "account_access_consent",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_auth_context_domestic_payment_consent_domestic_payment_cons",
                table: "auth_context",
                column: "domestic_payment_consent_id",
                principalTable: "domestic_payment_consent",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_domestic_payment_consent_bank_registration_bank_registratio",
                table: "domestic_payment_consent",
                column: "bank_registration_id",
                principalTable: "bank_registration",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_encrypted_object_account_access_consent_account_access_cons",
                table: "encrypted_object",
                column: "account_access_consent_id",
                principalTable: "account_access_consent",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_encrypted_object_domestic_payment_consent_domestic_payment_",
                table: "encrypted_object",
                column: "domestic_payment_consent_id",
                principalTable: "domestic_payment_consent",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_encrypted_object_domestic_vrp_consent_domestic_vrp_consent_",
                table: "encrypted_object",
                column: "domestic_vrp_consent_id",
                principalTable: "domestic_vrp_consent",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_software_statement_ob_seal_certificate_default_ob_seal_cert",
                table: "software_statement",
                column: "default_ob_seal_certificate_id",
                principalTable: "ob_seal_certificate",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_software_statement_ob_wac_certificate_default_ob_wac_certif",
                table: "software_statement",
                column: "default_ob_wac_certificate_id",
                principalTable: "ob_wac_certificate",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
            
            migrationBuilder.Sql(@"UPDATE software_statement SET modified = created;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_account_access_consent_bank_registration_bank_registration_",
                table: "account_access_consent");

            migrationBuilder.DropForeignKey(
                name: "fk_auth_context_account_access_consent_account_access_consent_",
                table: "auth_context");

            migrationBuilder.DropForeignKey(
                name: "fk_auth_context_domestic_payment_consent_domestic_payment_cons",
                table: "auth_context");

            migrationBuilder.DropForeignKey(
                name: "fk_domestic_payment_consent_bank_registration_bank_registratio",
                table: "domestic_payment_consent");

            migrationBuilder.DropForeignKey(
                name: "fk_encrypted_object_account_access_consent_account_access_cons",
                table: "encrypted_object");

            migrationBuilder.DropForeignKey(
                name: "fk_encrypted_object_domestic_payment_consent_domestic_payment_",
                table: "encrypted_object");

            migrationBuilder.DropForeignKey(
                name: "fk_encrypted_object_domestic_vrp_consent_domestic_vrp_consent_",
                table: "encrypted_object");

            migrationBuilder.DropForeignKey(
                name: "fk_software_statement_ob_seal_certificate_default_ob_seal_cert",
                table: "software_statement");

            migrationBuilder.DropForeignKey(
                name: "fk_software_statement_ob_wac_certificate_default_ob_wac_certif",
                table: "software_statement");

            migrationBuilder.DropColumn(
                name: "modified",
                table: "software_statement");

            migrationBuilder.AlterColumn<string>(
                name: "discriminator",
                table: "encrypted_object",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(34)",
                oldMaxLength: 34);

            migrationBuilder.AlterColumn<string>(
                name: "discriminator",
                table: "auth_context",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(34)",
                oldMaxLength: 34);

            migrationBuilder.AddForeignKey(
                name: "fk_account_access_consent_bank_registration_bank_registration_id",
                table: "account_access_consent",
                column: "bank_registration_id",
                principalTable: "bank_registration",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_auth_context_account_access_consent_account_access_consent_id",
                table: "auth_context",
                column: "account_access_consent_id",
                principalTable: "account_access_consent",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_auth_context_domestic_payment_consent_domestic_payment_consen",
                table: "auth_context",
                column: "domestic_payment_consent_id",
                principalTable: "domestic_payment_consent",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_domestic_payment_consent_bank_registration_bank_registration",
                table: "domestic_payment_consent",
                column: "bank_registration_id",
                principalTable: "bank_registration",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_encrypted_object_account_access_consent_account_access_consen",
                table: "encrypted_object",
                column: "account_access_consent_id",
                principalTable: "account_access_consent",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_encrypted_object_domestic_payment_consent_domestic_payment_co",
                table: "encrypted_object",
                column: "domestic_payment_consent_id",
                principalTable: "domestic_payment_consent",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_encrypted_object_domestic_vrp_consent_domestic_vrp_consent_id",
                table: "encrypted_object",
                column: "domestic_vrp_consent_id",
                principalTable: "domestic_vrp_consent",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_software_statement_ob_seal_certificate_default_ob_seal_certifi",
                table: "software_statement",
                column: "default_ob_seal_certificate_id",
                principalTable: "ob_seal_certificate",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_software_statement_ob_wac_certificate_default_ob_wac_certifica",
                table: "software_statement",
                column: "default_ob_wac_certificate_id",
                principalTable: "ob_wac_certificate",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
