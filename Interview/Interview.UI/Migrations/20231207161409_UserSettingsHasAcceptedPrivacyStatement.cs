using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class UserSettingsHasAcceptedPrivacyStatement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAcceptedPrivacyStatement",
                table: "UserSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAcceptedPrivacyStatement",
                table: "UserSettings");
        }
    }
}
