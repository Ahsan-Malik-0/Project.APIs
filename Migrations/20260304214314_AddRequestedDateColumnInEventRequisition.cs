using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.APIs.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestedDateColumnInEventRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "EventRequisitions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "EventRequisitions");
        }
    }
}
