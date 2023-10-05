using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class LanguageTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSettings_UserLanguages_UserLanguageId",
                table: "UserSettings");

            migrationBuilder.DropTable(
                name: "UserLanguages");

            migrationBuilder.DropIndex(
                name: "IX_UserSettings_UserLanguageId",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "UserLanguageId",
                table: "UserSettings");

            migrationBuilder.AddColumn<int>(
                name: "LanguageType",
                table: "UserSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageType",
                table: "UserSettings");

            migrationBuilder.AddColumn<Guid>(
                name: "UserLanguageId",
                table: "UserSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserLanguages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserSettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLanguages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserLanguageId",
                table: "UserSettings",
                column: "UserLanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettings_UserLanguages_UserLanguageId",
                table: "UserSettings",
                column: "UserLanguageId",
                principalTable: "UserLanguages",
                principalColumn: "Id");
        }
    }
}
