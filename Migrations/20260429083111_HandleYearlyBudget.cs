using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.APIs.Migrations
{
    /// <inheritdoc />
    public partial class HandleYearlyBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YearlyBudgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Session = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AllotedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AllotedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Credits = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SocietyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearlyBudgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YearlyBudgets_Societies_SocietyId",
                        column: x => x.SocietyId,
                        principalTable: "Societies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YearlyEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimateAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimateMonth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearlyBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearlyEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YearlyEvents_YearlyBudgets_YearlyBudgetId",
                        column: x => x.YearlyBudgetId,
                        principalTable: "YearlyBudgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YearlyEventRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    YearlyEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearlyEventRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YearlyEventRequirements_YearlyEvents_YearlyEventId",
                        column: x => x.YearlyEventId,
                        principalTable: "YearlyEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YearlyBudgets_Session",
                table: "YearlyBudgets",
                column: "Session",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YearlyBudgets_SocietyId",
                table: "YearlyBudgets",
                column: "SocietyId");

            migrationBuilder.CreateIndex(
                name: "IX_YearlyEventRequirements_YearlyEventId",
                table: "YearlyEventRequirements",
                column: "YearlyEventId");

            migrationBuilder.CreateIndex(
                name: "IX_YearlyEvents_YearlyBudgetId",
                table: "YearlyEvents",
                column: "YearlyBudgetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YearlyEventRequirements");

            migrationBuilder.DropTable(
                name: "YearlyEvents");

            migrationBuilder.DropTable(
                name: "YearlyBudgets");
        }
    }
}
