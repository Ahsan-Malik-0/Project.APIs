using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.APIs.Migrations
{
    /// <inheritdoc />
    public partial class AddTechfestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Session",
                table: "YearlyBudgets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "EventRequisitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VirtualSocieties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Session = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChairpersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualSocieties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualSocieties_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventVirtualSociety",
                columns: table => new
                {
                    EventsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    virtualSocietiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventVirtualSociety", x => new { x.EventsId, x.virtualSocietiesId });
                    table.ForeignKey(
                        name: "FK_EventVirtualSociety_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventVirtualSociety_VirtualSocieties_virtualSocietiesId",
                        column: x => x.virtualSocietiesId,
                        principalTable: "VirtualSocieties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventVirtualSociety_virtualSocietiesId",
                table: "EventVirtualSociety",
                column: "virtualSocietiesId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualSocieties_MemberId",
                table: "VirtualSocieties",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventVirtualSociety");

            migrationBuilder.DropTable(
                name: "VirtualSocieties");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "EventRequisitions");

            migrationBuilder.AlterColumn<string>(
                name: "Session",
                table: "YearlyBudgets",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
