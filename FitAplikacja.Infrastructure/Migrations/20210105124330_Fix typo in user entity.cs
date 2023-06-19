using Microsoft.EntityFrameworkCore.Migrations;

namespace FitAplikacja.Infrastructure.Migrations
{
    public partial class Fixtypoinuserentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetHeight",
                table: "AspNetUsers",
                newName: "TargetWeight");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetWeight",
                table: "AspNetUsers",
                newName: "TargetHeight");
        }
    }
}
