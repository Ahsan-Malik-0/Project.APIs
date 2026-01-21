using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.APIs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequirementTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventsRequirements_Events_EventId",
                table: "EventsRequirements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventsRequirements",
                table: "EventsRequirements");

            migrationBuilder.RenameTable(
                name: "EventsRequirements",
                newName: "EventRequirements");

            migrationBuilder.RenameIndex(
                name: "IX_EventsRequirements_EventId",
                table: "EventRequirements",
                newName: "IX_EventRequirements_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventRequirements",
                table: "EventRequirements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRequirements_Events_EventId",
                table: "EventRequirements",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRequirements_Events_EventId",
                table: "EventRequirements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventRequirements",
                table: "EventRequirements");

            migrationBuilder.RenameTable(
                name: "EventRequirements",
                newName: "EventsRequirements");

            migrationBuilder.RenameIndex(
                name: "IX_EventRequirements_EventId",
                table: "EventsRequirements",
                newName: "IX_EventsRequirements_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventsRequirements",
                table: "EventsRequirements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventsRequirements_Events_EventId",
                table: "EventsRequirements",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
