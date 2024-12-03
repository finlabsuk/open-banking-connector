using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddEncryptionKeyDescriptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "encryption_key_description_id",
                table: "encrypted_object",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "encryption_key_description",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    legacy_name = table.Column<string>(type: "text", nullable: true),
                    key = table.Column<string>(type: "text", nullable: false),
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_encryption_key_description", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_encrypted_object_encryption_key_description_id",
                table: "encrypted_object",
                column: "encryption_key_description_id");

            migrationBuilder.CreateIndex(
                name: "ix_encryption_key_description_legacy_name",
                table: "encryption_key_description",
                column: "legacy_name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_encrypted_object_encryption_key_description_encryption_key_",
                table: "encrypted_object",
                column: "encryption_key_description_id",
                principalTable: "encryption_key_description",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_encrypted_object_encryption_key_description_encryption_key_",
                table: "encrypted_object");

            migrationBuilder.DropTable(
                name: "encryption_key_description");

            migrationBuilder.DropIndex(
                name: "ix_encrypted_object_encryption_key_description_id",
                table: "encrypted_object");

            migrationBuilder.DropColumn(
                name: "encryption_key_description_id",
                table: "encrypted_object");
        }
    }
}
