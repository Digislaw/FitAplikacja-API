using Microsoft.EntityFrameworkCore.Migrations;

namespace FitAplikacja.Infrastructure.Migrations
{
    public partial class AssignedProductsCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "AssignedProducts",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "AssignedProducts");
        }
    }
}
