using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class UserSettingHasListOfEquities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSettings_Equities_EquityId",
                table: "UserSettings");

            migrationBuilder.DropIndex(
                name: "IX_UserSettings_EquityId",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "EquityId",
                table: "UserSettings");

            migrationBuilder.CreateIndex(
                name: "IX_Equities_UserSettingsId",
                table: "Equities",
                column: "UserSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equities_UserSettings_UserSettingsId",
                table: "Equities",
                column: "UserSettingsId",
                principalTable: "UserSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equities_UserSettings_UserSettingsId",
                table: "Equities");

            migrationBuilder.DropIndex(
                name: "IX_Equities_UserSettingsId",
                table: "Equities");

            migrationBuilder.AddColumn<Guid>(
                name: "EquityId",
                table: "UserSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_EquityId",
                table: "UserSettings",
                column: "EquityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettings_Equities_EquityId",
                table: "UserSettings",
                column: "EquityId",
                principalTable: "Equities",
                principalColumn: "Id");
        }
    }
}
