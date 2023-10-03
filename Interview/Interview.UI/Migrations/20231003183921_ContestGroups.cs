using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.UI.Migrations
{
    /// <inheritdoc />
    public partial class ContestGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Contests_ContestId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ContestId",
                table: "Groups");

            migrationBuilder.CreateTable(
                name: "ContestGroup",
                columns: table => new
                {
                    ContestsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestGroup", x => new { x.ContestsId, x.GroupsId });
                    table.ForeignKey(
                        name: "FK_ContestGroup_Contests_ContestsId",
                        column: x => x.ContestsId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestGroup_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContestGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContestGroups_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContestGroup_GroupsId",
                table: "ContestGroup",
                column: "GroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestGroups_ContestId",
                table: "ContestGroups",
                column: "ContestId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestGroups_GroupId",
                table: "ContestGroups",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContestGroup");

            migrationBuilder.DropTable(
                name: "ContestGroups");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ContestId",
                table: "Groups",
                column: "ContestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Contests_ContestId",
                table: "Groups",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
