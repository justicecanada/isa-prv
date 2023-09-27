using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class UserSettingNullableEquityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSettings_Equities_EquityId",
                table: "UserSettings");

            migrationBuilder.AlterColumn<Guid>(
                name: "EquityId",
                table: "UserSettings",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettings_Equities_EquityId",
                table: "UserSettings",
                column: "EquityId",
                principalTable: "Equities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSettings_Equities_EquityId",
                table: "UserSettings");

            migrationBuilder.AlterColumn<Guid>(
                name: "EquityId",
                table: "UserSettings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettings_Equities_EquityId",
                table: "UserSettings",
                column: "EquityId",
                principalTable: "Equities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
