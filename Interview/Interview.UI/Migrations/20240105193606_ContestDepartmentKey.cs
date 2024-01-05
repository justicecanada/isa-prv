using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class ContestDepartmentKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Contests");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentKey",
                table: "Contests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentKey",
                table: "Contests");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Contests",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
