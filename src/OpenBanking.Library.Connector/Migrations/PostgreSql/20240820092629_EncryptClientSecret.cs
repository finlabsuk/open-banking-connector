using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class EncryptClientSecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "bank_registration_id",
                table: "encrypted_object",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_encrypted_object_bank_registration_id",
                table: "encrypted_object",
                column: "bank_registration_id");

            migrationBuilder.AddForeignKey(
                name: "fk_encrypted_object_bank_registration_bank_registration_id",
                table: "encrypted_object",
                column: "bank_registration_id",
                principalTable: "bank_registration",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_encrypted_object_bank_registration_bank_registration_id",
                table: "encrypted_object");

            migrationBuilder.DropIndex(
                name: "ix_encrypted_object_bank_registration_id",
                table: "encrypted_object");

            migrationBuilder.DropColumn(
                name: "bank_registration_id",
                table: "encrypted_object");
        }
    }
}
