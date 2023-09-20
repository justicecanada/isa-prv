using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class ScheduleScheduleTypeEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_ScheduleTypes_ScheduleTypeId",
                table: "Schedules");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleTypeId",
                table: "Schedules",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleType",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_ScheduleTypes_ScheduleTypeId",
                table: "Schedules",
                column: "ScheduleTypeId",
                principalTable: "ScheduleTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_ScheduleTypes_ScheduleTypeId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "ScheduleType",
                table: "Schedules");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleTypeId",
                table: "Schedules",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_ScheduleTypes_ScheduleTypeId",
                table: "Schedules",
                column: "ScheduleTypeId",
                principalTable: "ScheduleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
