using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class UserSettingNullableRoleId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSettingEquity_Equities_EquityId",
                table: "UserSettingEquity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSettingEquity_UserSettings_UserSettingId",
                table: "UserSettingEquity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSettings_Roles_RoleId",
                table: "UserSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSettingEquity",
                table: "UserSettingEquity");

            migrationBuilder.RenameTable(
                name: "UserSettingEquity",
                newName: "UserSettingEquities");

            migrationBuilder.RenameIndex(
                name: "IX_UserSettingEquity_UserSettingId",
                table: "UserSettingEquities",
                newName: "IX_UserSettingEquities_UserSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSettingEquity_EquityId",
                table: "UserSettingEquities",
                newName: "IX_UserSettingEquities_EquityId");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "UserSettings",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSettingEquities",
                table: "UserSettingEquities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettingEquities_Equities_EquityId",
                table: "UserSettingEquities",
                column: "EquityId",
                principalTable: "Equities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettingEquities_UserSettings_UserSettingId",
                table: "UserSettingEquities",
                column: "UserSettingId",
                principalTable: "UserSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettings_Roles_RoleId",
                table: "UserSettings",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSettingEquities_Equities_EquityId",
                table: "UserSettingEquities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSettingEquities_UserSettings_UserSettingId",
                table: "UserSettingEquities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSettings_Roles_RoleId",
                table: "UserSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSettingEquities",
                table: "UserSettingEquities");

            migrationBuilder.RenameTable(
                name: "UserSettingEquities",
                newName: "UserSettingEquity");

            migrationBuilder.RenameIndex(
                name: "IX_UserSettingEquities_UserSettingId",
                table: "UserSettingEquity",
                newName: "IX_UserSettingEquity_UserSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSettingEquities_EquityId",
                table: "UserSettingEquity",
                newName: "IX_UserSettingEquity_EquityId");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "UserSettings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSettingEquity",
                table: "UserSettingEquity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettingEquity_Equities_EquityId",
                table: "UserSettingEquity",
                column: "EquityId",
                principalTable: "Equities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettingEquity_UserSettings_UserSettingId",
                table: "UserSettingEquity",
                column: "UserSettingId",
                principalTable: "UserSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettings_Roles_RoleId",
                table: "UserSettings",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
