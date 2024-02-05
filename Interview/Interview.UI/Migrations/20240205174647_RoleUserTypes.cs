using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class RoleUserTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoleType",
                table: "RoleUsers",
                newName: "RoleUserType");

            migrationBuilder.RenameColumn(
                name: "RoleType",
                table: "MockUsers",
                newName: "RoleUserType");

            migrationBuilder.RenameColumn(
                name: "RoleType",
                table: "InterviewUsers",
                newName: "RoleUserType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoleUserType",
                table: "RoleUsers",
                newName: "RoleType");

            migrationBuilder.RenameColumn(
                name: "RoleUserType",
                table: "MockUsers",
                newName: "RoleType");

            migrationBuilder.RenameColumn(
                name: "RoleUserType",
                table: "InterviewUsers",
                newName: "RoleType");
        }
    }
}
