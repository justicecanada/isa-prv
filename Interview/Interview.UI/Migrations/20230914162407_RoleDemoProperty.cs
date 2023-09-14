using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class RoleDemoProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DemoProperty",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DemoProperty",
                table: "Roles");
        }
    }
}
