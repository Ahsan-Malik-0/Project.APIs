using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.APIs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventRequisitionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRequisitions_Events__eventId",
                table: "EventRequisitions");

            migrationBuilder.DropIndex(
                name: "IX_EventRequisitions__eventId",
                table: "EventRequisitions");

            migrationBuilder.DropColumn(
                name: "_eventId",
                table: "EventRequisitions");

            migrationBuilder.RenameColumn(
                name: "SocietyId",
                table: "EventRequisitions",
                newName: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRequisitions_EventId",
                table: "EventRequisitions",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRequisitions_Events_EventId",
                table: "EventRequisitions",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRequisitions_Events_EventId",
                table: "EventRequisitions");

            migrationBuilder.DropIndex(
                name: "IX_EventRequisitions_EventId",
                table: "EventRequisitions");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "EventRequisitions",
                newName: "SocietyId");

            migrationBuilder.AddColumn<Guid>(
                name: "_eventId",
                table: "EventRequisitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventRequisitions__eventId",
                table: "EventRequisitions",
                column: "_eventId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRequisitions_Events__eventId",
                table: "EventRequisitions",
                column: "_eventId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
