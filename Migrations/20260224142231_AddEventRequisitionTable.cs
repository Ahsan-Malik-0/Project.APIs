using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.APIs.Migrations
{
    /// <inheritdoc />
    public partial class AddEventRequisitionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Picture",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "EventRequisitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllocatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BiitContribution = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SocietyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    _eventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRequisitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventRequisitions_Events__eventId",
                        column: x => x._eventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventRequisitions__eventId",
                table: "EventRequisitions",
                column: "_eventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventRequisitions");

            migrationBuilder.AlterColumn<string>(
                name: "Picture",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
