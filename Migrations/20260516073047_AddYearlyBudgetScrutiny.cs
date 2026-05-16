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
            migrationBuilder.DropForeignKey(
                name: "FK_AuditSpends_EventAudits_EventAuditId",
                table: "AuditSpends");

            migrationBuilder.RenameColumn(
                name: "ReceiptPicture",
                table: "AuditSpends",
                newName: "ReceiptPicture");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventAuditId",
                table: "AuditSpends",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "yearlrBudgetScrutinies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdministrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YearlyBydgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YearlyBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_yearlrBudgetScrutinies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_yearlrBudgetScrutinies_Administrations_AdministrationId",
                        column: x => x.AdministrationId,
                        principalTable: "Administrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_yearlrBudgetScrutinies_YearlyBudgets_YearlyBudgetId",
                        column: x => x.YearlyBudgetId,
                        principalTable: "YearlyBudgets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_yearlrBudgetScrutinies_AdministrationId",
                table: "yearlrBudgetScrutinies",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_yearlrBudgetScrutinies_YearlyBudgetId",
                table: "yearlrBudgetScrutinies",
                column: "YearlyBudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditSpends_EventAudits_EventAuditId",
                table: "AuditSpends",
                column: "EventAuditId",
                principalTable: "EventAudits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditSpends_EventAudits_EventAuditId",
                table: "AuditSpends");

            migrationBuilder.DropTable(
                name: "yearlrBudgetScrutinies");

            migrationBuilder.RenameColumn(
                name: "ReceiptPicture",
                table: "AuditSpends",
                newName: "ReceiptPicture");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventAuditId",
                table: "AuditSpends",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditSpends_EventAudits_EventAuditId",
                table: "AuditSpends",
                column: "EventAuditId",
                principalTable: "EventAudits",
                principalColumn: "Id");
        }
    }
}
