using Microsoft.EntityFrameworkCore.Migrations;

namespace FitAplikacja.Infrastructure.Migrations
{
    public partial class AssignedProductUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignedProductProduct");

            migrationBuilder.DropTable(
                name: "UserAssignedProducts");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "AssignedProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AssignedProducts_ProductId",
                table: "AssignedProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedProducts_Products_ProductId",
                table: "AssignedProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedProducts_Products_ProductId",
                table: "AssignedProducts");

            migrationBuilder.DropIndex(
                name: "IX_AssignedProducts_ProductId",
                table: "AssignedProducts");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AssignedProducts");

            migrationBuilder.CreateTable(
                name: "AssignedProductProduct",
                columns: table => new
                {
                    AssignedProductsId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedProductProduct", x => new { x.AssignedProductsId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_AssignedProductProduct_AssignedProducts_AssignedProductsId",
                        column: x => x.AssignedProductsId,
                        principalTable: "AssignedProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignedProductProduct_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignedProducts",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignedProducts", x => new { x.ApplicationUserId, x.AssignedProductId });
                    table.ForeignKey(
                        name: "FK_UserAssignedProducts_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAssignedProducts_AssignedProducts_AssignedProductId",
                        column: x => x.AssignedProductId,
                        principalTable: "AssignedProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedProductProduct_ProductsId",
                table: "AssignedProductProduct",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignedProducts_AssignedProductId",
                table: "UserAssignedProducts",
                column: "AssignedProductId");
        }
    }
}
