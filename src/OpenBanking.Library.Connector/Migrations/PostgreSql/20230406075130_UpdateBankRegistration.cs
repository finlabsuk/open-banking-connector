// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class UpdateBankRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "fk_bank_registration_bank_bank_id",
                "bank_registration");

            migrationBuilder.AlterColumn<Guid>(
                "bank_id",
                "bank_registration",
                "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                "authorization_endpoint",
                "bank_registration",
                "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                "jwks_uri",
                "bank_registration",
                "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                "registration_endpoint",
                "bank_registration",
                "text",
                nullable: true);
            
            migrationBuilder.AddColumn<string>(
                "token_endpoint",
                "bank_registration",
                "text",
                nullable: false,
                defaultValue: "");
            
            migrationBuilder.AddForeignKey(
                "fk_bank_registration_bank_bank_id",
                "bank_registration",
                "bank_id",
                "bank",
                principalColumn: "id");
            
            // NB: for PostreSQL, FROM clause effectively creates inner join which is then used for UPDATE
            migrationBuilder.Sql(
                @"UPDATE bank_registration SET (authorization_endpoint, jwks_uri, registration_endpoint, token_endpoint) = (bank.authorization_endpoint, bank.jwks_uri, bank.registration_endpoint, bank.token_endpoint) FROM bank WHERE bank.id = bank_registration.bank_id;");
           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "fk_bank_registration_bank_bank_id",
                "bank_registration");

            migrationBuilder.DropColumn(
                "authorization_endpoint",
                "bank_registration");

            migrationBuilder.DropColumn(
                "jwks_uri",
                "bank_registration");

            migrationBuilder.DropColumn(
                "registration_endpoint",
                "bank_registration");

            migrationBuilder.DropColumn(
                "token_endpoint",
                "bank_registration");

            migrationBuilder.AlterColumn<Guid>(
                "bank_id",
                "bank_registration",
                "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                "fk_bank_registration_bank_bank_id",
                "bank_registration",
                "bank_id",
                "bank",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
