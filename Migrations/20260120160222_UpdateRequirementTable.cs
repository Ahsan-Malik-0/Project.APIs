using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.APIs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequirementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventsRequirement_Events_EventId",
                table: "EventsRequirement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventsRequirement",
                table: "EventsRequirement");

            migrationBuilder.RenameTable(
                name: "EventsRequirement",
                newName: "EventsRequirements");

            migrationBuilder.RenameIndex(
                name: "IX_EventsRequirement_EventId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventsRequirements_Events_EventId",
                table: "EventsRequirements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventsRequirements",
                table: "EventsRequirements");

            migrationBuilder.RenameTable(
                name: "EventsRequirements",
                newName: "EventsRequirement");

            migrationBuilder.RenameIndex(
                name: "IX_EventsRequirements_EventId",
                table: "EventsRequirement",
                newName: "IX_EventsRequirement_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventsRequirement",
                table: "EventsRequirement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventsRequirement_Events_EventId",
                table: "EventsRequirement",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
