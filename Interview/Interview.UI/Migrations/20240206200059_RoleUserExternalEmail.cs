using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class RoleUserExternalEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalUserEmail",
                table: "RoleUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalUserEmail",
                table: "RoleUsers");
        }
    }
}
