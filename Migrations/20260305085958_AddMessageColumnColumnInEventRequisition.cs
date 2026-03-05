using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.APIs.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageColumnColumnInEventRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewMessage",
                table: "EventRequisitions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewMessage",
                table: "EventRequisitions");
        }
    }
}
