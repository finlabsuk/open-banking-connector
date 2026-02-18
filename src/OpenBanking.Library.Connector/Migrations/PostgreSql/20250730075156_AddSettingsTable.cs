using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bank_registration_software_statement_software_statement_id",
                table: "bank_registration");

            migrationBuilder.DropIndex(
                name: "ix_encryption_key_description_legacy_name",
                table: "encryption_key_description");

            migrationBuilder.DropColumn(
                name: "legacy_name",
                table: "encryption_key_description");

            migrationBuilder.DropColumn(
                name: "key_id",
                table: "encrypted_object");

            migrationBuilder.DropColumn(
                name: "access_token_access_token",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "access_token_expires_in",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "access_token_modified",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "access_token_modified_by",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "access_token_refresh_token",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "access_token_access_token",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "access_token_expires_in",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "access_token_modified",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "access_token_modified_by",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "access_token_refresh_token",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "external_api_secret",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "registration_access_token",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "software_statement_profile_id",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "software_statement_profile_override",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "access_token_access_token",
                table: "account_access_consent");

            migrationBuilder.DropColumn(
                name: "access_token_expires_in",
                table: "account_access_consent");

            migrationBuilder.DropColumn(
                name: "access_token_modified",
                table: "account_access_consent");

            migrationBuilder.DropColumn(
                name: "access_token_modified_by",
                table: "account_access_consent");

            migrationBuilder.DropColumn(
                name: "access_token_refresh_token",
                table: "account_access_consent");

            migrationBuilder.AlterColumn<string>(
                name: "external_api_id",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .OldAnnotation("Relational:ColumnOrder", 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "bank_registration_id",
                table: "domestic_vrp_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_state",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 101);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_nonce",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_modified_by",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "auth_context_modified",
                table: "domestic_vrp_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .OldAnnotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "domestic_vrp_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<string>(
                name: "external_api_id",
                table: "domestic_payment_consent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .OldAnnotation("Relational:ColumnOrder", 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "bank_registration_id",
                table: "domestic_payment_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_state",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 101);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_nonce",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_modified_by",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "auth_context_modified",
                table: "domestic_payment_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .OldAnnotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "domestic_payment_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "software_statement_id",
                table: "bank_registration",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "bank_registration",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "external_api_id",
                table: "account_access_consent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .OldAnnotation("Relational:ColumnOrder", 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "bank_registration_id",
                table: "account_access_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_state",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 101);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_nonce",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_modified_by",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "auth_context_modified",
                table: "account_access_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .OldAnnotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "account_access_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_encryption_key_description_id = table.Column<Guid>(type: "uuid", nullable: true),
                    disable_encryption = table.Column<bool>(type: "boolean", nullable: false),
                    modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_settings_encryption_key_description_current_encryption_key_",
                        column: x => x.current_encryption_key_description_id,
                        principalTable: "encryption_key_description",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_settings_current_encryption_key_description_id",
                table: "settings",
                column: "current_encryption_key_description_id");

            migrationBuilder.AddForeignKey(
                name: "fk_bank_registration_software_statement_software_statement_id",
                table: "bank_registration",
                column: "software_statement_id",
                principalTable: "software_statement",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bank_registration_software_statement_software_statement_id",
                table: "bank_registration");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.AddColumn<string>(
                name: "legacy_name",
                table: "encryption_key_description",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "key_id",
                table: "encrypted_object",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "external_api_id",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Relational:ColumnOrder", 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "bank_registration_id",
                table: "domestic_vrp_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_state",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 101);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_nonce",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_modified_by",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "auth_context_modified",
                table: "domestic_vrp_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "domestic_vrp_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<string>(
                name: "access_token_access_token",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 105);

            migrationBuilder.AddColumn<int>(
                name: "access_token_expires_in",
                table: "domestic_vrp_consent",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 106);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "access_token_modified",
                table: "domestic_vrp_consent",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)))
                .Annotation("Relational:ColumnOrder", 108);

            migrationBuilder.AddColumn<string>(
                name: "access_token_modified_by",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 109);

            migrationBuilder.AddColumn<string>(
                name: "access_token_refresh_token",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 107);

            migrationBuilder.AlterColumn<string>(
                name: "external_api_id",
                table: "domestic_payment_consent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Relational:ColumnOrder", 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "bank_registration_id",
                table: "domestic_payment_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_state",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 101);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_nonce",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_modified_by",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "auth_context_modified",
                table: "domestic_payment_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "domestic_payment_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<string>(
                name: "access_token_access_token",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 105);

            migrationBuilder.AddColumn<int>(
                name: "access_token_expires_in",
                table: "domestic_payment_consent",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 106);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "access_token_modified",
                table: "domestic_payment_consent",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)))
                .Annotation("Relational:ColumnOrder", 108);

            migrationBuilder.AddColumn<string>(
                name: "access_token_modified_by",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 109);

            migrationBuilder.AddColumn<string>(
                name: "access_token_refresh_token",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 107);

            migrationBuilder.AlterColumn<Guid>(
                name: "software_statement_id",
                table: "bank_registration",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "bank_registration",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<string>(
                name: "external_api_secret",
                table: "bank_registration",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "registration_access_token",
                table: "bank_registration",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "software_statement_profile_id",
                table: "bank_registration",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "software_statement_profile_override",
                table: "bank_registration",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "external_api_id",
                table: "account_access_consent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Relational:ColumnOrder", 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "bank_registration_id",
                table: "account_access_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_state",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 101);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_nonce",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "auth_context_modified_by",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "auth_context_modified",
                table: "account_access_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "account_access_consent",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<string>(
                name: "access_token_access_token",
                table: "account_access_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 105);

            migrationBuilder.AddColumn<int>(
                name: "access_token_expires_in",
                table: "account_access_consent",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 106);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "access_token_modified",
                table: "account_access_consent",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)))
                .Annotation("Relational:ColumnOrder", 108);

            migrationBuilder.AddColumn<string>(
                name: "access_token_modified_by",
                table: "account_access_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 109);

            migrationBuilder.AddColumn<string>(
                name: "access_token_refresh_token",
                table: "account_access_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 107);

            migrationBuilder.CreateIndex(
                name: "ix_encryption_key_description_legacy_name",
                table: "encryption_key_description",
                column: "legacy_name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_bank_registration_software_statement_software_statement_id",
                table: "bank_registration",
                column: "software_statement_id",
                principalTable: "software_statement",
                principalColumn: "id");
        }
    }
}
