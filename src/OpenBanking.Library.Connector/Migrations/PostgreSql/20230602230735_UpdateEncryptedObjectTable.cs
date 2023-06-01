using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class UpdateEncryptedObjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "domestic_payment_consent_id",
                table: "encrypted_object",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "domestic_vrp_consent_id",
                table: "encrypted_object",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_encrypted_object_domestic_payment_consent_id",
                table: "encrypted_object",
                column: "domestic_payment_consent_id");

            migrationBuilder.CreateIndex(
                name: "ix_encrypted_object_domestic_vrp_consent_id",
                table: "encrypted_object",
                column: "domestic_vrp_consent_id");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_encrypted_object_domestic_payment_consent_domestic_payment_co",
                table: "encrypted_object");

            migrationBuilder.DropForeignKey(
                name: "fk_encrypted_object_domestic_vrp_consent_domestic_vrp_consent_id",
                table: "encrypted_object");

            migrationBuilder.DropIndex(
                name: "ix_encrypted_object_domestic_payment_consent_id",
                table: "encrypted_object");

            migrationBuilder.DropIndex(
                name: "ix_encrypted_object_domestic_vrp_consent_id",
                table: "encrypted_object");

            migrationBuilder.DropColumn(
                name: "domestic_payment_consent_id",
                table: "encrypted_object");

            migrationBuilder.DropColumn(
                name: "domestic_vrp_consent_id",
                table: "encrypted_object");
        }
    }
}
