using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddEncryptedObjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "encrypted_object",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    key_id = table.Column<string>(type: "text", nullable: true),
                    discriminator = table.Column<string>(type: "text", nullable: false),
                    nonce = table.Column<byte[]>(type: "bytea", nullable: false),
                    tag = table.Column<byte[]>(type: "bytea", nullable: false),
                    text = table.Column<byte[]>(type: "bytea", nullable: false),
                    text2 = table.Column<string>(type: "text", nullable: true),
                    account_access_consent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reference = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted_modified_by = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_encrypted_object", x => x.id);
                    table.ForeignKey(
                        name: "fk_encrypted_object_account_access_consent_account_access_consen",
                        column: x => x.account_access_consent_id,
                        principalTable: "account_access_consent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_encrypted_object_account_access_consent_id",
                table: "encrypted_object",
                column: "account_access_consent_id");

            migrationBuilder.CreateIndex(
                name: "ix_encrypted_object_nonce",
                table: "encrypted_object",
                column: "nonce",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "encrypted_object");
        }
    }
}
