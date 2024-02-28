using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class InterviewUserListInterviewUserEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InterviewUserEmails_InterviewUserId",
                table: "InterviewUserEmails",
                column: "InterviewUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewUserEmails_InterviewUsers_InterviewUserId",
                table: "InterviewUserEmails",
                column: "InterviewUserId",
                principalTable: "InterviewUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewUserEmails_InterviewUsers_InterviewUserId",
                table: "InterviewUserEmails");

            migrationBuilder.DropIndex(
                name: "IX_InterviewUserEmails_InterviewUserId",
                table: "InterviewUserEmails");
        }
    }
}
