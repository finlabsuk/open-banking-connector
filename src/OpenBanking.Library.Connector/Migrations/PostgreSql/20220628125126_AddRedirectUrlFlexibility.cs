using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    public partial class AddRedirectUrlFlexibility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "default_response_mode",
                table: "bank");

            migrationBuilder.AlterColumn<string>(
                name: "nonce_modified_by",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 104)
                .OldAnnotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "nonce_modified",
                table: "domestic_vrp_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 103)
                .OldAnnotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "nonce",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 102)
                .OldAnnotation("Relational:ColumnOrder", 101);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_refresh_token",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 107)
                .OldAnnotation("Relational:ColumnOrder", 106);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_modified_by",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 109)
                .OldAnnotation("Relational:ColumnOrder", 108);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "access_token_modified",
                table: "domestic_vrp_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 108)
                .OldAnnotation("Relational:ColumnOrder", 107);

            migrationBuilder.AlterColumn<int>(
                name: "access_token_expires_in",
                table: "domestic_vrp_consent",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 106)
                .OldAnnotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_access_token",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 105)
                .OldAnnotation("Relational:ColumnOrder", 104);

            migrationBuilder.AddColumn<string>(
                name: "auth_context_state",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 101);

            migrationBuilder.AlterColumn<string>(
                name: "nonce_modified_by",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 104)
                .OldAnnotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "nonce_modified",
                table: "domestic_payment_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 103)
                .OldAnnotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "nonce",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 102)
                .OldAnnotation("Relational:ColumnOrder", 101);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_refresh_token",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 107)
                .OldAnnotation("Relational:ColumnOrder", 106);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_modified_by",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 109)
                .OldAnnotation("Relational:ColumnOrder", 108);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "access_token_modified",
                table: "domestic_payment_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 108)
                .OldAnnotation("Relational:ColumnOrder", 107);

            migrationBuilder.AlterColumn<int>(
                name: "access_token_expires_in",
                table: "domestic_payment_consent",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 106)
                .OldAnnotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_access_token",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 105)
                .OldAnnotation("Relational:ColumnOrder", 104);

            migrationBuilder.AddColumn<string>(
                name: "auth_context_state",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 101);

            migrationBuilder.AddColumn<string>(
                name: "default_redirect_uri",
                table: "bank_registration",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "default_response_mode",
                table: "bank_registration",
                type: "text",
                nullable: false,
                defaultValue: "Fragment");

            migrationBuilder.AddColumn<string>(
                name: "other_redirect_uris",
                table: "bank_registration",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AlterColumn<string>(
                name: "issuer_url",
                table: "bank",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
            
            migrationBuilder.Sql("TRUNCATE TABLE auth_context");

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "auth_context",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "nonce_modified_by",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 104)
                .OldAnnotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "nonce_modified",
                table: "account_access_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 103)
                .OldAnnotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "nonce",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 102)
                .OldAnnotation("Relational:ColumnOrder", 101);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_refresh_token",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 107)
                .OldAnnotation("Relational:ColumnOrder", 106);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_modified_by",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 109)
                .OldAnnotation("Relational:ColumnOrder", 108);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "access_token_modified",
                table: "account_access_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 108)
                .OldAnnotation("Relational:ColumnOrder", 107);

            migrationBuilder.AlterColumn<int>(
                name: "access_token_expires_in",
                table: "account_access_consent",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 106)
                .OldAnnotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_access_token",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 105)
                .OldAnnotation("Relational:ColumnOrder", 104);

            migrationBuilder.AddColumn<string>(
                name: "auth_context_state",
                table: "account_access_consent",
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 101);

           migrationBuilder.CreateIndex(
                name: "ix_auth_context_nonce",
                table: "auth_context",
                column: "nonce",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_auth_context_state",
                table: "auth_context",
                column: "state",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_auth_context_nonce",
                table: "auth_context");

            migrationBuilder.DropIndex(
                name: "ix_auth_context_state",
                table: "auth_context");

            migrationBuilder.DropColumn(
                name: "auth_context_state",
                table: "domestic_vrp_consent");

            migrationBuilder.DropColumn(
                name: "auth_context_state",
                table: "domestic_payment_consent");

            migrationBuilder.DropColumn(
                name: "default_redirect_uri",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "default_response_mode",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "other_redirect_uris",
                table: "bank_registration");

            migrationBuilder.DropColumn(
                name: "state",
                table: "auth_context");

            migrationBuilder.DropColumn(
                name: "auth_context_state",
                table: "account_access_consent");

            migrationBuilder.AlterColumn<string>(
                name: "nonce_modified_by",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 103)
                .OldAnnotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "nonce_modified",
                table: "domestic_vrp_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 102)
                .OldAnnotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<string>(
                name: "nonce",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 101)
                .OldAnnotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_refresh_token",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 106)
                .OldAnnotation("Relational:ColumnOrder", 107);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_modified_by",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 108)
                .OldAnnotation("Relational:ColumnOrder", 109);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "access_token_modified",
                table: "domestic_vrp_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 107)
                .OldAnnotation("Relational:ColumnOrder", 108);

            migrationBuilder.AlterColumn<int>(
                name: "access_token_expires_in",
                table: "domestic_vrp_consent",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 105)
                .OldAnnotation("Relational:ColumnOrder", 106);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_access_token",
                table: "domestic_vrp_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 104)
                .OldAnnotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<string>(
                name: "nonce_modified_by",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 103)
                .OldAnnotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "nonce_modified",
                table: "domestic_payment_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 102)
                .OldAnnotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<string>(
                name: "nonce",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 101)
                .OldAnnotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_refresh_token",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 106)
                .OldAnnotation("Relational:ColumnOrder", 107);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_modified_by",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 108)
                .OldAnnotation("Relational:ColumnOrder", 109);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "access_token_modified",
                table: "domestic_payment_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 107)
                .OldAnnotation("Relational:ColumnOrder", 108);

            migrationBuilder.AlterColumn<int>(
                name: "access_token_expires_in",
                table: "domestic_payment_consent",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 105)
                .OldAnnotation("Relational:ColumnOrder", 106);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_access_token",
                table: "domestic_payment_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 104)
                .OldAnnotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<string>(
                name: "issuer_url",
                table: "bank",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "default_response_mode",
                table: "bank",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "nonce_modified_by",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 103)
                .OldAnnotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "nonce_modified",
                table: "account_access_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 102)
                .OldAnnotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<string>(
                name: "nonce",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 101)
                .OldAnnotation("Relational:ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_refresh_token",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 106)
                .OldAnnotation("Relational:ColumnOrder", 107);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_modified_by",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 108)
                .OldAnnotation("Relational:ColumnOrder", 109);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "access_token_modified",
                table: "account_access_consent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 107)
                .OldAnnotation("Relational:ColumnOrder", 108);

            migrationBuilder.AlterColumn<int>(
                name: "access_token_expires_in",
                table: "account_access_consent",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 105)
                .OldAnnotation("Relational:ColumnOrder", 106);

            migrationBuilder.AlterColumn<string>(
                name: "access_token_access_token",
                table: "account_access_consent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 104)
                .OldAnnotation("Relational:ColumnOrder", 105);
        }
    }
}
