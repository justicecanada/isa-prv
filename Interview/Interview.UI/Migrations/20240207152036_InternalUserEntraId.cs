using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class InternalUserEntraId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntraUserName",
                table: "InternalUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "EntraId",
                table: "InternalUsers",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntraId",
                table: "InternalUsers");

            migrationBuilder.AddColumn<string>(
                name: "EntraUserName",
                table: "InternalUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
