using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.APIs.Migrations
{
    /// <inheritdoc />
    public partial class AddYearlyBudgetScrutiny : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YearlyBudgetScrutinies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdministrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YearlyBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearlyBudgetScrutinies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YearlyBudgetScrutinies_Administrations_AdministrationId",
                        column: x => x.AdministrationId,
                        principalTable: "Administrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YearlyBudgetScrutinies_YearlyBudgets_YearlyBudgetId",
                        column: x => x.YearlyBudgetId,
                        principalTable: "YearlyBudgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YearlyBudgets_Societies_SocietyId",
                table: "YearlyBudgets");

            migrationBuilder.DropTable(
                name: "YearlyBudgetScrutinies");

            migrationBuilder.AddColumn<Guid>(
                name: "AdministrationId",
                table: "YearlyBudgets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "YearlyBudgetId",
                table: "YearlyBudgets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_YearlyBudgets_Administrations_AdministrationId",
                table: "YearlyBudgets",
                column: "AdministrationId",
                principalTable: "Administrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_YearlyBudgets_YearlyBudgets_YearlyBudgetId",
                table: "YearlyBudgets",
                column: "YearlyBudgetId",
                principalTable: "YearlyBudgets",
                principalColumn: "Id");
        }
    }
}
