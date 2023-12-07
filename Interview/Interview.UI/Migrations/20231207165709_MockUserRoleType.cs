using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class MockUserRoleType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "MockUsers");

            migrationBuilder.AddColumn<int>(
                name: "RoleType",
                table: "MockUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "MockUsers");

            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "MockUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
