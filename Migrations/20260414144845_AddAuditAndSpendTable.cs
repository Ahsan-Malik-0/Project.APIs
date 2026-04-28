using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.APIs.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditAndSpendTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FundProvided = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpendAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RevenueGenerated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventAudits_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditSpends",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceiptPiture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventAuditId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditSpends", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditSpends_EventAudits_EventAuditId",
                        column: x => x.EventAuditId,
                        principalTable: "EventAudits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditSpends_EventAuditId",
                table: "AuditSpends",
                column: "EventAuditId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAudits_EventId",
                table: "EventAudits",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditSpends");

            migrationBuilder.DropTable(
                name: "EventAudits");
        }
    }
}
